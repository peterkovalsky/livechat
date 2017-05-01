function Message(data) {
    this.author = ko.observable(data.author);
    this.text = ko.observable(data.text);
    this.sentBy = ko.observable(data.sentBy);
    this.time = ko.observable(moment(data.time).format('LT'));
}

function WidgetViewModel(accountKey) {
    var self = this;

    self.visitorName = ko.observable("");
    self.visitorEmail = ko.observable("");
         
    self.operatorName = ko.observable("");
    self.messages = ko.observableArray([]);
    self.newMessage = ko.observable("");

    self.isStopChat = ko.observable(false);
    self.isMessageBoxFocus = ko.observable(true);

    self.view = ko.observable('');

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
                    self.view('Intro')
                }
                else {
                    // operator gone offline                
                    self.view('GoneOffline')
                }
            });
        });
    };

    self.startChat = function () {
        var url = (window.location != window.parent.location)
            ? document.referrer
            : document.location.href;


    };

    self.leaveMessage = function () {
        self.view('Offline')
    };

    // resume chat
    // -----------
    self.resumeChat = function (previousConversation) {
        self.messages(previousConversation);
        self.conversationStarted(true);
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
    self.addEnterPressEvent = function () {
        $(document).keypress(function (e) {
            if (e.which == 13) {

                //self.messages.push(new Message({
                //    author: self.visitorName(),
                //    text: self.newMessage(),
                //    time: moment().format('LT'),
                //    sentBy: 'visitor'
                //}));

                chatHubProxy.server.sendToOperator(self.newMessage());

                self.newMessage('');
                self.scrollDown();

                e.preventDefault();
            }
        });
    };

    self.scrollDown = function () {
        $('#conversation').animate({ scrollTop: $('#messages').prop('scrollHeight') }, "slow");
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


    // On enter press event handler
    // ----------------------------
    self.onEnterStartChat = function (data, event) {
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

    self.init = function () {
        $('#name').focus();
    };
}