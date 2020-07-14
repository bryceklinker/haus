import {useState} from 'react';

export function useInput(initialValue = '') {
    const [value, setValue] = useState(initialValue);
    const handler = (evt) => setValue(evt.target.value);
    return [value, handler];
}