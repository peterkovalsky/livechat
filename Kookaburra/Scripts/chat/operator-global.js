﻿function OperatorGlobalViewModel() {
 
    var self = this; 
    self.activeChats = ko.observableArray();
   

    self.init = function () {
        self.clientCallbackFunctions();

        // Start the connection.
        $.connection.hub.start().done(function () {
            $.connection.chatHub.server.connectOperator().done(function (result) {
                //self.activeChats(result.currentChats);

                self.activeChats.push(new Chat(
                {
                    visitorSessionId: 'asdfsf'
                }));
            });
        });

        // In case operator is disconnected from the server
        $.connection.hub.disconnected(function () {
            alert('You were disconnected from the messaging server. Please refresh the page.');
        });
    }

    self.clientCallbackFunctions = function () {
        // Visitor CONNECTED 
        $.connection.chatHub.client.visitorConnected = function (visitorInfo) {

            

            var chat = ko.utils.arrayFirst(self.activeChats(), function (c) {
                return c.visitorSessionId == visitorInfo.sessionId;
            });

            if (chat == null) {
                // increment active chats
                self.activeChats.push(new Chat(
                {
                    visitorSessionId: visitorInfo.sessionId
                }));
            }
        };

        // Visitor DISCONNECTED 
        $.connection.chatHub.client.visitorDisconnected = function (visitorSessionId) {

            var chat = ko.utils.arrayFirst(self.activeChats(), function (c) {
                return c.visitorSessionId == visitorSessionId;
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