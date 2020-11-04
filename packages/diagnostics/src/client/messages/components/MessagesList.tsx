import * as React from 'react';
import {MessageModel} from '../models/message.model';
import {MessageItem} from './MessageItem';
import {PublishMessageModal} from './PublishMessageModal';
import {useState} from 'react';
import {Button, Card, CardActionArea, CardActions, CardContent, Toolbar} from '@material-ui/core';
import {makeStyles} from '@material-ui/core/styles';

interface MessagesListProps {
    messages: Array<MessageModel>;
}

const useStyles = makeStyles(theme => ({
    card: {
        flex: 1
    },
    cardSpacer: {
        flexGrow: 1,
    }
}));

export function MessagesList({messages}: MessagesListProps) {
    const [isPublishOpen, setIsPublishOpen] = useState(false);
    const handlePublishClick = () => setIsPublishOpen(true);
    const handleModalClose = () => setIsPublishOpen(false);
    const items = messages.map((m, i) => <MessageItem message={m} key={i}/>);
    const styles = useStyles();
    return (
        <Card className={styles.card}>
            <CardActions>
                <span className={styles.cardSpacer} />
                <Button onClick={handlePublishClick} variant={'contained'} color={'primary'}>
                    Publish Message
                </Button>
            </CardActions>
            <CardContent>
                {items}
                <PublishMessageModal open={isPublishOpen} onClose={handleModalClose}/>
            </CardContent>
        </Card>
    );
}
