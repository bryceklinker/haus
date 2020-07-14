import {createAsyncActions, createModalActions} from '../../shared/state/actions';

export const loadUsers = createAsyncActions('[Users] Load');

export const addUser = createModalActions('[Users] Add User');