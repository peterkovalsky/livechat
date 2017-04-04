function Message(data) {
    var self = this;

    self.id = ko.observable(data.id);
    self.message = ko.observable(data.message);
    self.time = ko.observable(moment(data.time).format('lll')); // Mar 27, 2017 4:29 PM
    self.name = ko.observable(data.name);
    self.email = ko.observable(data.email);
    self.country = ko.observable(data.country);
    self.region = ko.observable(data.region);
    self.isRead = ko.observable(data.isRead);
    self.isCurrent = ko.observable(false);
    self.reply = ko.observable('');
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

function MessagesViewModel(data) {
    var self = this;

    self.messages = ko.observableArray([]);
    self.totalMessages = ko.observable(data.totalMessages);
    self.totalInitialMessages = ko.observable(data.totalMessages);
    self.currentPage = ko.observable(1);
    self.searchTerm = ko.observable('');
    self.searchTermLabel = ko.observable('');
    self.searching = ko.observable(false);
    self.filter = ko.observable('All');


    self.init = function () {
        self.addMessages(data.offlineMessages);
    };

    self.currentMessage = ko.computed(function () {
        var result = $.grep(self.messages(), function (e) { return e.isCurrent() == true; });
        if (result && result.length > 0) {
            return result[0];
        } else {
            return null;
        }
    });

    self.addMessages = function (newMessages) {
        $.each(newMessages, function (index, item) {
            self.messages.push(new Message(item));
        });
    };

    self.openMessage = function (message) {

        if (self.messages().length > 0) {
            for (var i = 0; i < self.messages().length; i++) {
                self.messages()[i].isCurrent(false);
            }

            if (message) {
                message.isCurrent(true);
                self.markAsRead(message);                
            }
        }

        var html = $('#message-details-template').html();

        $.slidePanel.show({
            content: html
        }, {
                mouseDrag: false,
                touchDrag: false,
                pointerDrag: false,
                closeSelector: '.slidePanel-close',
                direction: 'right'
            });
    };

    self.search = function () {
        if (self.searchTerm() && self.searchTerm().length >= 3) {

            $.get("/api/messages/search/" + self.searchTerm() + '/' + + self.currentPage())
                .done(function (data) {
                    self.searchTermLabel(self.searchTerm());
                    self.searching(true);
                    self.messages([]);
                    self.currentPage(1);

                    self.addMessages(data.offlineMessages);
                    self.totalMessages(data.totalMessages);
                });
        }
    };

    self.showMore = function () {

        self.currentPage(self.currentPage() + 1);

        if (self.searching()) {
            $.get("/api/messages/search/" + self.searchTerm() + '/' + self.currentPage())
                .done(function (data) {
                    self.addMessages(data.offlineMessages);
                });
        }
        else {
            $.get("/api/messages/" + self.filter() + "/" + self.currentPage())
                .done(function (data) {
                    self.addMessages(data.offlineMessages);
                });
        }
    };

    self.markAsRead = function (message) {

        if (!message.isRead()) {
            postbox.notifySubscribers(0, "decrementUnreadMessages");

            message.isRead(true);

            $.ajax({
                method: "PATCH",
                url: "api/messages/mark-read/" + message.id()
            })
            .done(function (msg) {

            });
        }
    };

    self.backToAll = function () {
        self.messages([]);
        self.totalMessages(data.totalMessages);      
        self.currentPage(1);
        self.searchTerm('');
        self.searchTermLabel('');
        self.searching(false);
        self.filter('All');

        self.init();
    };

    self.filterAll = function () {
        self.filterOut('All');
    };

    self.filterByDay = function () {
        self.filterOut('Day');
    };

    self.filterByWeek = function () {
        self.filterOut('Week');
    };

    self.filterByFortnight = function () {
        self.filterOut('Fortnight');
    };

    self.filterByMonth = function () {
        self.filterOut('Month');
    };

    self.filterByYear = function () {
        self.filterOut('Year');
    };

    self.filterOut = function (filter) {

        self.filter(filter);
        self.currentPage(1);

        $.get("/api/messages/" + self.filter() + "/" + self.currentPage())
            .done(function (data) {
                self.messages([]);
                self.addMessages(data.offlineMessages);
                self.totalMessages(data.totalMessages);
            });
    };

    self.delete = function (id) {

        var message = ko.utils.arrayFirst(self.messages(), function (item) {
            return item.id() == id;
        });

        // confirm dialog
        alertify.confirm("Are you sure you want to delete message from " + message.name() + "?", function () {            

            self.messages.remove(message);
            self.totalMessages(self.totalMessages()-1);
            $.slidePanel.hide();

            $.ajax({
                method: "DELETE",
                url: "api/messages/" + id
            })
            .done(function (msg) {

            });

        }, function () {
            // user clicked "cancel"
        });
    };

    self.init();
}