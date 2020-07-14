import {createAction, createAsyncActions, createModalActions} from './actions';

test('when creating action with no props then returns creator that creates actions with correct type', () => {
    const proplessAction = createAction('hello');

    expect(proplessAction()).toEqual({type: 'hello'});
});

test('when creating action then action creator type is set', () => {
    const actionCreator = createAction('bill');
    
    expect(actionCreator.type).toEqual('bill');
});

test('when creating action with one prop then returns action creator that creates action with assigned property', () => {
    const creator = createAction('jack');

    expect(creator({id: 65})).toEqual({
        type: 'jack',
        payload: {
            id: 65
        }
    });
});

test('when creating async actions then returns three action creators', () => {
    const creators = createAsyncActions('load');

    expect(creators.request()).toEqual({ type: 'load Request' });
    expect(creators.success()).toEqual({type: 'load Success'});
    expect(creators.failed()).toEqual({type: 'load Failed'});
});

test('when creating async actions then each creator type is set', () => {
    const creators = createAsyncActions('stuff');
    
    expect(creators.request.type).toEqual('stuff Request');
    expect(creators.success.type).toEqual('stuff Success');
    expect(creators.failed.type).toEqual('stuff Failed');
})

test('when creating async actions then each async action allows payload', () => {
    const creators = createAsyncActions('load');

    expect(creators.request({id: 65})).toEqual({
        type: 'load Request',
        payload: {id: 65}
    });

    expect(creators.success({name: 'bob'})).toEqual({
        type: 'load Success',
        payload: {name: 'bob'}
    });

    expect(creators.failed({error: 'nope'})).toEqual({
        type: 'load Failed',
        payload: {error: 'nope'}
    });
});

test('when creating modal actions then returns open, cancel, confirm, request, failed and success creators', () => {
    const creators = createModalActions('hello');
    
    expect(creators.open.type).toEqual('hello Open');
    expect(creators.cancel.type).toEqual('hello Cancelled');
    expect(creators.confirm.type).toEqual('hello Confirmed');
    expect(creators.request.type).toEqual('hello Request');
    expect(creators.success.type).toEqual('hello Success');
    expect(creators.failed.type).toEqual('hello Failed');
})