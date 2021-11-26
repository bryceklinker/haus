import {WelcomeHarness} from './welcome.harness';
import {waitFor} from '@testing-library/angular';
import {AuthActions} from '../../../shared/auth';

describe('WelcomeComponent', () => {
  test('when displayed then shows welcome message', async () => {
    const harness = await WelcomeHarness.render(
      AuthActions.userLoggedIn({name: 'Bob', email: '', picture: ''})
    );

    await waitFor(() => expect(harness.header).toHaveTextContent('Bob'));
  });
});
