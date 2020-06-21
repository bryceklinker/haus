import React, {useEffect} from 'react';
import {connect} from 'react-redux';
import {loadUsers} from '../state/users.actions';
import {UserListView} from '../views/UserListView';
import {selectUsers} from '../state/users.reducer';
import {selectIsLoading} from '../../shared/loading/loading.reducer';

function UserList({loadUsers, users, isLoadingUsers}) {
    useEffect(() => {
        loadUsers();
    }, []);
    return <UserListView users={users} isLoading={isLoadingUsers}/>;
}

function mapStateToProps(state) {
    return {
        users: selectUsers(state),
        isLoadingUsers: selectIsLoading(state, loadUsers.request)
    }
}

function mapDispatchToProps(dispatch) {
    return {
        loadUsers: () => dispatch(loadUsers.request())
    };
}

export const UserListContainer = connect(mapStateToProps, mapDispatchToProps)(UserList);