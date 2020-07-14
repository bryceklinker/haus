import {addUser, loadUsers} from './users.actions';
import {createSagaTester} from '../../testing/create-saga-tester';
import {usersSaga} from './users.sagas';

let requestor, sagaTester;

beforeEach(() => {
    const result = createSagaTester();
    requestor = result.requestor;
    sagaTester = result.sagaTester;
    sagaTester.start(usersSaga);
})

test('when load users requested then retrieves users from api', async () => {
    const body = {items: [{}, {}, {}]};
    requestor.setupGet('/api/users', JSON.stringify(body));

    sagaTester.dispatch(loadUsers.request());

    const successAction = await sagaTester.waitFor(loadUsers.success.type);
    expect(successAction).toEqual(loadUsers.success(body));
});

test('when add user confirmed then posts user to api', async () => {
    requestor.setupPost('/api/users', JSON.stringify({username: 'jack', wasSuccessful: true}), 201);
    
    sagaTester.dispatch(addUser.confirm({username: 'jack', password: 'philly'}));
    
    const action = await sagaTester.waitFor(addUser.success.type);
    
    expect(action).toEqual(addUser.success({username: 'jack', wasSuccessful: true}));
})