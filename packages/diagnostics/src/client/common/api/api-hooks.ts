import axios from 'axios';
import {useCallback, useReducer} from 'react';
import {Action, createAction} from '../actions';

type ApiState = {
    isLoading: boolean;
    data: any;
    error: any;
}

const request = createAction('[Api] Request');
const success = createAction('[Api] Success');
const failed = createAction('[Api] Failed');

const initialApiState: ApiState = {
    isLoading: false,
    data: null,
    error: null
};

function reducer(state: ApiState, action: Action) {
    switch (action.type) {
        case request.type:
            return {...state, isLoading: true, data: null, error: null};
        case success.type:
            return {...state, isLoading: false, data: action.payload, error: null};
        case failed.type:
            return {...state, isLoading: false, data: null, error: action.payload};
        default:
            return state;
    }
}

export function useApiPost(url: string) {
    const [state, dispatch] = useReducer(reducer, initialApiState);
    const execute = useCallback(async (body) => {
        try {
            dispatch(request());
            const response = await axios.post(url, body);
            if (response.status > 199 && response.status < 300) {
                dispatch(success(response.data));
                return true;
            } else {
                dispatch(failed({status: response.status, text: response.statusText}));
                return false;
            }
        } catch (err) {
            dispatch(failed(err));
            return false;
        }
    }, [url]);

    return {
        ...state,
        execute
    };
}
