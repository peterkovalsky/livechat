function ChatRoomViewModel(operatorName) {
  
    var self = this;

    self.operatorName = operatorName;
    self.newText = ko.observable("");
    self.conversations = ko.observableArray([]);
    self.currentChat = ko.computed(function () {
        var result = $.grep(self.conversations(), function (e) { return e.isCurrent() == true; });
        if (result.length > 0) {
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
        self.startOrResumeChat();
    };

    // Resume operator chats
    // -------------------------
    self.startOrResumeChat = function () {
        $.connection.chatHub.server.resumeOperatorChat().done(function (result) {
            if (result.conversations.length > 0) {
                self.conversations(result.conversations);
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
                    read: true,
                    me: true
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

        for (var i = 0; i < self.conversations().length; i++) {
            self.conversations()[i].isCurrent(false);
        }

        conversation.isCurrent(true);

        // mark all messages as READ in the current conversation
        ko.utils.arrayForEach(conversation.messages(), function (item) {
            item.read(true);
        });
    };
      

    // Register SignalR callbacks
    // -----------------------------
    self.registerCallbackFunctions = function () {
        // Visitor just CONNECTED
        $.connection.chatHub.client.visitorConnected = function (conversationView) {

            self.conversations.push(new Conversation(
                {                    
                    sessionId: conversationView.sessionId,
                    visitorName: conversationView.visitorName,
                    startTime: conversationView.time,
                    location: conversationView.location,
                    currentUrl: conversationView.currentUrl,
                    isCurrent: (self.conversations().length == 0 ? true : false)
                }))
        };

        // Message from visitor
        $.connection.chatHub.client.sendMessageToOperator = function (message, sessionId) {

            var conversation = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.sessionId() == sessionId;
            });

            if (conversation) {
                conversation.messages.push(new Message({
                    author: message.author,
                    text: message.text,
                    time: moment(message.time).format('LT'),
                    read: conversation.isCurrent(),
                    me: false
                }));
            }
        };

        // Visitor just has been DISCONNECTED 
        $.connection.chatHub.client.visitorDisconnected = function (clientId, name, time) {

        };
    };  
}

function Message(data) { 
    this.author = ko.observable(data.author);
    this.text = ko.observable(data.text);
    this.sentBy = ko.observable(data.sentBy);
   
    this.time = ko.observable(data.time);
    this.read = ko.observable(data.read);
    this.me = ko.observable(data.me);
}

function Conversation(data) {
    var self = this;

    self.sessionId = ko.observable(data.sessionId);
    self.visitorName = ko.observable(data.visitorName);
    self.startTime = ko.observable(data.startTime);
    self.location = ko.observable(data.location);
    self.currentUrl = ko.observable(data.currentUrl);

    self.messages = ko.observableArray([]);

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

        return unreadMessages.length;
    });
}