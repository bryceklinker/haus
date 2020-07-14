import {takeEvery, call, apply, put} from '@redux-saga/core/effects';
import {addUser, loadUsers} from './users.actions';
import {getRequestor} from '../../shared/state/sagas';


function* loadUsersSaga() {
    const requestor = yield getRequestor();
    const response = yield call(requestor, '/api/users');
    const result = yield apply(response, response.json);
    yield put(loadUsers.success(result));
}

function* addUserSaga(action) {
    const request = {url: '/api/users', method: 'POST', body: JSON.stringify(action.payload) };
    
    const requestor = yield getRequestor();
    const response = yield call(requestor, request);
    const result = yield apply(response, response.json);
    yield put(addUser.success(result));
}

export function* usersSaga() {
    yield takeEvery(loadUsers.request.type, loadUsersSaga);
    yield takeEvery(addUser.confirm.type, addUserSaga);
}