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
            }
        });
    };

    // Sends message to visitor on Enter Press
    // ----------------------------------------
    self.addEnterPressEvent = function () {
        $(document).keypress(function (e) {
            if (e.which == 13) {
                self.currentChat().messages.push(new Message({
                    author: self.operatorName,
                    text: self.newText(),
                    time: moment().format('LT'),
                    sentBy: 'operator',
                    read: true,                 
                }));
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
            $.connection.chatHub.server.disconnectVisitor(self.currentChat().sessionId());

            ko.utils.arrayRemoveItem(self.conversations(), self.currentChat());
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
        // Visitor just CONNECTED
        $.connection.chatHub.client.visitorConnected = function (conversationView) {

            self.conversations.push(new Conversation(conversationView))
            self.setCurrentChat();
        };

        // Message from visitor
        $.connection.chatHub.client.sendMessageToOperator = function (message, sessionId) {

            var conversation = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.sessionId() == sessionId;
            });

            if (conversation) {
                conversation.messages.push(new Message(message, conversation.isCurrent()));
            }
        };

        // Visitor just has been DISCONNECTED 
        $.connection.chatHub.client.visitorDisconnected = function (clientId, name, time) {

        };
    };  
}

function Message(data, isRead) { 
    this.author = ko.observable(data.author);
    this.text = ko.observable(data.text);
    this.sentBy = ko.observable(data.sentBy);   
    this.time = ko.observable(moment(data.time).format('LT'));

    this.read = ko.observable(isRead);
}

function Conversation(data) {
    var self = this;

    self.sessionId = ko.observable(data.sessionId);
    self.visitorName = ko.observable(data.visitorName);
    self.startTime = ko.observable(data.startTime);
    self.location = ko.observable(data.location);
    self.currentUrl = ko.observable(data.currentUrl);
    self.messages = ko.observableArray([]);

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