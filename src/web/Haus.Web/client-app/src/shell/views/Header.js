import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu'
import makeStyles from '@material-ui/core/styles/makeStyles';
import {Switch} from '@material-ui/core';

const useStyles = makeStyles(theme => ({
    appBar: {
        zIndex: theme.zIndex.drawer + 1
    }
}));

export function Header({onToggleDrawer, isDarkTheme, onToggleDarkTheme}) {
    const classes = useStyles();
    
    return (
        <AppBar position={'fixed'} className={classes.appBar}>
            <Toolbar>
                <IconButton onClick={onToggleDrawer}>
                    <MenuIcon />
                </IconButton>  
                
                <Switch checked={isDarkTheme} onChange={onToggleDarkTheme} />
            </Toolbar>
        </AppBar>
    );
}