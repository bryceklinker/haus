import {createReducer, handleAction} from '../../shared/state/reducers';
import {createSelector} from 'reselect';
import {loadUsers} from './users.actions';

const initialState = {users: []};
export const usersReducer = createReducer(
    initialState,
    handleAction(loadUsers.success, (state, action) => ({...state, users: action.payload.items}))
);

const selectUsersState = state => state.users;
export const selectUsers = createSelector(selectUsersState, s => s.users);