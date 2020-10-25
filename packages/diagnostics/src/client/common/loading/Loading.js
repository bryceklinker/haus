import React from 'react';
import {CircularProgress} from '@material-ui/core';

export function Loading({show = true}) {
    const hide = !show;
    if (hide) {
        return null;
    }

    return (
        <CircularProgress data-testid={'loading'} />
    )
}
