import {Component, ElementRef, OnInit, Input, Output, EventEmitter} from 'angular2/core';
import {ConversationModel} from '../models/conversation.model';
import {MessageModel} from '../models/message.model';

declare var jQuery: any;

@Component({
    selector: 'chat-list',
    templateUrl: '/app/chat/chat-list.component.html'
})

export class ChatListComponent implements OnInit {

    @Input() conversations: ConversationModel[];
         

    constructor(private elementRef: ElementRef) {
      
    }

    ngOnInit() {

    }

    switch(event: any) {
        
    }
}