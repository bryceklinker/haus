import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import {Toolbar} from '@material-ui/core';
import {useChangeTheme, useThemeType, useThemeTypes} from '../common/theming/theme.context';
import Switch from '@material-ui/core/Switch';
import {makeStyles} from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
    spacer: {
        flex: 1
    }
}));
export function Header() {
    const changeTheme = useChangeTheme();
    const themeTypes = useThemeTypes();
    const themeType = useThemeType();
    const handleChange = (checked) => {
        changeTheme(checked ? themeTypes.DARK : themeTypes.LIGHT);
    };
    const classes = useStyles();
    return (
        <header>
            <AppBar>
                <Toolbar>
                    <span className={classes.spacer}/>
                    <Switch checked={themeType === themeTypes.DARK}
                            onChange={handleChange}
                            color={'primary'}/>
                </Toolbar>
            </AppBar>
        </header>
    )
}
