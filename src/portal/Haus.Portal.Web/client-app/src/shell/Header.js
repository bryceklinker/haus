import React from "react";
import {AppBar, Toolbar, Typography} from "@material-ui/core";
import {useCurrentUser} from "../shared/auth";

export function Header() {
    const user = useCurrentUser();
    return (
        <header>
            <AppBar position={'fixed'}>
                <Toolbar>
                    <Typography variant={'h3'}>
                        Welcome {user.profile.name}
                    </Typography>
                </Toolbar>
            </AppBar>
        </header>
    )
}