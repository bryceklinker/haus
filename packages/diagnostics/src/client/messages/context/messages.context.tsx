import * as React from 'react';
import io from 'socket.io-client';
import {createContext, useReducer, useMemo, useEffect, Dispatch, useContext} from 'react';
import {
    connected,
    connecting,
    disconnected,
    INITIAL_MESSAGES_STATE,
    messageReceived,
    MessagesState,
    reducer, selectMessages, selectMessagesStatus
} from './messages.reducer';
import {Action} from '../../common/actions';

type MessagesContextValue = {
    state: MessagesState;
    dispatch: Dispatch<Action>;
}
const MessagesContext = createContext<MessagesContextValue>({
    state: INITIAL_MESSAGES_STATE,
    dispatch: () => ({type: 'hello'})
});

function useMessagesContext(hookName: string) {
    const context = useContext(MessagesContext);
    if (context) {
        return context;
    }

    throw new Error(`${hookName} must be used under ${MessagesProvider.name}`);
}

export function useMessages() {
    const {state} = useMessagesContext(useMessages.name);
    return selectMessages(state);
}

export function useMessagesStatus() {
    const {state} = useMessagesContext(useMessagesStatus.name);
    return selectMessagesStatus(state);
}

export function MessagesProvider({connect = io, ...rest}) {
    const [state, dispatch] = useReducer(reducer, INITIAL_MESSAGES_STATE);
    const value = useMemo(() => ({state, dispatch}), [state]);
    useEffect(() => {
        dispatch(connecting());

        const socket = connect();
        socket.on('connect', () => dispatch(connected()));
        socket.on('disconnect', () => dispatch(disconnected()));
        socket.on('message', (message: any) => dispatch(messageReceived(message)));
    }, [dispatch]);
    return <MessagesContext.Provider value={value} {...rest} />;
}
