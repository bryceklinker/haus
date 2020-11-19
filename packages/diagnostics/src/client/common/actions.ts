export interface Action<TPayload = any> {
    type: string;
    payload?: TPayload;
}

export interface ActionCreator<TPayload = any> {
    type: string;
    (payload?: TPayload): Action<TPayload>;
}

export function createAction<TPayload = any>(type: string): ActionCreator<TPayload> {
    const creator = (payload?: TPayload) => ({type, payload});
    creator.type = type;
    return creator;
}
