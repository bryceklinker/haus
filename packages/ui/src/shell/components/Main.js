import React from 'react';
import {useAuthUser} from '../../common';
import {Login} from './Login';
import makeStyles from '@material-ui/core/styles/makeStyles';
import {Toolbar} from '@material-ui/core';

const useStyles = makeStyles(theme => ({
    main: {
        display: 'flex',
        flex: 1
    },
    content: {
        flexGrow: 1,
        padding: theme.spacing(3)
    }
}));

export function Main() {
    const classes = useStyles();
    const user = useAuthUser();
    if (!user) {
        return <Login />
    }
    return (
        <main className={classes.main}>
            <div className={classes.content}>
                <Toolbar />
                {!user ? <Login /> : null}
            </div>
        </main>
    )
}
