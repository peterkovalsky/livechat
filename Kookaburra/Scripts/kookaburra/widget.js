ko.bindingHandlers.enterkey = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};

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
    //validate(target());

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
};

function Offline(accountKey) {
    var self = this;

    self.name = ko.observable("").extend({ required: "Please enter your name" });
    self.email = ko.observable("").extend({ required: "Please enter your email" });
    self.message = ko.observable("").extend({ required: "Please enter your message" });

    self.accountKey = accountKey;
    self.focusName = ko.observable(true);
}

function Message(data) {
    var self = this;

    self.author = ko.observable(data.author);
    self.text = ko.observable(data.text);
    self.sentBy = ko.observable(data.sentBy);
    self.time = ko.observable(moment(data.time).format('LT'));
}

function Visitor(accountKey) {
    var self = this;

    self.name = ko.observable("").extend({ required: "Please enter your name" });
    self.email = ko.observable("");
    self.url = (window.location != window.parent.location)
        ? document.referrer
        : document.location.href;

    self.accountKey = accountKey;
    self.focusName = ko.observable(true);
}

function WidgetViewModel(accountKey) {
    var self = this;

    self.visitor = ko.observable(new Visitor(accountKey));
    self.offline = ko.observable(new Offline(accountKey));

    self.operatorName = ko.observable("");
    self.messages = ko.observableArray([]);
    self.newMessage = ko.observable("");

    self.isStopChat = ko.observable(false);
    self.isMessageBoxFocus = ko.observable(true);

    self.view = ko.observable('');

    var chatHubProxy = $.connection.visitorHub;

    // init widget
    // -----------
    self.init = function () {

        // register SignalR callbacks
        self.registerClientSideFunctions();

        // connect to SignalR
        $.connection.hub.start().done(function () {

            $.connection.visitorHub.server.initWidget(accountKey).done(function (initResult) {

                if (initResult.step == 'Resume') {
                    // resume started chat
                    var conversationViewModel = initResult.resumedChat;

                    self.visitor().name(conversationViewModel.visitorName);
                    self.operatorName(conversationViewModel.operatorName);
                    self.messages(conversationViewModel.conversation);


                    self.view('Chat')

                    self.scrollDown();
                    self.isMessageBoxFocus(true);
                }
                else if (initResult.step == 'Introduction') {
                    // introduction                    
                    self.view('Intro');

                    if (initResult.visitorName) {
                        self.visitor().name(initResult.visitorName);
                    }
                    if (initResult.visitorEmail) {
                        self.visitor().email(initResult.visitorEmail);
                    }

                    self.visitor().focusName(true);
                }
                else {
                    // operator gone offline                    
                    self.view('Offline');

                    if (initResult.visitorName) {
                        self.offline().name(initResult.visitorName);
                    }
                    if (initResult.visitorEmail) {
                        self.offline().email(initResult.visitorEmail);
                    }

                    self.offline().focusName(true);
                }
            });
        });
    };

    self.startChat = function () {
        self.visitor().name.hasError(self.visitor().name() ? false : true);

        if (!self.visitor().name.hasError()) {
            var visitorView = ko.toJS(self.visitor());

            $.connection.visitorHub.server.startChat(visitorView).done(function (result) {

                if (result != null) {
                    $.cookie(result.cookieName, result.sessionId, { path: '/' });

                    if (result.operatorName) {
                        // load up previous messages
                        if (result.messages) {
                            self.messages(result.messages);
                        }

                        self.operatorName(result.operatorName);
                        self.view('Chat');
                        self.isMessageBoxFocus(true);
                    }
                    else {
                        self.view('GoneOffline')
                    }
                }
            });
        }
    };

    self.gotoOfflineForm = function () {
        self.offline().focusName(true);
        self.view('Offline')
    };

    self.sendOfflineMessage = function () {
        // trigger validation
        self.offline().name.hasError(self.offline().name() ? false : true);
        self.offline().email.hasError(self.offline().email() ? false : true);
        self.offline().message.hasError(self.offline().message() ? false : true);

        if (!self.offline().name.hasError() && !self.offline().email.hasError() && !self.offline().message.hasError()) {
            var offlineMsgView = ko.toJS(self.offline());

            $.connection.visitorHub.server.sendOfflineMessage(offlineMsgView);
            self.view('ThankYou');
        }
    };

    // Visitor wants to stop conversation
    // ------------------
    self.closeChat = function () {
        $.connection.visitorHub.server.finishChattingWithOperator(accountKey);
        self.view('ThankYouEndChat');
    }

    // Sends message to operator on Enter Press
    // ----------------------------------------  
    self.sendMessage = function () {
        $.connection.visitorHub.server.sendToOperator(self.newMessage());

        self.newMessage('');
        self.scrollDown();
    };

    self.scrollDown = function () {
        $('.chat-window').animate({ scrollTop: $('#messages').prop('scrollHeight') }, "slow");
    }

    // register SignalR callback functions
    // -----------------------------------
    self.registerClientSideFunctions = function () {

        // Visitor received message from operator
        var messageToVisitorEventHandler = function (message) {
            self.messages.push(new Message(message));

            self.scrollDown();
        };

        $.connection.visitorHub.client.sendMessageToVisitor = messageToVisitorEventHandler;
        $.connection.chatHub.client.sendMessageToVisitor = messageToVisitorEventHandler;

        var visitorDisconnectedEventHandler = function (result) {
            //$.connection.hub.stop();

            var disconnectMessage = 'Chat has been stopped.';
            if (result.disconnectedBy == 'Operator') {
                disconnectMessage = 'Operator ended chat with you.';
            }
            else if (result.disconnectedBy == 'Visitor') {
                disconnectMessage = 'You ended chat.';
                self.view('ThankYou')
            }
            else if (result.disconnectedBy == 'System') {
                disconnectMessage = 'Chat was closed due to inactivity.';
            }

            self.messages.push(new Message({
                text: disconnectMessage,
                sentBy: 'system',
                time: result.time
            }));

            self.scrollDown();
        };

        $.connection.visitorHub.client.visitorDisconnected = visitorDisconnectedEventHandler;
        $.connection.chatHub.client.visitorDisconnected = visitorDisconnectedEventHandler;
    };
}