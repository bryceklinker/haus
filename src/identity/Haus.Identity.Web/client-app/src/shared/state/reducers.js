export function createReducer(initialState, ...handlers) {
    return (state = initialState, action) => {
        const handler = handlers.find(h => h.type === action.type);
        return handler ? handler(state, action) : state;
    }
}

export function handleAction(actionCreator, handler) {
    handler.type = actionCreator.type;
    return handler;
}