function Conversation(data) {
    var self = this;

    self.id = data.id;
    self.visitorName = data.visitorName;
    self.operatorName = data.operatorName;
    self.text = data.text;
    self.totalMessages = data.totalMessages;
    sef.time = data.time;
}

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

function ChatHistoryViewModel(initialData) {
    var self = this;

    self.conversations = ko.observableArray([]);
    self.totalMessages = ko.observable(data.totalMessages);
    self.currentPage = ko.observable(1);
    self.searchTerm = ko.observable('');
    self.searchTermLabel = ko.observable('');
    self.searching = ko.observable(false);
    self.filter = ko.observable('All');

    self.addConversations = function (newConversations) {
        $.each(newConversations, function (index, item) {
            self.conversations.push(new Conversation(item));
        });
    };

    self.search = function () {
        if (self.searchTerm() && self.searchTerm().length >= 3) {

            $.get("/api/history/search/" + self.searchTerm() + '/' + + self.currentPage())
                .done(function (data) {
                    self.searchTermLabel(self.searchTerm());
                    self.searching(true);
                    self.conversations([]);
                    self.currentPage(1);

                    self.addConversations(data.offlineMessages);
                    self.totalMessages(data.totalMessages);
                });
        }
    };

    self.showMore = function () {

        self.currentPage(self.currentPage() + 1);

        if (self.searching()) {
            $.get("/api/history/search/" + self.searchTerm() + '/' + self.currentPage())
                .done(function (data) {
                    self.addConversations(data.offlineMessages);
                });
        }
        else {
            $.get("/api/history/" + self.filter() + "/" + self.currentPage())
                .done(function (data) {
                    self.addConversations(data.offlineMessages);
                });
        }
    };

    // init
    self.addConversations(initialData.conversations);
}