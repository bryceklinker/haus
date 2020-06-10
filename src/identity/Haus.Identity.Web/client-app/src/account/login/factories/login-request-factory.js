export function createLoginRequest({username, password, clientId, redirectUrl, scope} = {}) {
    const formData = new FormData();
    formData.append('grant_type', 'password');
    formData.append('username', username);
    formData.append('password', password);
    formData.append('client_id', clientId);
    formData.append('redirect_url', redirectUrl);
    formData.append('scope', scope);
    return formData;
}