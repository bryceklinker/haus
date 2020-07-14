import React from 'react';
import { CircularProgress } from '@material-ui/core'

export function LoadingView({show}) {
    if (show) {
        return <CircularProgress data-testid={'loading-indicator'} />
    }
    return null;
}