import React from 'react';
import {render, fireEvent} from '@testing-library/react';
import {createTestingStore} from './create-testing-store';
import {Provider} from 'react-redux';

export function renderWithStore(children, ...actions) {
    const store = createTestingStore(...actions); 
    const result = render(
        <Provider store={store}>
            {children}
        </Provider>
    );
    
    return {...result, fireEvent, store};
}