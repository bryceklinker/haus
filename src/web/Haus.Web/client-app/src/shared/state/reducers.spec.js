import {initAction} from '../../testing/testing.actions';
import {createReducer, handleAction} from './reducers';
import {createAction} from './actions';

describe('reducers', () => {
    test('when reducer executed then initial state is returned', () => {
        const reducer = createReducer({items: [{id: 7}]});

        expect(reducer(undefined, initAction())).toEqual({items: [{id: 7}]});
    });
    
    test('when reducer is setup to handle action then new state is returned', () => {
        const actionCreator = createAction('blah');
        
        const reducer = createReducer(
            {},
            handleAction(actionCreator, (state, a) => ({...state, id: 7}))
        );
        
        expect(reducer(undefined, actionCreator())).toEqual({id: 7});
    });
});