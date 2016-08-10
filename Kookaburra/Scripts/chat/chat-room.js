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
    self.switchChat = function (conversation) {

        for (var i = 0; i < self.conversations().length; i++) {
            self.conversations()[i].isCurrent(false);
        }

        conversation.isCurrent(true);
    };

    var chatHubProxy = $.connection.chatHub;

    //------------------- INCOMMING MESSAGE --------------------
    chatHubProxy.client.sendMessageToOperator = function (name, message, time, visitorId) {       

        var conversation = ko.utils.arrayFirst(self.conversations(), function (c) {
            return c.visitorId() == visitorId;
        });

        if (conversation)
        {
            conversation.messages.push(new Message({
                author: name,
                text: message,
                time: moment(time).format('LT')
            }));
        }       
    };

    //------------------- Visitor CONNECTED --------------------
    chatHubProxy.client.clientConnected = function (clientId, name, time, location, currentUrl) {

        self.conversations.push(new Conversation(
            {
                visitorId: clientId,
                visitorName: name,
                startTime: time,
                location: location,
                visitorUrl: currentUrl,
                isCurrent: (self.conversations().length == 0 ? true : false)
            }))
    };

    //------------------- Visitor DISCONNECTED --------------------
    chatHubProxy.client.clientDisconnected = function (clientId, name, time) {

    };

    // ---------------------- OPERATOR DISCONNECTED -------------------
    $.connection.hub.disconnected(function () {
        alert('You were disconnected from the messaging server. Please refresh the page.');
    });

    // Start the connection.
    $.connection.hub.start().done(function () {
        chatHubProxy.server.connectOperator().done(function () { });

        // SEND MESSAGE ON ENTER PRESS
        $(document).keypress(function (e) {
            if (e.which == 13) {
                self.currentChat().messages.push(new Message({
                    author: self.operatorName,
                    text: self.newText(),
                    time: moment().format('LT')
                }));
                chatHubProxy.server.sendToVisitor(self.operatorName, self.newText(), self.currentChat().visitorId());

                self.newText(''); // clear input area

                e.preventDefault();
            }
        });

        // DISCONNECT A Visitor
        $("body").on("click", "button.disconnect", function (e) {
            var _clientId = $(this).closest('.chat-client[data-client-id]').attr('data-client-id');
            var name = $(this).closest('.client-info .client-name').val();

            if (confirm("Are you sure you want to disconnect " + name + "?")) {
                chatHubProxy.server.disconnectClient(_clientId);
            }
        });
    });
}

function Message(data) {
    this.id = ko.observable(data.id);
    this.author = ko.observable(data.author);
    this.sender = ko.observable(data.sender);
    this.text = ko.observable(data.text);
    this.time = ko.observable(data.time);
}

function Conversation(data) {
    var self = this;

    self.visitorId = ko.observable(data.visitorId);
    self.visitorName = ko.observable(data.visitorName);
    self.startTime = ko.observable(data.startTime);
    self.location = ko.observable(data.location);
    self.visitorUrl = ko.observable(data.visitorUrl);
    self.messages = ko.observableArray([]);
    self.isCurrent = ko.observable(data.isCurrent);
    self.startTimeFormatted = ko.computed(function () {
        return moment(self.startTime()).calendar();
    });
    self.visitorNameFormatted = ko.computed(function () {
        return self.visitorName().toUpperCase();
    });
}