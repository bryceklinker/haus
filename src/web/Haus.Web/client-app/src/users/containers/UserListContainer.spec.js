import React from 'react';
import {renderWithStore} from '../../testing/render-with-store';
import {UserListContainer} from './UsersListContainer';
import {loadUsers} from '../state/users.actions';

test('when rendered then users are loaded', () => {
    const {store} = renderWithStore(<UserListContainer/>);

    expect(store.getActions()).toContainEqual(loadUsers.request());
});

test('when rendered with then users are shown', () => {
    const userList = {
        items: [
            {username: 'bob'},
            {username: 'jack'},
            {username: 'joe'},
        ]
    }
    const {container} = renderWithStore(<UserListContainer/>, loadUsers.success(userList));
    
    expect(container).toHaveTextContent('bob');
    expect(container).toHaveTextContent('jack');
    expect(container).toHaveTextContent('joe');
});

test('when users are being loaded then loading is shown', () => {
   const {queryAllByTestId} = renderWithStore(<UserListContainer />, loadUsers.request());
   
   expect(queryAllByTestId('loading-indicator')).toHaveLength(1);
});