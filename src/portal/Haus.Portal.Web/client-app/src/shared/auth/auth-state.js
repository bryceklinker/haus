import {createUserManager} from "./oidc-auth";

const createAction = (type) => {
    const creator = (payload = undefined) => ({type, payload})
    creator.type = type;
    return creator;
};

export const loadUserRequest = createAction('LOAD_USER_REQUEST');
export const loadUserSuccess = createAction('LOAD_USER_SUCCESS');
export const loadUserFailed = createAction('LOAD_USER_FAILED');

export function createInitialState(settings) {
    return {
        isLoading: false,
        settings: settings,
        user: null,
        userManager: createUserManager(settings)
    };
}

export function authReducer(state, action) {
    switch (action.type) {
        case loadUserRequest.type:
            return {...state, isLoading: true, user: null};
        case loadUserSuccess.type:
            return {...state, isLoading: false, user: action.payload};
        case loadUserFailed.type:
            return {...state, user: null, error: action.payload};
        default:
            return state;
    }
}