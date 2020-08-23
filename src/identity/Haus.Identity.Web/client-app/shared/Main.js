import React from "react";
import {CssBaseline, ThemeProvider, Container, makeStyles} from "@material-ui/core";
import {theme} from "./theme";
import {Header} from "./Header";

const useStyles = makeStyles(theme => ({
    offset: theme.mixins.toolbar
}));
export function Main({children}) {
    const classes = useStyles();
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <Header />
            
            <div className={classes.offset} />
            
            <Container>
                {children}    
            </Container>
        </ThemeProvider>
    )
}