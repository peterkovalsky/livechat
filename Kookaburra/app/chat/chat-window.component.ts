import {Component, ElementRef, OnInit, Input, Output, EventEmitter} from 'angular2/core';
import {ConversationModel} from '../models/conversation.model';
import {MessageModel} from '../models/message.model';

declare var jQuery: any;

@Component({
    selector: 'chat-window',
    templateUrl: '/app/chat/chat-window.component.html'
})
    
export class ChatWindowComponent implements OnInit {  

    @Input() conversation: ConversationModel;
    @Output() onEnterMessage: EventEmitter<string> = new EventEmitter();

    currentMessage: string;
  

    constructor(private elementRef: ElementRef) {
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

    ngOnInit() {
         
    }

    onEnter(event: any) {
        this.onEnterMessage.emit(this.currentMessage);        
    }
}