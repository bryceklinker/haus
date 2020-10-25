export function createAction(type) {
    const creator = (payload = {}) => ({type, payload});
    creator.type = type;
    return creator;
}
