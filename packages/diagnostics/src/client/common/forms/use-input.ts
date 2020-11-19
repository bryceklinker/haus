import {ChangeEvent, useState} from 'react';

export function useInput(defaultValue?: any) {
    const [value, setValue] = useState(defaultValue);
    const onChange = (evt: ChangeEvent<HTMLInputElement>) => {
        setValue(evt.currentTarget.value);
    }

    return [value, onChange, setValue];
}
