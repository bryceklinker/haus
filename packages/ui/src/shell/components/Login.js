import React from 'react';
import {useLoginRedirect} from '../../common/auth';
import {Button, Typography} from '@material-ui/core';

export function Login() {
    const loginRedirect = useLoginRedirect();
    return (
        <Button variant={'contained'} onClick={() => loginRedirect()}>
            <Typography variant={'h3'}>Login</Typography>
        </Button>
    )
}
