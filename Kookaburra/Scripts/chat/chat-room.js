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


    // init operator chat room
    // -----------------------
    self.init = function () {        
        self.registerCallbackFunctions();
        self.addEnterPressEvent();
    };

    // Start or resume chat room
    // -------------------------
    self.startOrResumeChat = function () {
        $.connection.chatHub.server.resumeOperatorChat().done(function (result) {
            self.conversations(result.conversations);
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
                $.connection.chatHub.server.sendToVisitor(self.operatorName, self.newText(), self.currentChat().visitorId());

                self.newText(''); // clear input area

                e.preventDefault();
            }
        });
    };


    // Operator wants to disconnect visitor
    // ------------------------------------
    self.disconnect = function () {
        if (confirm("Are you sure you want to disconnect " + self.currentChat().visitorName() + "?")) {
            $.connection.chatHub.server.disconnectVisitor(self.currentChat().visitorId());

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
        $.connection.chatHub.client.visitorConnected = function (visitorInfo) {

            self.conversations.push(new Conversation(
                {
                    visitorId: visitorInfo.sessionId,
                    visitorName: visitorInfo.name,
                    startTime: visitorInfo.time,
                    location: visitorInfo.location,
                    visitorUrl: visitorInfo.currentUrl,
                    isCurrent: (self.conversations().length == 0 ? true : false)
                }))
        };

        // Message from visitor
        $.connection.chatHub.client.sendMessageToOperator = function (name, message, time, visitorId) {

            var conversation = ko.utils.arrayFirst(self.conversations(), function (c) {
                return c.visitorId() == visitorId;
            });

            if (conversation) {
                conversation.messages.push(new Message({
                    author: name,
                    text: message,
                    time: moment(time).format('LT'),
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

function VisitorInfo(data) {
    this.sessionId = ko.observable(data.sessionId);
    this.name = ko.observable(data.name);
    this.startTime = ko.observable(data.startTime);
    this.location = ko.observable(data.location);
    this.visitorUrl = ko.observable(data.visitorUrl);
}

function Conversation(data) {
    var self = this;

    self.visitor = ko.observable(data.visitor);
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