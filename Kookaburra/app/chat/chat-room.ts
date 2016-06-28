import {Component, OnInit, ElementRef} from 'angular2/core';
import {ChatWindowComponent} from './chat-window.component';
import {ConversationModel} from '../models/conversation.model';
import {MessageModel} from '../models/message.model';

declare var jQuery: any;

@Component({ 
    selector: 'chat-room',
    templateUrl: '/app/chat/chat-room.html',
    directives: [ChatWindowComponent]
})

export class ChatRoom implements OnInit {

    operatorName: string;
    conversations: ConversationModel[] = [];

    chatHubProxy: any;
    hub: any;

    constructor(private elementRef: ElementRef) {
        var native = this.elementRef.nativeElement;
        this.operatorName = native.getAttribute("data-operator-name");

        this.chatHubProxy = jQuery.connection.chatHub;
        this.hub = jQuery.connection.hub;
    }

    ngOnInit() {

        // Visitor sent message
        this.chatHubProxy.client.addNewMessageToPage = jQuery.proxy(function (name, message, time, sender, visitorConnectionId) {
            console.log(message);

            var jsTime = new Date();
            jsTime.setTime(time);

            var msg: MessageModel = { id: 1, author: name, text: message, sender: sender, time: jsTime };
            this.conversation.messages.push(msg);

        }, this);

        // Visitor initiated conversation
        this.chatHubProxy.client.clientConnected = jQuery.proxy(function (clientId, name, time, location, currentUrl) {         

            this.updateConversations(clientId, name, time, location, currentUrl);

        }, this);

        // Operator starts the connection.
        this.hub.start().done(jQuery.proxy(function () {
            this.chatHubProxy.server.connectOperator().done(function () {              
            })
        }, this));
    }

    // Send message to visitor
    enterNewMessage(message: string, visitorConnectionId: string) {
        this.chatHubProxy.server.sendToVisitor(message, visitorConnectionId);
    }

    updateConversations(clientId: number, name: string, time: number, location: string, currentUrl: string) {
        var jsTime = new Date();
        jsTime.setTime(time);

        var conversation: ConversationModel =
            {
                visitorId: clientId,
                visitorName: name,
                conversationStartTime: jsTime,
                location: location,
                visitorUrl: currentUrl,
                messages: [],
                isCurrent: this.conversations.length == 0
            };

        this.conversations.push(conversation);  
    }
}