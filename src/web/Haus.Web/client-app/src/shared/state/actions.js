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

export function createModalActions(baseType) {
    return {
        ...createAsyncActions(baseType),
        open: createAction(`${baseType} Open`),
        confirm: createAction(`${baseType} Confirmed`),
        cancel: createAction(`${baseType} Cancelled`),
    }
}

export function removeActionPostFix(type) {
    return type
        .replace(' Request', '')
        .replace(' Success', '')
        .replace(' Failed', '')
        .replace(' Open', '')
        .replace(' Confirmed', '')
        .replace(' Cancelled', '');
}