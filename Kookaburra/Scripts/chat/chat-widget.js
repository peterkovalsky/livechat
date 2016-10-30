ko.extenders.required = function(target, overrideMessage) {
    //add some sub-observables to our observable
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();
 
    //define a function to do validation
    function validate(newValue) {
       target.hasError(newValue ? false : true);
       target.validationMessage(newValue ? "" : overrideMessage || "This field is required");
    }
 
    //initial validation
    validate(target());
 
    //validate whenever the value changes
    target.subscribe(validate);
 
    //return the original observable
    return target;
};

function ChatWidgetViewModel(accountKey, currentPage) {

    var self = this;

    self.visitorName = ko.observable("").extend({ required: "" });
    self.visitorEmail = ko.observable("");
    self.conversationStarted = ko.observable(false);
    self.goneOffline = ko.observable(false);
    self.newMessage = ko.observable("");
    self.messages = ko.observableArray([]);
    self.operatorId = null;
    self.operatorName = ko.observable("");
    self.showErrors = ko.observable(false);

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

    self.StartChatOnEnter = function(data, event) {
        try {
            if (event.which == 13) {
                self.startConversation();
                return false;
            }
            return true;
        }
        catch (e)
        { }
    }

    self.startConversation = function () {
              
        self.showErrors(true);

        if (!self.visitorName.hasError()) {
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
        }
    };
}

function Message(data) {
    this.author = ko.observable(data.author);
    this.text = ko.observable(data.text);
    this.time = ko.observable(data.time);
}