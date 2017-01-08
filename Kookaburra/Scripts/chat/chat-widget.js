ko.extenders.required = function (target, overrideMessage) {
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
    self.operatorName = ko.observable("");

    self.showErrors = ko.observable(false);
    self.isFocus = ko.observable(true);

    var chatHubProxy = $.connection.chatHub;

    self.init = function () {
        $.connection.hub.start().done(function () {

            var sessionId = $.cookie('kookaburra.visitor.sessionid');

            if (sessionId) {
                chatHubProxy.server.checkVisitorSession(sessionId).done(function (conversationViewModel) {
                    self.messages(conversationViewModel.conversation);
                    self.conversationStarted(true);
                });
            }

            // SEND MESSAGE ON ENTER PRESS
            $(document).keypress(function (e) {
                if (e.which == 13) {

                    self.messages.push(new Message({
                        author: self.visitorName(),
                        text: self.newMessage(),
                        time: moment().format('LT')
                    }));

                    chatHubProxy.server.sendToOperator(self.visitorName(), self.newMessage());

                    self.newMessage('');

                    e.preventDefault();
                }
            });
        });

        self.isFocus(true);
    };

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

    self.StartChatOnEnter = function (data, event) {
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

        if (!self.visitorName.hasError()) {
            self.goneOffline(false);

            // Start the connection.
            chatHubProxy.server.connectVisitor(self.visitorName(), self.visitorEmail(), currentPage.href, accountKey).done(function (_sessionId) {
                if (_sessionId == null) {
                    self.goneOffline(true)
                }
                else {
                    // show chat window
                    self.conversationStarted(true);
                          
                    $.cookie('kookaburra.visitor.sessionid', _sessionId);
                }
            });
        }
        else {
            self.showErrors(true);
            self.isFocus(true);
        }
    };
}

function Message(data) {
    this.author = ko.observable(data.author);
    this.text = ko.observable(data.text);
    this.time = ko.observable(data.time);
}