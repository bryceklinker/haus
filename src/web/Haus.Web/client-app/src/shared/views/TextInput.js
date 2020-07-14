import React from 'react';
import TextField from '@material-ui/core/TextField';

export function TextInput({'data-testid': testId, ...rest}) {
    return <TextField {...rest} inputProps={{
        'data-testid': testId
    }}/>
}