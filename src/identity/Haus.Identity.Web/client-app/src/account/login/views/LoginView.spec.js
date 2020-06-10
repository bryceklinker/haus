import React from 'react';
import {fireEvent, render} from '@testing-library/react';
import {LoginView} from "./LoginView";

function enterUsernameAndPassword(getByTestId, username, password) {
    fireEvent.change(getByTestId('username-input'), {target: {value: username}});
    fireEvent.change(getByTestId('password-input'), {target: {value: password}});
}

function submitLoginForm(getByTestId, username, password) {
    enterUsernameAndPassword(getByTestId, username, password);
    fireEvent.submit(getByTestId('login-form'));
}

function clickLoginButton(getByTestId, username, password) {
    enterUsernameAndPassword(getByTestId, username, password);
    fireEvent.click(getByTestId('login-button'));
}

let onLogin = null;
beforeEach(() => {
    
    onLogin = jest.fn();
})

test('when user logs in using button then triggers login', async () => {
    const {getByTestId} = render(<LoginView onLogin={onLogin}/>);

    clickLoginButton(getByTestId, 'nameo', 'passy');

    expect(onLogin).toHaveBeenCalledWith({username: 'nameo', password: 'passy'});
})

test('when user logs in using form submission then triggers login', async () => {
    const {getByTestId} = render(<LoginView onLogin={onLogin}/>);

    submitLoginForm(getByTestId, 'nameo', 'passy');

    expect(onLogin).toHaveBeenCalledWith({username: 'nameo', password: 'passy'});
});

test('when redirect url is provided then form submitted with redirect url', async () => {
    const {getByTestId} = render(<LoginView onLogin={onLogin} redirectUrl={'https://localhost:5001'}/>);

    submitLoginForm(getByTestId, 'something', 'else');

    expect(onLogin).toHaveBeenCalledWith({
        username: 'something',
        password: 'else',
        redirectUrl: 'https://localhost:5001'
    });
});

test('when client id is provided then form submission includes client id', async () => {
    const {getByTestId} = render(<LoginView onLogin={onLogin} clientId={'bill'}/>);

    submitLoginForm(getByTestId, 'something', 'else');

    expect(onLogin).toHaveBeenCalledWith({
        username: 'something',
        password: 'else',
        clientId: 'bill'
    });
})

test('when scope is provided then form submission includes scope', async () => {
    const {getByTestId} = render(<LoginView onLogin={onLogin} scope={'scoped'}/>);

    submitLoginForm(getByTestId, 'something', 'else');

    expect(onLogin).toHaveBeenCalledWith({
        username: 'something',
        password: 'else',
        scope: 'scoped'
    });
})