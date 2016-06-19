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
var conversation_model_1 = require('../models/conversation.model');
var ChatWindowComponent = (function () {
    function ChatWindowComponent(elementRef) {
        this.elementRef = elementRef;
        this.onEnterMessage = new core_1.EventEmitter();
        this.elementRef = elementRef;
        //this.messages = new Array<Message>();
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
    };
    ChatWindowComponent.prototype.onEnter = function (event) {
        this.onEnterMessage.emit(this.currentMessage);
    };
    __decorate([
        core_1.Input(), 
        __metadata('design:type', conversation_model_1.ConversationModel)
    ], ChatWindowComponent.prototype, "conversation", void 0);
    __decorate([
        core_1.Output(), 
        __metadata('design:type', core_1.EventEmitter)
    ], ChatWindowComponent.prototype, "onEnterMessage", void 0);
    ChatWindowComponent = __decorate([
        core_1.Component({
            selector: 'chat-window',
            templateUrl: '/app/chat/chat-window.component.html'
        }), 
        __metadata('design:paramtypes', [core_1.ElementRef])
    ], ChatWindowComponent);
    return ChatWindowComponent;
})();
exports.ChatWindowComponent = ChatWindowComponent;
//# sourceMappingURL=chat-window.component.js.map