import React from 'react';
import Dialog from '@material-ui/core/Dialog';
import DialogTitle from '@material-ui/core/DialogTitle';
import DialogContent from '@material-ui/core/DialogContent';
import DialogActions from '@material-ui/core/DialogActions';
import Button from '@material-ui/core/Button';

import {TextInput} from '../../shared/views/TextInput';
import {useInput} from '../../shared/hooks/use-input.hook';

export function AddUserModalView({isOpen = false, onConfirm = (() => {}), onCancel = (() => {}) }) {
    const [username, usernameHandler] = useInput('');
    const [password, passwordHandler] = useInput('');
    const handleConfirm = () => {
        onConfirm({ username, password });
    }
    return (
        <Dialog open={isOpen} data-testid={'add-user-modal'} onClose={onCancel}>
            <DialogTitle>Add User Dialog</DialogTitle>
            <DialogContent>
                <TextInput autoFocus data-testid={'username-input'} value={username} onChange={usernameHandler}/>
                <TextInput data-testid={'password-input'} value={password} onChange={passwordHandler} type={'password'} />
            </DialogContent>
            <DialogActions>
                <Button onClick={onCancel} data-testid={'cancel-user-button'}>
                    Cancel
                </Button>
                <Button onClick={handleConfirm} data-testid={'save-user-button'}>
                    Save
                </Button>
            </DialogActions>
        </Dialog>
    );
}