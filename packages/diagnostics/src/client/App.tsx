import * as React from 'react';
import {useMessages, useMessagesStatus} from './messages/context/messages.context';
import {MessagesList} from './messages/components/MessagesList';
import {AppBar, Switch, Toolbar, Typography} from '@material-ui/core';
import {
    useDarkTheme,
    useIsDarkTheme,
    useLightTheme,
} from './common/theming/theme.context';
import {makeStyles} from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
    root: {
        display: 'flex',
        flex: 1,
        flexDirection: 'column'
    },
    spacer: {
        flexGrow: 1
    },
    buffer: theme.mixins.toolbar,
    content: {
        flex: 1,
        display: 'flex',
        flexDirection: 'column',
        padding: theme.spacing(2)
    }
}));
export function App() {
    const messages = useMessages();
    const status = useMessagesStatus();
    const isDarkMode = useIsDarkTheme();
    const changeToDarkMode = useDarkTheme();
    const changeToLightMode = useLightTheme();
    const handleChangeTheme = (_: any, checked: boolean) => {
        if (checked) {
            changeToDarkMode()
        } else {
            changeToLightMode();
        }
    }
    const styles = useStyles();
    return (
        <div className={styles.root}>
            <AppBar position={'fixed'}>
                <Toolbar>
                    <Typography variant={'h5'}>Haus Diagnostics</Typography>
                    <span className={styles.spacer}/>
                    <Switch checked={isDarkMode}
                            onChange={handleChangeTheme}/>
                </Toolbar>
            </AppBar>
            <Toolbar />
            <div className={styles.content}>
                <Typography variant={'h5'}>Status: {status}</Typography>
                <MessagesList messages={messages} />
            </div>
        </div>
    )
}
