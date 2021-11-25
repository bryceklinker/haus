import {createAppTestingService} from '../../../../testing';
import {HausAuthService} from './haus-auth.service';

describe('HausAuthService', () => {
  test('when no cypress authentication used then returns token from auth service', done => {
    const {service, authService} = createAppTestingService(HausAuthService);
    service.token$.subscribe(token => {
      expect(token).toEqual('one.token.to.rule.them');
      done();
    });
    authService.setAccessToken('one.token.to.rule.them');
  });

  test('when cypress authentication is used then returns token from cypress', done => {
    setupCypressAuthentication('one.token.to.bind.them');

    const {service} = createAppTestingService(HausAuthService);
    service.token$.subscribe(token => {
      expect(token).toEqual('one.token.to.bind.them');
      done();
    });
  });

  xtest('when no cypress authentication then returns authenticated from auth service', done => {
    const {service, authService} = createAppTestingService(HausAuthService);

    service.isAuthenticated$.subscribe(isAuthenticated => {
      expect(isAuthenticated).toEqual(true);
      done();
    });
    authService.setIsAuthenticated(true);
  });

  test('when cypress authentication is used then returns authenticated', done => {
    setupCypressAuthentication('idk');

    const {service} = createAppTestingService(HausAuthService);
    service.isAuthenticated$.subscribe(isAuthenticated => {
      expect(isAuthenticated).toEqual(true);
      done();
    });
  })

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
