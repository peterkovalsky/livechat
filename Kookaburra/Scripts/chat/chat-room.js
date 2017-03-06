function ChatRoomViewModel(operatorName) {
  
    var self = this;

    self.operatorName = operatorName;
    self.newText = ko.observable("");
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
            if (result.conversations && result.conversations.length > 0) {
                $.each(result.conversations, function (index, item) {
                    self.conversations.push(new Conversation(item));
                });
               
                self.setCurrentChat();
                $('#conversation').perfectScrollbar();
                self.scrollDown();
            }
        });
    };

    self.scrollDown = function () {
        $('#conversation').animate({ scrollTop: $('#messages').prop('scrollHeight') }, "slow");
    }

    // Sends message to visitor on Enter Press
    // ----------------------------------------
    self.addEnterPressEvent = function () {
        $(document).keypress(function (e) {
            if (e.which == 13) {
                //self.currentChat().messages.push(new Message({
                //    author: self.operatorName,
                //    text: self.newText(),
                //    sentBy: 'operator',
                //    time: moment()                                                  
                //}, true));
                $.connection.chatHub.server.sendToVisitor(self.operatorName, self.newText(), self.currentChat().sessionId());

                self.newText(''); // clear input area

                e.preventDefault();
            }
        });
    };


    // Operator wants to disconnect visitor
    // ------------------------------------
    self.disconnect = function () {
        if (confirm("Are you sure you want to disconnect " + self.currentChat().visitorName() + "?")) {
            $.connection.chatHub.server.finishChattingWithVisitor(self.currentChat().sessionId());
        }        
    };


    // Operator switches to another chat
    // ---------------------------------
    self.switchChat = function (conversation) {

        self.setCurrentChat(conversation);

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
        $.connection.chatHub.client.visitorConnected = function (conversationView) {

            self.conversations.push(new Conversation(conversationView))
            self.setCurrentChat();
        };

        // Message from visitor/operator
        $.connection.chatHub.client.sendMessageToOperator = function (message, sessionId) {

            var conversation = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.sessionId() == sessionId;
            });

            if (conversation) {
                conversation.messages.push(new Message(message, conversation.isCurrent()));

                $('#conversation').perfectScrollbar('update');
                self.scrollDown();
            }
        };

        // Visitor stopped chat
        $.connection.chatHub.client.visitorDisconnectedByVisitor = function (result) {

            var conversationToClose = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.sessionId() == result.visitorSessionId;
            });

            if (conversationToClose)
            {
                if (conversationToClose.isCurrent()) {
                    conversationToClose.messages.push(new Message({                        
                        text: 'Visitor closed the chat.',
                        sentBy: 'system',
                        time: result.time
                    }, true));

                    conversationToClose.isClosed(true);
                }
                else {
                    self.conversations.remove(conversationToClose);
                }
            }            
        };

        // Operator stopped chat
        $.connection.chatHub.client.visitorDisconnectedByOperator = function (result) {

            var conversationToClose = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.sessionId() == result.visitorSessionId;
            });

            if (conversationToClose) {
                self.conversations.remove(conversationToClose);                
            }
        };
    };  
}

function Message(data, isRead) {
    var self = this;

    self.author = ko.observable(data.author);
    self.text = ko.observable(data.text);
    self.sentBy = ko.observable(data.sentBy);

    var now = ko.observable(new Date());
    setInterval(function () { now(new Date()); }, 60 * 1000);

    self.time = ko.computed(function () {
        return moment(data.time).startOf('minute').from(now());
    });

    self.read = ko.observable(isRead);
}

function Conversation(data) {
    var self = this;

    self.sessionId = ko.observable(data.sessionId);
    self.visitorName = ko.observable(data.visitorName);
    self.startTime = ko.observable(data.startTime);
    self.location = ko.observable(data.location);
    self.currentUrl = ko.observable(data.currentUrl);
    self.messages = ko.observableArray([]);
    self.isClosed = ko.observable(false);

    if (data.messages) {
        $.each(data.messages, function (index, item) {
            self.messages.push(new Message(item, true));
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
}