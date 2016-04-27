import {Component, ElementRef, OnInit} from 'angular2/core';

declare var jQuery: any;

@Component({
    selector: 'chat-window',
    templateUrl: '/app/chat/chat-window.component.html'
})
    
export class ChatWindowComponent implements OnInit {

    elementRef: ElementRef;
    messages: Message[];
    clientName: string;
    clientId: string;
    currentMessage: string;
    conversationStartTime: Date;
    chatHubProxy: any;
    hub: any;

    constructor(elementRef: ElementRef) {
        this.elementRef = elementRef;
        this.messages = new Array<Message>();
        
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

    ngOnInit() {
        this.chatHubProxy.client.addNewMessageToPage = jQuery.proxy(function (name, message, time, sender, clientId) {

            var jsTime = new Date();
            jsTime.setTime(time);

            var msg: Message = { id: 1, author: name, text: message, sender: sender, time: jsTime };
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

            this.chatHubProxy.server.connectOperator('John Dou', '086FBDC2-14F3-4F68-B3C6-9EA42D257061').done(function () { })

        }, this));
    }

    onEnter(event: any) {
        this.chatHubProxy.server.sendToClient('John Dou', this.currentMessage, this.clientId);
        this.currentMessage = '';
    }
}

class Message {
    public id: number;
    public author: string;
    public sender: string;
    public text: string;
    public time: Date;
}