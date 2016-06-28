"use strict";
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
        this.conversations = [];
        var native = this.elementRef.nativeElement;
        this.operatorName = native.getAttribute("data-operator-name");
        this.chatHubProxy = jQuery.connection.chatHub;
        this.hub = jQuery.connection.hub;
    }
    ChatRoom.prototype.ngOnInit = function () {
        // Visitor sent message
        this.chatHubProxy.client.addNewMessageToPage = jQuery.proxy(function (name, message, time, sender, visitorConnectionId) {
            console.log(message);
            var jsTime = new Date();
            jsTime.setTime(time);
            var msg = { id: 1, author: name, text: message, sender: sender, time: jsTime };
            this.conversation.messages.push(msg);
        }, this);
        // Visitor initiated conversation
        this.chatHubProxy.client.clientConnected = jQuery.proxy(function (clientId, name, time, location, currentUrl) {
            this.updateConversations(clientId, name, time, location, currentUrl);
        }, this);
        // Operator starts the connection.
        this.hub.start().done(jQuery.proxy(function () {
            this.chatHubProxy.server.connectOperator().done(function () {
            });
        }, this));
    };
    // Send message to visitor
    ChatRoom.prototype.enterNewMessage = function (message, visitorConnectionId) {
        this.chatHubProxy.server.sendToVisitor(message, visitorConnectionId);
    };
    ChatRoom.prototype.updateConversations = function (clientId, name, time, location, currentUrl) {
        var jsTime = new Date();
        jsTime.setTime(time);
        var conversation = {
            visitorId: clientId,
            visitorName: name,
            conversationStartTime: jsTime,
            location: location,
            visitorUrl: currentUrl,
            messages: [],
            isCurrent: this.conversations.length == 0
        };
        this.conversations.push(conversation);
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
}());
exports.ChatRoom = ChatRoom;
//# sourceMappingURL=chat-room.js.map