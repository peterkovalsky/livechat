function DashboardViewModel() {
    var self = this;

    self.totalCurrentChats = ko.observable(0);
    self.newOfflineMessages = ko.observable(0);
    self.currentChats = ko.observableArray([]);

    self.init = function() {
        self.registerCallbackFunctions();

        $(window).on('signalr.start', function (e) {
            self.loadDashboard();
        }); 
    }

    // Load dashboard
    // -------------------------
    self.loadDashboard = function () {
        $.connection.chatHub.server.loadDashboard().done(function (result) {
            if (result) {

                if (result.currentChats && result.currentChats.length > 0)
                $.each(result.currentChats, function (index, item) {
                    self.currentChats.push(new LiveChat(item));
                });

                self.totalCurrentChats(result.totalCurrentChats);
                self.newOfflineMessages(result.newOfflineMessages);
            }
        });
    };

    // Register SignalR callbacks
    // -----------------------------
    self.registerCallbackFunctions = function () {

    }
}

function LiveChat(data) {
    var self = this;

    self.chatId = ko.observable(data.chatId);
    self.countryCode = ko.observable(data.countryCode);
    self.country = ko.observable(data.country);
    self.visitorName = ko.observable(data.visitorName);
    self.timeStarted = ko.observable(data.timeStarted);
    self.page = ko.observable(data.page);

    var now = ko.observable(new Date());
    setInterval(function () { now(new Date()); }, 60 * 1000);

    self.startedChatTime = ko.computed(function () {
        return moment(self.timeStarted()).startOf('minute').from(now());
    });
}