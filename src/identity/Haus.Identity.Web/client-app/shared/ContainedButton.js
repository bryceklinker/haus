import React from "react";
import {Button} from "@material-ui/core";

export function ContainedButton({children, ...rest}) {
    return (
        <Button variant={'contained'} {...rest}>
            {children}
        </Button>
    )
}