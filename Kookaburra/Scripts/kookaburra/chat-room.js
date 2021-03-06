﻿function ChatRoomViewModel(operatorName, chatId) {
  
    var self = this;

    self.operatorName = operatorName;
    self.newText = ko.observable("");
    self.enterMessageFocus = ko.observable(true);
    self.conversations = ko.observableArray([]);
    self.currentChat = ko.computed(function () {
        var result = $.grep(self.conversations(), function (e) { return e.isCurrent() == true; });
        if (result && result.length > 0) {
            return result[0];
        } else {
            return null;
        }
    });


    // Init operator chat room
    // -----------------------
    self.init = function () {        
        self.registerCallbackFunctions();
        self.addEnterPressEvent();        

        $(window).on('signalr.start', function (e) {
            self.startOrResumeChat();
        });    
    };

    // Resume operator chats
    // -------------------------
    self.startOrResumeChat = function () {
        $.connection.chatHub.server.resumeOperatorChat().done(function (result) {
            if (result && result.conversations && result.conversations.length > 0) {
                $.each(result.conversations, function (index, item) {
                    self.conversations.push(new Conversation(item));
                });
               
                self.setCurrentChat();                

                $('.scrollbar-macosx').scrollbar({ scrollStep: 5});
                $('[data-toggle="tooltip"]').tooltip();

                self.scrollDown();
                self.enterMessageFocus(true);
            }            
        });
    };

    self.scrollDown = function () {
        $('#conversation').animate({ scrollTop: $('#conversation').prop('scrollHeight') }, "slow");        
    }

    // Sends message to visitor on Enter Press
    // ----------------------------------------
    self.addEnterPressEvent = function () {
        $(document).keypress(function (e) {
            if (e.which == 13) {                

                if (!isNullOrWhitespace(self.newText())) {

                    var newMessageId = new Date().valueOf();

                    self.currentChat().messages.push(new Message({
                        id: newMessageId,
                        author: self.operatorName,
                        text: self.newText(),
                        sentBy: 'operator',
                        time: moment()
                    }, true, true));

                    self.scrollDown();
                    $('[data-toggle="tooltip"]').tooltip();

                    $.connection.chatHub.server.sendToVisitor(self.operatorName, self.newText(), self.currentChat().sessionId(), newMessageId)
                        .done(function (data) {
                            var conversations = $.grep(self.conversations(), function (e) {
                                return e.sessionId() == data.visitorSessionId && !e.isClosed();
                            });

                            if (conversations && conversations.length > 0) {
                                var messages = $.grep(conversations[0].messages(), function (e) { return e.id == data.messageId; });
                                if (messages && messages.length > 0) {
                                    messages[0].sending(false);
                                }
                            }
                        });
                }

                self.newText(''); // clear input area

                e.preventDefault();
            }
        });
    };


    // Operator wants to disconnect visitor
    // ------------------------------------
    self.disconnect = function () {
        alertify.confirm("Are you sure you want to end chat with " + self.currentChat().visitorName() + "?", function () {
            $.connection.chatHub.server.finishChattingWithVisitor(self.currentChat().sessionId());

            self.currentChat().messages.push(new Message({
                text: 'You ended chat with ' + self.currentChat().visitorName(),
                sentBy: 'system',
                time: moment()
            }, true, false));

            self.scrollDown();
            self.currentChat().isClosed(true);            
        });
    };


    // Operator switches to another chat
    // ---------------------------------
    self.switchChat = function (conversation) {

        self.setCurrentChat(conversation);

        $('.scrollbar-macosx').scrollbar({ scrollStep: 5 });
        $('[data-toggle="tooltip"]').tooltip();
        $('#conversation').scrollTop($('#conversation').prop("scrollHeight"));

        // mark all messages as READ in the current conversation
        ko.utils.arrayForEach(conversation.messages(), function (item) {
            item.read(true);
        });        
    };
      
    // Set a current chat. If null argument - set first chat as a current one
    // ----------------------------------------------------------------------
    self.setCurrentChat = function (conversation) {
        if (self.conversations().length > 0) {
            for (var i = 0; i < self.conversations().length; i++) {
                self.conversations()[i].isCurrent(false);
            }

            if (conversation) {
                conversation.isCurrent(true);
            }
            else {
                self.conversations()[0].isCurrent(true);
            }
        }
    };

    // Register SignalR callbacks
    // -----------------------------
    self.registerCallbackFunctions = function () {

        // Visitor connected
        $.connection.visitorHub.client.visitorConnected = function (conversationView) {

            self.conversations.push(new Conversation(conversationView))
            self.setCurrentChat();

            $('.scrollbar-macosx').scrollbar({ scrollStep: 5 });
            $('[data-toggle="tooltip"]').tooltip();
        };

        // Message from visitor/operator
        var messageToOperatorEventHandler = function (message, sessionId) {

            var conversation = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.sessionId() == sessionId && !c.isClosed();
            });

            if (conversation) {
                conversation.messages.push(new Message(message, conversation.isCurrent(), false));

                self.scrollDown();
                $('[data-toggle="tooltip"]').tooltip();
            }
        };

        $.connection.visitorHub.client.sendMessageToOperator = messageToOperatorEventHandler;
        $.connection.chatHub.client.sendMessageToOperator = messageToOperatorEventHandler;

        // Visitor stopped chat
        var visitorDisconnectedEventHandler = function (result) {

            var conversationToClose = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.sessionId() == result.visitorSessionId && !c.isClosed();
            });

            if (conversationToClose) {
                if (conversationToClose.isCurrent()) {
                    var disconnectMessage = 'Chat has been stopped.';
                    if (result.disconnectedBy == 'Operator') {
                        disconnectMessage = 'You ended chat with ' + conversationToClose.visitorName();
                    }
                    else if (result.disconnectedBy == 'Visitor') {
                        disconnectMessage = 'Visitor closed the chat.';
                    }
                    else if (result.disconnectedBy == 'System') {
                        disconnectMessage = 'Chat was closed due to inactivity.';
                    }

                    conversationToClose.messages.push(new Message({
                        text: disconnectMessage,
                        sentBy: 'system',
                        time: result.time
                    }, true, false));

                    conversationToClose.isClosed(true);
                }
                else {
                    self.conversations.remove(conversationToClose);
                }
            }
        };

        $.connection.chatHub.client.visitorDisconnected = visitorDisconnectedEventHandler;
        $.connection.visitorHub.client.visitorDisconnected = visitorDisconnectedEventHandler;
    };
}

function Message(data, isRead, isSending) {
    var self = this;

    self.id = data.id;
    self.author = ko.observable(data.author);
    self.text = ko.observable(data.text);
    self.sentBy = ko.observable(data.sentBy);
    self.timeSent = ko.observable(data.time);
    self.sending = ko.observable(isSending);

    var now = ko.observable(new Date());
    setInterval(function () { now(new Date()); }, 60 * 1000);

    self.time = ko.computed(function () {
        return moment(self.timeSent()).startOf('minute').from(now());
    });

    self.read = ko.observable(isRead);
}

function MessageGroup(author, sentBy, messages) {
    var self = this;

    self.author = ko.observable(author);    
    self.sentBy = ko.observable(sentBy);
    self.messages = ko.observableArray(messages);
}

function Conversation(data) {
    var self = this;

    self.sessionId = ko.observable(data.sessionId);
    self.visitorName = ko.observable(data.visitorName);
    self.email = ko.observable(data.email);
    self.startTime = ko.observable(data.startTime);
    self.country = ko.observable(data.country);
    self.city = ko.observable(data.city);
    self.region = ko.observable(data.region);    
    self.currentUrl = ko.observable(data.currentUrl);
    self.messages = ko.observableArray([]);
    self.isClosed = ko.observable(false);

    if (data.messages) {
        $.each(data.messages, function (index, item) {
            self.messages.push(new Message(item, true, false));
        });
    }

    self.isCurrent = ko.observable(data.isCurrent);

    self.startTimeFormatted = ko.computed(function () {
        return moment(self.startTime()).calendar();
    });

    self.visitorNameFormatted = ko.computed(function () {
        return self.visitorName().toUpperCase();
    });

    self.unreadMessages = ko.computed(function () {
        var unreadMessages = ko.utils.arrayFilter(self.messages(), function (item) {
            return !item.read();
        });

        if (unreadMessages)
            return unreadMessages.length;
        else
            return 0;
    });

    self.lastMessage = ko.computed(function () {
        var unreadMessages = ko.utils.arrayFilter(self.messages(), function (item) {
            return !item.read();
        });

        if (unreadMessages)
            return unreadMessages.length;
        else
            return 0;
    });

    self.lastMessageTime = ko.computed(function () {
        var lastMessage = self.messages()[self.messages().length - 1];

        if (lastMessage) {
            return lastMessage.time();
        }

        return null;
    });

    self.groupedMessages = ko.computed(function () {
        var groups = [];
        var tempMessages = [];

        for (var i = 0; i < self.messages().length; i++) {

            var currentMessage = self.messages()[i];            
            
            tempMessages.push(currentMessage);

            // check if it's not the last message
            if (i+1 < self.messages().length)
            {
                var nextMessage = self.messages()[i + 1];

                if (currentMessage.sentBy() != nextMessage.sentBy())
                {
                    groups.push(new MessageGroup(currentMessage.author(), currentMessage.sentBy(), tempMessages));
                    tempMessages = [];
                }
            }
            else
            {
                groups.push(new MessageGroup(currentMessage.author(), currentMessage.sentBy(), tempMessages));
                tempMessages = [];
            }
        }

        return groups;
    });
}