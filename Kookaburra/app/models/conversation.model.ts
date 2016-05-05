import {MessageModel} from './message.model';

export class ConversationModel {

    public visitorId: number;
    public visitorName: string;
    public conversationStartTime: Date;
    public location: string;
    public visitorUrl: string;
    public messages: MessageModel[];
}