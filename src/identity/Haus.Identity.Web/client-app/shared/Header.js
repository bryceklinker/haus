import React from "react";
import {AppBar, Toolbar, Typography} from "@material-ui/core";

export function Header() {
    return (
        <AppBar position={'fixed'}>
            <Toolbar>
                <Typography variant={'h4'}>Haus Identity</Typography>
            </Toolbar>
        </AppBar>
    )
}