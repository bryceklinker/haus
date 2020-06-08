import React from "react";
import TextField from "@material-ui/core/TextField";

export function TextInput({'data-testid': testId, ...rest}) {
    const {InputProps = {}} = rest;
    const inputProps = {
        ...InputProps,
        inputProps: {
            ...(InputProps.inputProps || {}),
            'data-testid': testId
        }
    };
    return <TextField {...rest} InputProps={inputProps} />
}