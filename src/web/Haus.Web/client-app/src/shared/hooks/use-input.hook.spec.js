import React from 'react';
import {render, fireEvent} from '@testing-library/react';
import {useInput} from './use-input.hook';
import {TextInput} from '../views/TextInput';

function StubForm() {
    const [value, handler] = useInput('three');

    return (
        <TextInput data-testid={'input'} 
                   value={value}
                    onChange={handler}/>
    );
}

test('when using input then initial value is used first', () => {
    const {getByTestId} = render(<StubForm />);
    
    expect(getByTestId('input')).toHaveValue('three');
})

test('when using input then returns value', () => {
    const {getByTestId} = render(<StubForm/>);

    fireEvent.change(getByTestId('input'), {target: {value: 'jack'}});
    
    expect(getByTestId('input')).toHaveValue('jack');
});