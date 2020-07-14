import React from 'react';
import {render} from '@testing-library/react';
import {TextInput} from './TextInput';

test('when rendered then test id is on input', () => {
    const {container} = render(<TextInput data-testid={'one'} />);
    
    expect(container.querySelectorAll('input[data-testid=one]')).toHaveLength(1);
});