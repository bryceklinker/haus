import React from 'react';
import {List, ListItem} from '@material-ui/core';
import {LoadingView} from '../../shared/loading/LoadingView';

export function UserListView({users = [], isLoading = false}) {
    const userItems = users.map(u => <ListItem key={u.username}>{u.username}</ListItem>);
    return (
        <List>
            <LoadingView show={isLoading}/>
            {userItems}
        </List>
    );
}