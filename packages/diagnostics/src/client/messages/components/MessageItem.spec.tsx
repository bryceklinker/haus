import * as React from 'react';
import {render} from '@testing-library/react';
import {MessageItem} from './MessageItem';
import {createMessageModel} from '../models/message.model';

describe('MessageItem', () => {
    test('when rendered then shows message topic', () => {
        const message = createMessageModel({topic: 'hello', payload: {}});

        const {getByTestId} = render(<MessageItem message={message}/>);

        expect(getByTestId('message-item')).toHaveTextContent('hello');
    });

    test('when rendered then shows message payload', () => {
        const message = createMessageModel({topic: '', payload: {id: '123'}});

        const {getByTestId} = render(<MessageItem message={message} />);

        expect(getByTestId('message-item')).toHaveTextContent('123');
    });
});
