import {MessageModel} from '../models/message.model';
import {Action, createAction} from '../../common/actions';

export type ConnectionStatus = 'connected' | 'connecting' | 'disconnected';

export interface MessagesState {
    status: ConnectionStatus;
    messages: Array<MessageModel>;
}

export const INITIAL_MESSAGES_STATE: MessagesState = {
    status: 'disconnected',
    messages: []
};

export const messageReceived = createAction<MessageModel>('[Messages] Received Message');
const statusChanged = createAction<ConnectionStatus>('[Messages] Status Changed');
export const connecting = () => statusChanged('connecting');
export const connected = () => statusChanged('connected');
export const disconnected = () => statusChanged('disconnected');

export function reducer(state: MessagesState = INITIAL_MESSAGES_STATE, action: Action) {
    switch (action.type) {
        case messageReceived.type:
            return {...state, messages: [...state.messages, action.payload]};
        case statusChanged.type:
            return {...state, status: action.payload};
        default:
            return state;
    }
}

export const selectMessages = (s: MessagesState) => s.messages;
export const selectMessagesStatus = (s: MessagesState) => s.status;
