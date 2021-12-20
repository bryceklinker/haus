import {AuthService} from '@auth0/auth0-angular';
import {BehaviorSubject, from, Observable, of} from 'rxjs';
import {Injectable} from '@angular/core';
import {map, skip} from 'rxjs/operators';
import {GetTokenSilentlyOptions, GetTokenSilentlyVerboseResponse} from '@auth0/auth0-spa-js';

interface AuthProperties {
  isLoading: boolean,
  isAuthenticated: boolean,
  user?: any,
  idTokenClaims?: any,
  error?: Error,
  accessToken?: string;
}

const INITIAL_AUTH_PROPERTIES: AuthProperties = {
  isLoading: false,
  isAuthenticated: false
};

@Injectable()
export class TestingAuthService extends AuthService {
  private properties = new BehaviorSubject<AuthProperties>(INITIAL_AUTH_PROPERTIES);

  isLoading$: Observable<boolean>;
  isAuthenticated$: Observable<boolean>;
  user$: Observable<any>;
  idTokenClaims$: Observable<import('@auth0/auth0-spa-js').IdToken>;
  error$: Observable<Error>;
  accessToken$: Observable<string>;

  constructor() {
    super(
      <any>{isAuthenticated: jest.fn().mockReturnValue(of(false))},
      <any>{get: jest.fn().mockReturnValue({})},
      <any>{path: jest.fn().mockReturnValue(''), navigateByUrl: jest.fn()},
      <any>{setError: jest.fn(), setIsLoading: jest.fn()}
    );

    this.isLoading$ = this.properties.pipe(map(p => p.isLoading));
    this.isAuthenticated$ = this.properties.pipe(map(p => p.isAuthenticated));
    this.accessToken$ = <any>this.properties.pipe(
      skip(1),
      map(p => p.accessToken)
    );
    this.user$ = this.properties.pipe(
      skip(1),
      map(p => p.user)
    );
    this.idTokenClaims$ = this.properties.pipe(
      skip(1),
      map(p => p.idTokenClaims),
    );
    this.error$ = <any>this.properties.pipe(
      skip(1),
      map(p => p.error)
    );

    jest.spyOn(this as TestingAuthService, 'loginWithRedirect');
    jest.spyOn(this as TestingAuthService, 'loginWithPopup');
    jest.spyOn(this as TestingAuthService, 'logout');
    jest.spyOn(this as TestingAuthService, 'getAccessTokenWithPopup');
    jest.spyOn(this as TestingAuthService, 'getAccessTokenSilently');
  }

  getAccessTokenSilently(options?: GetTokenSilentlyOptions): Observable<string>;
  getAccessTokenSilently(options?: GetTokenSilentlyOptions & { detailedResponse: true }): Observable<GetTokenSilentlyVerboseResponse>;
  getAccessTokenSilently(options?: GetTokenSilentlyOptions & { detailedResponse: true }): Observable<string | GetTokenSilentlyVerboseResponse> {
    if (options?.detailedResponse) {
      return this.accessToken$.pipe(
        map(token => ({
          access_token: token,
          id_token: token,
          expires_in: 100
        }))
      );
    } else {
      return this.accessToken$;
    }
  }

  ngOnDestroy() {
  }

  loginWithRedirect(options?: any): Observable<void> {
    return from(Promise.resolve());
  }

  loginWithPopup(options?: any, config?: any): Observable<void> {
    return from(Promise.resolve());
  }

  logout(options?: any) {
    this.properties.next(INITIAL_AUTH_PROPERTIES);
  }

  getAccessTokenWithPopup(options?: any): Observable<string> {
    return this.accessToken$;
  }

  setIsAuthenticated(value: boolean) {
    this.properties.next({
      ...this.properties.value,
      isAuthenticated: value,
    });
  }

  setAccessToken(token?: string) {
    this.properties.next({
      ...this.properties.value,
      accessToken: token
    });
  }

  setUser(user: any) {
    this.properties.next({
      ...this.properties.value,
      user
    });
  }

  setIsLoading(isLoading: boolean) {
    this.properties.next({
      ...this.properties.value,
      isLoading
    });
  }
}
