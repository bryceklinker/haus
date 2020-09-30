import React from 'react';
import {useAuthUser, useLoginRedirect, useLogout} from '../../common/auth';
import Button from '@material-ui/core/Button';
import makeStyles from '@material-ui/core/styles/makeStyles';

const useStyles = makeStyles(theme => ({
    container: {
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
    }
}))

export function UserActions() {
    const user = useAuthUser();
    const logout = useLogout();
    const login = useLoginRedirect();
    const classes = useStyles();
    return (
        <div className={classes.container}>
            {user ? <Button variant={'text'} onClick={logout}>Logout</Button> : null}
            {!user ? <Button variant={'text'} onClick={login}>Login</Button> : null}
        </div>
    );
}
