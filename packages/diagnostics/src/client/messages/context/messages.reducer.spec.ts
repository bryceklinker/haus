import {
    connected,
    connecting,
    disconnected,
    INITIAL_MESSAGES_STATE,
    messageReceived,
    reducer,
    selectMessages,
    selectMessagesStatus
} from './messages.reducer';
import {createMessageModel} from '../models/message.model';

describe('messagesReducer', () => {
    test('when state is not provided then initial state is used', () => {
        const state = reducer(undefined, {type: 'idk'});

        expect(state).toEqual(INITIAL_MESSAGES_STATE);
    });

    test('when message received then message is added to messages array', () => {
        const message = createMessageModel({topic: 'stuff', payload: '123'});

        let state = reducer(undefined, {type: 'idk'});
        state = reducer(state, messageReceived(message));

        expect(selectMessages(state)).toContainEqual(message);
    });

    test('when connected then status is connected', () => {
        let state = reducer(undefined, {type: 'ikd'});
        state = reducer(state, connected());

        expect(selectMessagesStatus(state)).toEqual('connected');
    });

    test('when connecting then status is connecting', () => {
        let state = reducer(undefined, {type: 'idk'});
        state = reducer(state, connecting());

        expect(selectMessagesStatus(state)).toEqual('connecting');
    })

    test('when disconnected then status is disconnected', () => {
        let state = reducer(undefined, {type: 'ikd'});
        state = reducer(state, connected());
        state = reducer(state, disconnected());

        expect(selectMessagesStatus(state)).toEqual('disconnected');
    });
});
