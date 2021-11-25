import {
  createAppTestingService,
  eventually,
  ModelFactory,
  TestingActionsSubject,
  TestingAuthService
} from '../../../../testing';
import {AuthEffects} from './auth.effects';
import {AuthActions} from '../actions';
import {SharedActions} from '../../actions';

describe('AuthEffects', () => {
  let actions$: TestingActionsSubject;
  let testingAuthService: TestingAuthService;

  beforeEach(() => {
    const {actionsSubject, authService} = createAppTestingService(AuthEffects);
    actions$ = actionsSubject;
    testingAuthService = authService;

    actions$.next(SharedActions.initApp());
  });

  it('should notify user logged in when user logs in', async () => {
    const user = ModelFactory.createUser();

    testingAuthService.setUser(user);

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(AuthActions.userLoggedIn(user));
    });
  });

  it('should notify user logged out when user is no longer available', async () => {
    testingAuthService.setUser(null);

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(AuthActions.userLoggedOut());
    });
  });

  it('should trigger logout when log out action received', async () => {
    actions$.next(AuthActions.logout());

    await eventually(() => {
      expect(testingAuthService.logout).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(AuthActions.userLoggedOut());
    });
  });

  it('should be loading while auth is loading', async () => {
    testingAuthService.setIsLoading(true);

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(AuthActions.isLoading(true));
    });
  });

  it('should not be loading when auth is done loading', async () => {
    testingAuthService.setIsLoading(true);
    testingAuthService.setIsLoading(false);

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(AuthActions.isLoading(false));
    });
  });
});
