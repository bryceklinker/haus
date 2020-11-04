import * as React from 'react';
import {Paper, Typography} from '@material-ui/core';
import ReactJson from 'react-json-view';
import {MessageModel} from '../models/message.model';
import {makeStyles} from '@material-ui/core/styles';

interface MessageItemProps {
    message: MessageModel;
}

const useStyles = makeStyles(theme => ({
    paper: {
        padding: theme.spacing(2),
        marginBottom: theme.spacing(1)
    }
}));
export function MessageItem({message}: MessageItemProps) {
    const styles = useStyles();
    const {topic} = message;
    return (
        <Paper data-testid={'message-item'} className={styles.paper}>
            <Typography variant={'h6'}>Topic: {topic}</Typography>
            <ReactJson src={message} />
        </Paper>
    );
}
