import {Component} from 'angular2/core';
import {ChatWindowComponent} from './chat-window.component';

@Component({ 
    selector: 'chat-room',
    templateUrl: '/app/chat/chat-room.html',
    directives: [ChatWindowComponent]
})

export class ChatRoom { }