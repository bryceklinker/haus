import {createAppTestingService, eventually} from '../../../../testing';
import {HausAuthService} from './haus-auth.service';

describe('HausAuthService', () => {
  test('when no cypress authentication used then returns token from auth service', async () => {
    let actual: string | null = null;
    const {service, authService} = createAppTestingService(HausAuthService);

    service.token$.subscribe(token => actual = token);
    authService.setAccessToken('one.token.to.rule.them');

    await eventually(() => expect(actual).toEqual('one.token.to.rule.them'));
  });

  test('when cypress authentication is used then returns token from cypress', async () => {
    let actual: string | null = null;
    setupCypressAuthentication('one.token.to.bind.them');

    const {service} = createAppTestingService(HausAuthService);
    service.token$.subscribe(token => actual = token);

    await eventually(() => expect(actual).toEqual('one.token.to.bind.them'));
  });

  xtest('when no cypress authentication then returns authenticated from auth service', async () => {
    const {service, authService} = createAppTestingService(HausAuthService);
    let actual = false;

    service.isAuthenticated$.subscribe(isAuthenticated => actual = isAuthenticated);
    authService.setIsAuthenticated(true);

    await eventually(() => expect(actual).toEqual(true));
  });

  test('when cypress authentication is used then returns authenticated', async () => {
    setupCypressAuthentication('idk');
    let actual = false;

    const {service} = createAppTestingService(HausAuthService);
    service.isAuthenticated$.subscribe(isAuthenticated => actual = isAuthenticated);

    await eventually(() => expect(actual).toEqual(true));
  });

  test('when no cypress authentication then returns loading from auth service', async () => {
    const {service, authService} = createAppTestingService(HausAuthService);
    let actual = false;

    service.isLoading$.subscribe(isLoading => actual = isLoading);
    authService.setIsLoading(true);

    await eventually(() => expect(actual).toEqual(true));
  });

  xtest('when cypress authentication is used then returns done loading', async () => {
    setupCypressAuthentication('idk');
    let actual = true;

    const {service} = createAppTestingService(HausAuthService);
    service.isLoading$.subscribe(isLoading => actual = isLoading);

    await eventually(() => expect(actual).toEqual(false));
  });

  afterEach(() => {
    (window as any).Cypress = null;
    localStorage.clear();
  });

  function setupCypressAuthentication(accessToken: string) {
    (window as any).Cypress = {};
    localStorage.setItem('auth0-cypress', JSON.stringify({
      body: {
        access_token: accessToken
      },
    }));
  }
});
