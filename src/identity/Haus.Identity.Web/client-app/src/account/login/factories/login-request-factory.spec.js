import {createLoginRequest} from "./login-request-factory";

test('when login request created then returns object with password grant type', () => {
    const request = createLoginRequest();
    expect(request.get('grant_type')).toEqual('password');
});

test('when login request created then returns credentials in form', () => {
    const credentials = {
        username: 'bob', 
        password: 'passy', 
        clientId: 'mustard', 
        redirectUrl: 'https://something.com',
        scope: 'scoped'
    };
    const request = createLoginRequest(credentials);
    
    expect(request.get('username')).toEqual('bob');
    expect(request.get('password')).toEqual('passy');
    expect(request.get('client_id')).toEqual('mustard');
    expect(request.get('redirect_url')).toEqual('https://something.com');
    expect(request.get('scope')).toEqual('scoped');
});