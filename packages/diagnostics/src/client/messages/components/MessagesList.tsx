import * as React from 'react';
import {MessageModel} from '../models/message.model';
import {MessageItem} from './MessageItem';

interface MessagesListProps {
    messages: Array<MessageModel>;
}

export function MessagesList({messages}: MessagesListProps) {
    const items = messages.map((m, i) => <MessageItem message={m} key={i} />);
    return (
        <div>
            {items}
        </div>
    )
}
