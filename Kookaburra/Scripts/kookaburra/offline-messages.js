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

function MessagesViewModel(data) {
    var self = this;

    self.messages = ko.observableArray([]);
    self.totalMessages = ko.observable(data.totalMessages);
    self.pageSize = ko.observable(data.pageSize);
    self.currentPage = ko.observable(1);   


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

    self.sendReply = function (message) {

    };

    self.showMore = function () {

        self.currentPage(self.currentPage() + 1);

        $.get("/api/messages/" + self.currentPage())
            .done(function (data) {
                self.addMessages(data);
            });
    };

    // Init
    self.addMessages(data.offlineMessages);
}