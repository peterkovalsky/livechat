
function ChatWidgetViewModel(accountKey, currentPage) {

    var self = this;

    self.visitorName = ko.observable("").extend({ required: "" });
    self.visitorEmail = ko.observable("");
    self.conversationStarted = ko.observable(true);
    self.goneOffline = ko.observable(false);
    self.newMessage = ko.observable("");
    self.messages = ko.observableArray([]);
    self.operatorName = ko.observable("");

    self.showErrors = ko.observable(false);
    self.isFocus = ko.observable(true);

    var chatHubProxy = $.connection.chatHub;


    // init widget
    // -----------
    self.init = function () {

        // register SignalR callbacks
        self.registerClientSideFunctions();

        // connect to SignalR
        $.connection.hub.start().done(function () {

            var sessionId = $.cookie('kookaburra.visitor.sessionid');

            // if visitor session valid - resume chat
            if (sessionId) {
                chatHubProxy.server.checkVisitorSession(sessionId).done(function (conversationViewModel) {
                    if (conversationViewModel) {
                        self.resumeChat(conversationViewModel.conversation);
                    }
                });
            }       
        });

        self.isFocus(true);
    };


    // resume chat
    // -----------
    self.resumeChat = function (previousConversation)
    {
        self.messages(previousConversation);
        self.conversationStarted(true);
        self.addEnterPressEvent();        
    };


    // Sends message to operator on Enter Press
    // ----------------------------------------
    self.addEnterPressEvent = function () {        
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
    };


    // register SignalR callback functions
    // -----------------------------------
    self.registerClientSideFunctions = function () {
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
    };
   

    // starts new chat with operator
    // -----------------------------
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
                    self.addEnterPressEvent();             

                    $.cookie('kookaburra.visitor.sessionid', _sessionId);
                }
            });
        }
        else {
            self.showErrors(true);
            self.isFocus(true);
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
}

function Message(data) {
    this.author = ko.observable(data.author);
    this.text = ko.observable(data.text);
    this.time = ko.observable(data.time);
}