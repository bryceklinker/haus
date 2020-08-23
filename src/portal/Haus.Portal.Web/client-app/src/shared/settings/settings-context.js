import React, {createContext, useContext, useEffect, useState} from "react";
import {Loading} from "../loading";

const SettingsContext = createContext();

export function useSettings() {
    const context = useContext(SettingsContext);
    if (context) {
        return context;
    }
    
    throw new Error(`${Function.name} must be used under SettingsProvider`);
}

async function getSettings() {
    const response = await fetch('/settings');
    return await response.json();
}

export function SettingsProvider({children, ...rest}) {
    const [settings, setSettings] = useState(null);
    useEffect(() => {
        getSettings()
            .then(value => setSettings(value))
            .catch(err => console.error(err));
    }, [settings])
    return (
        <SettingsContext.Provider value={settings} {...rest}>
            {
                settings ? children : <Loading />
            }
        </SettingsContext.Provider>
    )
}