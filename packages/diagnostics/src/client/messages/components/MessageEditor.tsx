import {useInput} from '../../common/forms/use-input';
import {FormControl, FormControlLabel, FormLabel, Radio, RadioGroup, TextField} from '@material-ui/core';
import React, {ChangeEvent} from 'react';
import JSONInput from 'react-json-editor-ajrm';

interface MessageEditorProps {
    value: any;
    onChange: (value: any) => void;
    disabled: boolean;
}

export function MessageEditor({value, onChange, disabled}: MessageEditorProps) {
    const [messageType, onMessageTypeChange] = useInput('json');
    const createChangeEvent = (newValue: any) => ({currentTarget: {value: newValue}});
    const handleJsonChange = (content: any) => {
        onChange(createChangeEvent(content.jsObject));
    };
    const jsonEditor = messageType === 'json'
        ? <JSONInput value={value}
                     onChange={handleJsonChange}
                     viewOnly={disabled}/>
        : null;

    const textEditor = messageType === 'text'
        ? <TextField value={value}
                     fullWidth
                     label={'Message Content'}
                     onChange={onChange}/>
        : null;

    const handleMessageTypeChange = (evt: ChangeEvent<HTMLInputElement>, radioValue: string) => {
        const newValue = radioValue === 'json' ? {} : '';
        onChange(createChangeEvent(newValue));
        onMessageTypeChange(evt);
    };
    return (
        <div>
            <FormControl fullWidth component={'fieldset'}>
                <FormLabel component={'legend'}>Message Type</FormLabel>
                <RadioGroup row value={messageType} onChange={handleMessageTypeChange}>
                    <FormControlLabel value={'json'} control={<Radio/>} label={'JSON'}/>
                    <FormControlLabel value={'text'} control={<Radio/>} label={'Text'}/>
                </RadioGroup>
            </FormControl>

            {jsonEditor}
            {textEditor}
        </div>
    );
}
