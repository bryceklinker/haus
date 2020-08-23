import React from "react";
import {useCurrentUser} from "../shared/auth";

export function Main() {
    const user = useCurrentUser();
    return (
        <main>
            {JSON.stringify(user)}
        </main>
    )
}