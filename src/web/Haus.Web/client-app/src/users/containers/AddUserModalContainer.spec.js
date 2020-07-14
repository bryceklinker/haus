import React from 'react';
import {addUser} from '../state/users.actions';
import {renderWithStore} from '../../testing/render-with-store';
import {AddUserModalContainer} from './AddUserModalContainer';

test('when rendered modal defaults to closed', () => {
    const {queryAllByTestId} = renderWithStore(<AddUserModalContainer/>);

    expect(queryAllByTestId('add-user-modal')).toHaveLength(0);
});

test('when adding user then modal is open', () => {
    const {queryAllByTestId} = renderWithStore(<AddUserModalContainer/>, addUser.open());

    expect(queryAllByTestId('add-user-modal')).toHaveLength(1);
});

test('when adding user is cancelled then cancel action is dispatched', () => {
    const {getByTestId, fireEvent, store} = renderWithStore(<AddUserModalContainer/>, addUser.open());

    fireEvent.click(getByTestId('cancel-user-button'));

    expect(store.getActions()).toContainEqual(addUser.cancel());
});

test('when adding user is confirmed then confirm action is dispatched', () => {
    const {getByTestId, fireEvent, store} = renderWithStore(<AddUserModalContainer/>, addUser.open());

    fireEvent.change(getByTestId('username-input'), {target: {value: 'someone'}});
    fireEvent.change(getByTestId('password-input'), {target: {value: 'password'}});
    fireEvent.click(getByTestId('save-user-button'));

    expect(store.getActions()).toContainEqual(addUser.confirm({
        username: 'someone',
        password: 'password'
    }));
});