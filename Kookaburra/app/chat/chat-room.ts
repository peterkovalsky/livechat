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

    accountId: string;
    operatorName: string;    

    chatHubProxy: any;
    hub: any;

    conversations: ConversationModel[];

    constructor(private elementRef: ElementRef) {
        var native = this.elementRef.nativeElement;
        this.accountId = native.getAttribute("data-account-id");
        this.operatorName = native.getAttribute("data-operator-name");
    }

    ngOnInit() {

        // Start the connection.
        this.hub.start().done(jQuery.proxy(function () {

            this.chatHubProxy.server.connectOperator(this.operatorName, this.accountId).done(function () { })

        }, this));

        this.chatHubProxy.client.clientConnected = jQuery.proxy(function (clientId, name, time, location, currentUrl) {
            var jsTime = new Date();
            jsTime.setTime(time);

            var conversation: ConversationModel =
                {
                    visitorId: clientId,
                    visitorName: name,
                    conversationStartTime: jsTime,
                    location: location,
                    visitorUrl: currentUrl,
                    messages:[]
                };

            this.conversations.push(conversation);          

        }, this);
    }
}