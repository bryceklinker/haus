import React, {useState} from 'react';

import {Footer} from './Footer';
import {MainContent} from './MainContent';
import {Header} from './Header';
import {Routes} from '../../shared/routing/routes';
import Grid from '@material-ui/core/Grid';
import makeStyles from '@material-ui/core/styles/makeStyles';
import {Sidenav} from './Sidenav';
import {createTheme} from '../../shared/theming/create-theme';
import CssBaseline from '@material-ui/core/CssBaseline';
import {ThemeProvider} from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
    toolbarOffset: theme.mixins.toolbar,
    root: {
        display: 'flex'
    }
}));

export function Shell() {
    const [isDrawerOpen, setDrawerState] = useState(false);
    const [isDarkTheme, setDarkTheme] = useState(true);
    const classes = useStyles();
    const theme = createTheme(isDarkTheme);
    const handleToggleDrawer = () => setDrawerState(!isDrawerOpen);
    const handleCloseDrawer = () => setDrawerState(false);
    const handleToggleDarkTheme = () => setDarkTheme(!isDarkTheme);
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline/>
            <Grid container direction={'column'} className={classes.root}>
                <Header onToggleDrawer={handleToggleDrawer} onToggleDarkTheme={handleToggleDarkTheme} isDarkTheme={isDarkTheme}/>

                <div className={classes.toolbarOffset}/>
                <Sidenav isDrawerOpen={isDrawerOpen} onClose={handleCloseDrawer}/>
                <MainContent isDrawerOpen={isDrawerOpen}>
                    <Routes/>
                </MainContent>

                <Footer/>
            </Grid>
        </ThemeProvider>
    );
}