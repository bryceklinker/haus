import React from 'react';
import {Container} from '@material-ui/core';
import {Messages} from '../messages/Messages';

export function MainContent() {
    return (
        <main>
            <Container>
                <Messages />
            </Container>
        </main>
    )
}
