import React from 'react';
import {render} from '@testing-library/react';
import {MessagesList} from './MessagesList';
import {createMessageModel} from '../models/message.model';

describe('MessagesList', () => {
    test('when rendered then shows each message', () => {
        const messages = [
            createMessageModel({topic: '', payload: ''}),
            createMessageModel({topic: '', payload: ''}),
            createMessageModel({topic: '', payload: ''})
        ];

        const {queryAllByTestId} = render(<MessagesList messages={messages}/>);

        expect(queryAllByTestId('message-item')).toHaveLength(3);
    });
});
