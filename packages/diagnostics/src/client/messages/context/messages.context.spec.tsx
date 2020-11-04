import React from 'react';
import {act, render, waitFor} from '@testing-library/react';
import {MessagesProvider, useMessages, useMessagesStatus} from './messages.context';
import {SocketIoClientFake} from '../../../testing/socket-io-client-fake';
import {createMessageModel} from '../models/message.model';

describe('MessagesContext', () => {
    let socketIoFake: SocketIoClientFake;
    let connect: jest.MockedFunction<any>;

    beforeEach(() => {
        socketIoFake = new SocketIoClientFake();
        connect = jest.fn().mockReturnValue(socketIoFake);
    });

    test('when connected then shows connected', async () => {
        const {container} = renderTester();

        act(() => socketIoFake.triggerConnect());

        await waitFor(() => {
            expect(container).toHaveTextContent('connected');
        });
    });

    test('when disconnected then shows disconnected', async () => {
        const {container} = renderTester();

        act(() => socketIoFake.triggerDisconnect());

        await waitFor(() => {
            expect(container).toHaveTextContent('connected');
        });
    });

    test('when message received then message is shown', async () => {
        const {container} = renderTester();

        act(() => socketIoFake.triggerMessage(createMessageModel({topic: 'bob', payload: 'hello'})));

        await waitFor(() => {
            expect(container).toHaveTextContent('bob');
            expect(container).toHaveTextContent('hello');
        });
    })

    function renderTester() {
        return render(
            <MessagesProvider connect={connect}>
                <MessagesContextTester/>
            </MessagesProvider>
        );
    }
});

function MessagesContextTester() {
    const status = useMessagesStatus();
    const messages = useMessages();
    return (
        <div>
            {status}
            <pre>{JSON.stringify(messages, null, 2)}</pre>
        </div>
    );
}
