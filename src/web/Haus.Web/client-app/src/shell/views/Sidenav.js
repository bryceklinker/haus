import Drawer from '@material-ui/core/Drawer';
import React from 'react';
import makeStyles from '@material-ui/core/styles/makeStyles';

const drawerWidth = '240px';
const useStyles = makeStyles(theme => ({
    sidenav: {
        width: drawerWidth,
        overflow: 'auto'
    },
}));

export function Sidenav({isDrawerOpen, onClose}) {
    const classes = useStyles();
    return (
        <Drawer open={isDrawerOpen}
                className={classes.drawer}
                onBackdropClick={onClose}>
            <div className={classes.sidenav}>
                
            </div>
        </Drawer>
    );
}