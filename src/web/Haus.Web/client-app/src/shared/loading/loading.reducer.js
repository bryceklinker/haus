import {createSelector} from 'reselect';
import {removeActionPostFix} from '../state/actions';

const initialState = {};

const ASYNC_ACTION_CONVENTION_REGEX = /(.*)\s(Request|Success|Failed)/;

export function loadingReducer(state = initialState, action) {
    const values = extractAsyncLoadingValues(action.type);
    if (values) {
        return {
            ...state,
            [values.requestName]: values.requestState === 'Request'
        }
    }
    
    return state;
}

const selectLoadingState = state => state.loading;
export const selectIsLoading = createSelector(
    selectLoadingState,
    (_, actionCreator) => actionCreator.type,
    (loadingState, type) => {
        const requestName = removeActionPostFix(type);
        return loadingState[requestName];
    }
)

function extractAsyncLoadingValues(type) {
    const matches = ASYNC_ACTION_CONVENTION_REGEX.exec(type);
    if (matches) {
        const [, requestName, requestState] = matches;
        return {requestName, requestState};
    }

    return null;
}