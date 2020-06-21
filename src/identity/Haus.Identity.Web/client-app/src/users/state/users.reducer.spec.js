import {initAction} from '../../testing/testing.actions';
import {usersReducer} from './users.reducer';
import {loadUsers} from './users.actions';

test('when initialized then initial state returned', () => {
    const state = usersReducer(undefined, initAction());
    
    expect(state).toEqual({ users: [] });
})

test('when load users successful then users is populated', () => {
    const users = {
        items: [
            {username: 'joe'},
            {username: 'jack'},
            {username: 'stacey'}
        ]
    }
    let state = usersReducer(undefined, initAction());
    state = usersReducer(state, loadUsers.success(users));
    
    expect(state.users).toEqual(users.items);
});