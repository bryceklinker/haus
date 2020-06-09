import React from 'react';
import { render } from '@testing-library/react';
import {MemoryRouter} from "react-router";

import {Shell} from "./Shell";

test('when rendered then login is shown', async () => {
    const { getByTestId } = await renderWithRouter();
    
    expect(getByTestId('username-input')).toBeVisible();
})

function renderWithRouter(currentRoute = '/') {
    return render(
        <MemoryRouter initialEntries={[currentRoute]}>
            <Shell />
        </MemoryRouter>
    )
}