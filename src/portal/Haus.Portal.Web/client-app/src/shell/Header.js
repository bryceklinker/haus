import React from "react";
import {AppBar, Toolbar} from "@material-ui/core";

export function Header() {
    return (
        <header>
            <AppBar position={'fixed'}>
                <Toolbar/>
            </AppBar>
        </header>
    )
}