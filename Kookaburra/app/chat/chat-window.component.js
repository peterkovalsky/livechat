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
var ChatWindowComponent = (function () {
    function ChatWindowComponent(elementRef) {
        this.elementRef = elementRef;
        this.messages = new Array();
        this.chatHubProxy = jQuery.connection.chatHub;
        this.hub = jQuery.connection.hub;
        // test
        //var date = new Date();
        //this.clientName = "Peter Kovalskyy";
        //this.conversationStartTime = date;
        //this.messages =
        //[
        //    { id: 1, author: "Slava", text: "Where are you?", sender: "OPERATOR", time: date },
        //    { id: 2, author: "Slava", text: "Come home", sender: "OPERATOR", time: date },
        //    { id: 3, author: "Peter", text: "I'm at work", sender: "CLIENT", time: date },
        //    { id: 4, author: "Slava", text: "OK", sender: "OPERATOR", time: date },
        //];
    }
    ChatWindowComponent.prototype.ngOnInit = function () {
        this.chatHubProxy.client.addNewMessageToPage = jQuery.proxy(function (name, message, time, sender, clientId) {
            var jsTime = new Date();
            jsTime.setTime(time);
            var msg = { id: 1, author: name, text: message, sender: sender, time: jsTime };
            this.messages.push(msg);
        }, this);
        this.chatHubProxy.client.clientConnected = jQuery.proxy(function (clientId, name, time, location, currentUrl) {
            var jsTime = new Date();
            jsTime.setTime(time);
            this.clientName = name;
            this.conversationStartTime = jsTime;
        }, this);
        // Start the connection.
        this.hub.start().done(jQuery.proxy(function () {
            this.chatHubProxy.server.connectOperator('John Dou', '086FBDC2-14F3-4F68-B3C6-9EA42D257061').done(function () { });
        }, this));
    };
    ChatWindowComponent.prototype.onEnter = function (event) {
        this.chatHubProxy.server.sendToClient('John Dou', this.currentMessage, this.clientId);
        this.currentMessage = '';
    };
    ChatWindowComponent = __decorate([
        core_1.Component({
            selector: 'chat-window',
            templateUrl: '/app/chat/chat-window.component.html'
        }), 
        __metadata('design:paramtypes', [core_1.ElementRef])
    ], ChatWindowComponent);
    return ChatWindowComponent;
}());
exports.ChatWindowComponent = ChatWindowComponent;
var Message = (function () {
    function Message() {
    }
    return Message;
}());
//# sourceMappingURL=chat-window.component.js.map