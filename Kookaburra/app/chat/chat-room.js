var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('angular2/core');
var chat_window_component_1 = require('./chat-window.component');
var ChatRoom = (function () {
    function ChatRoom(elementRef) {
        this.elementRef = elementRef;
        var native = this.elementRef.nativeElement;
        this.accountId = native.getAttribute("data-account-id");
        this.operatorName = native.getAttribute("data-operator-name");
    }
    ChatRoom.prototype.ngOnInit = function () {
        // Start the connection.
        this.hub.start().done(jQuery.proxy(function () {
            this.chatHubProxy.server.connectOperator(this.operatorName, this.accountId).done(function () { });
        }, this));
        this.chatHubProxy.client.clientConnected = jQuery.proxy(function (clientId, name, time, location, currentUrl) {
            var jsTime = new Date();
            jsTime.setTime(time);
            var conversation = {
                visitorId: clientId,
                visitorName: name,
                conversationStartTime: jsTime,
                location: location,
                visitorUrl: currentUrl,
                messages: []
            };
            this.conversations.push(conversation);
        }, this);
    };
    ChatRoom = __decorate([
        core_1.Component({
            selector: 'chat-room',
            templateUrl: '/app/chat/chat-room.html',
            directives: [chat_window_component_1.ChatWindowComponent]
        }), 
        __metadata('design:paramtypes', [core_1.ElementRef])
    ], ChatRoom);
    return ChatRoom;
})();
exports.ChatRoom = ChatRoom;
//# sourceMappingURL=chat-room.js.map