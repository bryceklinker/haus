import React from 'react';
import {render} from '@testing-library/react';
import {MemoryRouter} from "react-router";

import {Shell} from "./Shell";

test('when rendered then login is shown', async () => {
    const {getByTestId} = await renderWithRouter();

    expect(getByTestId('username-input')).toBeVisible();
});

test('when rendered to auth-callback then auth callback is shown', async () => {
    const {container} = await renderWithRouter('/auth-callback');

    expect(container).toHaveTextContent('Finishing login process');
});

function renderWithRouter(currentRoute = '/') {
    return render(
        <MemoryRouter initialEntries={[currentRoute]}>
            <Shell/>
        </MemoryRouter>
    )
}