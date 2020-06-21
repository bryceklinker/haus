export function createAction(type) {
    const creator = (payload) => ({ type, payload });
    creator.type = type;
    return creator;
}

export function createAsyncActions(baseType) {
    return {
        request: createAction(`${baseType} Request`),
        success: createAction(`${baseType} Success`),
        failed: createAction(`${baseType} Failed`)
    }
}