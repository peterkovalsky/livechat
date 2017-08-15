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

    self.name = '';
    self.email = '';
    self.message = ko.observable("");

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

    self.name = ko.observable('').extend({ required: "Please enter your name" });
    self.email = '';
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

                    self.visitorName(conversationViewModel.visitorName);
                    self.operatorName(conversationViewModel.operatorName);
                    self.resumeChat(conversationViewModel.conversation);

                    self.isMessageBoxFocus(true);

                    self.view('Chat')
                }
                else if (initResult.step == 'Introduction') {
                    // introduction                    
                    self.view('Intro');
                    self.visitor().focusName(true);
                }
                else {
                    // operator gone offline                    
                    self.view('Offline');
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
            });
        }
    };

    self.gotoOfflineForm = function () {
        self.offline().focusName(true);
        self.view('Offline')
    };

    self.sendOfflineMessage = function () {
        $.connection.visitorHub.server.sendOfflineMessage(self.offline()).done(function () {
            self.view('ThankYou')
        });
    };

    // resume chat
    // -----------
    self.resumeChat = function (previousConversation) {
        self.messages(previousConversation);

        self.addEnterPressEvent();
        self.scrollDown();
    };

    // Visitor wants to stop conversation
    // ------------------
    self.closeChat = function () {
        $.connection.visitorHub.server.finishChattingWithOperator();
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