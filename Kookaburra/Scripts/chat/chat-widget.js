function ChatWidgetViewModel(accountKey, currentPage) {

    var self = this;

    self.visitorName = ko.observable("");
    self.visitorEmail = ko.observable("");
    self.conversationStarted = ko.observable(false);
    self.goneOffline = ko.observable(false);
    self.newMessage = ko.observable("");
    self.messages = ko.observableArray([]);
    self.operatorId = null;
    self.operatorName = ko.observable("");

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
            author: 'System',
            text: 'You were disconnected by the operator',
            time: moment().format('LT')
        }));
    };

    self.validate = function () {
        if (self.visitorName() == '')
        {

        }
    };

    self.startConversation = function () {
              

        self.goneOffline(false);

        // Start the connection.
        $.connection.hub.start().done(function () {
            chatHubProxy.server.connectVisitor(self.visitorName(), self.visitorEmail(), currentPage, accountKey).done(function (_operatorId) {
                if (_operatorId == null) {
                    self.goneOffline(true) 
                }
                else {
                    // show chat window
                    self.conversationStarted(true);
                    self.operatorId = _operatorId;             

                    // SEND MESSAGE ON ENTER PRESS
                    $(document).keypress(function (e) {
                        if (e.which == 13) {

                            self.messages.push(new Message({
                                author: self.visitorName(),
                                text: self.newMessage(),
                                time: moment().format('LT')
                            }));

                            chatHubProxy.server.sendToOperator(self.visitorName(), self.newMessage(), _operatorId);                         
                            
                            self.newMessage('');

                            e.preventDefault();
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