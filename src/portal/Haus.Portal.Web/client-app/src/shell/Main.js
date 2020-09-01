import React from "react";
import {Container} from '@material-ui/core';
import {useCurrentUser} from "../shared/auth";

export function Main() {
    const user = useCurrentUser();
    return (
        <main>
            <Container>
                {JSON.stringify(user)}    
            </Container>
        </main>
    )
}