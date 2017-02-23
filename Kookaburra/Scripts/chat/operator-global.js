﻿function OperatorGlobalViewModel() {
 
    var self = this; 
    self.activeChats = ko.observableArray();
   

    self.init = function () {
        self.registerCallbackFunctions();

        // Start the connection.
        $.connection.hub.start().done(function () {

            var evt = $.Event('signalr.start');           
            $(window).trigger(evt);

            $.connection.chatHub.server.connectOperator().done(function (result) {
                self.activeChats(result.currentChats);          
            });
        });

        // In case operator is disconnected from the server
        $.connection.hub.disconnected(function () {
            alert('You were disconnected from the messaging server. Please refresh the page.');
        });
    }

    self.registerCallbackFunctions = function () {
        // Visitor CONNECTED 
        $.connection.chatHub.client.visitorConnectedGlobal = function (sessionId) {
           
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
        $.connection.chatHub.client.visitorDisconnectedGlobal = function (visitorSessionId) {

            var chat = ko.utils.arrayFirst(self.activeChats(), function (c) {
                return c.visitorSessionId() == visitorSessionId;
            });

            if (chat) {
                // remove chat
                self.activeChats.remove(chat);
            }
        };
    }
}

function Chat(data) {
    this.visitorSessionId = ko.observable(data.visitorSessionId);
}