function ChatWidgetViewModel(accountKey, currentPage) {

    var self = this;

    self.visitorName = ko.observable("");
    self.visitorEmail = ko.observable("");
    self.conversationStarted = ko.observable(false);
    self.goneOffline = ko.observable(false);
    self.newMessage = ko.observable("");
    self.messages = ko.observableArray([]);
    self.operatorId = null;

    self.startConversation = function () {
        
        var chatHubProxy = $.connection.chatHub;

        // Create a function that the hub can call back to display messages.
        chatHubProxy.client.sendMessageToVisitor = function (name, message, time) {
            self.messages.push(new Message({
                author: name,
                text: message,
                time: moment(time).format('LT')
            }));
        };

        chatHubProxy.client.orderToDisconnect = function () {
            $.connection.hub.stop();

            self.messages.push(new Message({
                author: name,
                text: 'You were disconnected by the operator',
                time: moment(time).format('LT')
            }));

            $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: You were disconnected by the operator       ' + htmlEncode(time) + '</li>');
        };

        // Start the connection.
        $.connection.hub.start().done(function () {
            chatHubProxy.server.connectClient(self.visitorName, accountKey, currentPage).done(function (_operatorId) {
                if (_operatorId == null) {
                    self.goneOffline(true) 
                }
                else {
                    // show chat window
                    self.conversationStarted(true);

                    self.operatorId = _operatorId;

                    // Get the user name and store it to prepend to messages.
                    $('#displayname').val(clientName);
                    // Set initial focus to message input box.
                    $('#message').focus();

                    // SEND MESSAGE ON ENTER PRESS
                    $(document).keypress(function (e) {
                        if (e.which == 13) {
                            var message = $('#message').val();
                            // Call the Send method on the hub.
                            chatHubProxy.server.sendToOperator($('#displayname').val(), $('#message').val(), _operatorId);

                            e.preventDefault();
                            // Clear text box and reset focus for next comment.
                            $('#message').val('').focus();
                            return false;
                        }
                    });
                }
            });
        });
    };
}

function Message(data) {
    this.author = ko.observable(data.author);
    this.text = ko.observable(data.text);
    this.time = ko.observable(data.time);
}