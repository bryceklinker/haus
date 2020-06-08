import React from 'react';
import {render, fireEvent} from '@testing-library/react';
import {LoginView} from "./LoginView";

test('when user logs in using button then triggers login', async () => {
    const onLogin = jest.fn();
    const {getByTestId} = render(<LoginView onLogin={onLogin} />);
    
    fireEvent.change(getByTestId('username-input'), { target: { value: 'nameo' }});
    fireEvent.change(getByTestId('password-input'), { target: { value: 'passy' }});
    fireEvent.click(getByTestId('login-button'));
    
    expect(onLogin).toHaveBeenCalledWith({ username: 'nameo', password: 'passy' });
})

test('when user logs in using form submission then triggers login', async () => {
    const onLogin = jest.fn();
    const {getByTestId} = render(<LoginView onLogin={onLogin} />);

    fireEvent.change(getByTestId('username-input'), { target: { value: 'nameo' }});
    fireEvent.change(getByTestId('password-input'), { target: { value: 'passy' }});
    fireEvent.submit(getByTestId('login-form'));

    expect(onLogin).toHaveBeenCalledWith({ username: 'nameo', password: 'passy' });
})