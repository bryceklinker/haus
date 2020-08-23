import React, {useState} from "react";
import {TextField, makeStyles} from "@material-ui/core";

const useStyles = makeStyles(theme => ({
    textField: {
        margin: theme.spacing(1)
    }
}))
export function TextInput({'data-testid': testId, ...rest}) {
    const classes = useStyles();
    return (
        <TextField className={classes.textField} {...rest} InputProps={{
            'data-testid': testId
        }}/>
    )
}

export function useInput(initialValue = '') {
    const [value, setValue] = useState(initialValue);
    const bind = {
        value,
        onChange: evt => setValue(evt.target.value)
    };
    const reset = () => setValue('');
    return [value, bind, setValue, reset];
}