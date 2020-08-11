import React from "react";
import Container from '@material-ui/core/Container';

export function MainContent({isDrawerOpen, children}) {
    return (
        <main>
            <Container>
                {children}
            </Container>
        </main>
    )
}