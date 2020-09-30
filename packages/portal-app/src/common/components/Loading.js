import React from 'react';
import CircularProgress from '@material-ui/core/CircularProgress';

function convertToSize(size) {
    if (size === 'large') {
        return '5rem';
    } else if (size === 'small') {
        return '1rem';
    } else {
        return '3rem';
    }
}

export function Loading({size = 'normal'}) {
    return (
        <CircularProgress size={convertToSize(size)} />
    )
}
