function OperatorGlobalViewModel() {
 
    var self = this; 
    self.activeChats = ko.observableArray([]);
    self.unreadMessages = ko.observable(0);

    self.init = function () {
        self.registerCallbackFunctions();

        // Start the connection.
        $.connection.hub.start().done(function () {

            // trigger event idicating that SignalR has started
            var evt = $.Event('signalr.start');           
            $(window).trigger(evt);

            $.connection.chatHub.server.connectOperator().done(function (result) {
                // resume
                if (result.currentChats && result.currentChats.length > 0) {
                    $.each(result.currentChats, function (index, item) {
                        self.activeChats.push(new Chat(item));
                    });
                }

                self.unreadMessages(result.unreadMessages);
            });
        });

        // In case operator is disconnected from the server
        $.connection.hub.disconnected(function () {
            //alert('You were disconnected from the messaging server. Please refresh the page.');
            console.log('You were disconnected from the messaging server. Please refresh the page.');
        });

 
        postbox.subscribe(function (newValue) {
            self.unreadMessages(self.unreadMessages() - 1);
        }, self, "decrementUnreadMessages");
    }

    self.registerCallbackFunctions = function () {
        // Visitor CONNECTED 
        $.connection.visitorHub.client.visitorConnectedGlobal = function (sessionId) {
           
            var chat = ko.utils.arrayFirst(self.activeChats(), function (c) {
                return c.visitorSessionId() == sessionId;
            });

            if (chat == null) {
                // increment active chats
                self.activeChats.push(new Chat(
                {
                    visitorSessionId: sessionId
                }));
            }
        };

        // Visitor DISCONNECTED 
        var visitorDisconnectedGlobalEventHandler = function (visitorSessionId) {

            var chat = ko.utils.arrayFirst(self.activeChats(), function (c) {
                return c.visitorSessionId() == visitorSessionId;
            });

            if (chat) {
                // remove chat
                self.activeChats.remove(chat);
            }
        };

        $.connection.visitorHub.client.visitorDisconnectedGlobal = visitorDisconnectedGlobalEventHandler;
        $.connection.chatHub.client.visitorDisconnectedGlobal = visitorDisconnectedGlobalEventHandler;
    }
}

function Chat(data) {
    this.visitorSessionId = ko.observable(data.visitorSessionId);
}