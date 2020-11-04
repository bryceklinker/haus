import React from 'react';
import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Paper,
    TextField,
    Typography
} from '@material-ui/core';
import {useApiPost} from '../../common/api/api-hooks';
import {useInput} from '../../common/forms/use-input';

import {MessageEditor} from './MessageEditor';

interface PublishMessageModalProps {
    open: boolean;
    onClose: () => void;
}

export function PublishMessageModal({open, onClose}: PublishMessageModalProps) {
    const [topic, onTopicChange] = useInput();
    const [message, onMessageChange] = useInput({});
    const {isLoading, error, execute} = useApiPost('/mqtt/publish');
    const handlePublish = async () => {
        const success = await execute({topic, payload: message});
        if (success) {
            onClose();
        }
    };

    return (
        <Dialog fullScreen onClose={onClose} open={open}>
            <DialogTitle>Publish Mqtt Message</DialogTitle>
            <DialogContent>
                <Paper hidden={!error}>
                    <Typography variant={'h5'}>Failed to publish message</Typography>
                </Paper>

                <TextField autoFocus
                           fullWidth
                           label={'Topic'}
                           value={topic}
                           onChange={onTopicChange}
                           disabled={isLoading}/>

                <MessageEditor value={message} onChange={onMessageChange} disabled={isLoading}/>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} color={'primary'} disabled={isLoading}>
                    Cancel
                </Button>
                <Button onClick={handlePublish} color={'primary'} disabled={isLoading}>
                    Publish
                </Button>
            </DialogActions>
        </Dialog>
    );
}
