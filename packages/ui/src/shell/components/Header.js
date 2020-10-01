import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import makeStyles from '@material-ui/core/styles/makeStyles';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import Typography from '@material-ui/core/Typography';

import {THEME_TYPES, useSetTheme, useThemeType} from '../../common';
import {UserActions} from './UserActions';

const useStyles = makeStyles(theme => ({
    spacer: {
        flexGrow: 1
    },
    appbar: {
        display: 'flex',
        zIndex: theme.zIndex.drawer + 1
    },
}));

export function Header() {
    const classes = useStyles();
    const setTheme = useSetTheme();
    const themeType = useThemeType();
    const options = THEME_TYPES.ALL.map(t => (
        <MenuItem key={t} value={t} data-testid={t}>
            {t}
        </MenuItem>
    ));
    const handleThemeChange = (evt) => setTheme(evt.target.value);

    return (
        <header>
            <AppBar position={'fixed'} className={classes.appbar}>
                <Toolbar>
                    <Typography variant={'h6'} noWrap>
                        Haus Portal
                    </Typography>

                    <div className={classes.spacer}/>

                    <FormControl data-testid={'theme-select'}>
                        <Select onChange={handleThemeChange}
                                value={themeType}>
                            {options}
                        </Select>
                    </FormControl>

                    <UserActions/>
                </Toolbar>
            </AppBar>
        </header>
    );
}
