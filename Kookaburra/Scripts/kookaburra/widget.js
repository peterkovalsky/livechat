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
   
    self.name = '';
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

    var SESSION_ID_COOKIE = "kookaburra.visitor.sessionid";

    var chatHubProxy = $.connection.chatHub;

    // init widget
    // -----------
    self.init = function () {

        // register SignalR callbacks
        self.registerClientSideFunctions();

        // connect to SignalR
        $.connection.hub.start().done(function () {

            chatHubProxy.server.initWidget(accountKey).done(function (initResult) {

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
        chatHubProxy.server.startChat(self.visitor()).done(function (result) {

            if (result != null)
            {
                $.cookie(SESSION_ID_COOKIE, result.sessionId, { path: '/' });

                self.operatorName(result.operatorName);
                self.isMessageBoxFocus(true);
                self.view('Chat')
            }
            else
            {
                self.view('GoneOffline')
            }            
        });
    };

    self.gotoOfflineForm = function () {
        self.offline().focusName(true);
        self.view('Offline')
    };

    self.sendOfflineMessage = function () {
        chatHubProxy.server.sendOfflineMessage(self.offline()).done(function () {
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
        chatHubProxy.server.finishChattingWithOperator();
    }

    // Sends message to operator on Enter Press
    // ----------------------------------------  
    self.sendMessage = function () {
        chatHubProxy.server.sendToOperator(self.newMessage());

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
        chatHubProxy.client.sendMessageToVisitor = function (message) {
            self.messages.push(new Message(message));

            self.scrollDown();
        };

        chatHubProxy.client.visitorDisconnectedByOperator = function (result) {
            $.connection.hub.stop();

            self.messages.push(new Message({
                text: 'You were disconnected by the operator',
                sentBy: 'system',
                time: result.time
            }));

            self.scrollDown();
        };

        chatHubProxy.client.visitorDisconnectedByVisitor = function () {
            $.connection.hub.stop();

            window.location = "/widget/stop";
        }
    }; 
}