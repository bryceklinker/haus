import {AuthService} from "@auth0/auth0-angular";
import {BehaviorSubject, Observable, of} from "rxjs";
import {Injectable} from "@angular/core";
import {filter, map, skip} from "rxjs/operators";

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
}

@Injectable()
export class TestingAuthService extends AuthService {
  private properties = new BehaviorSubject<AuthProperties>(INITIAL_AUTH_PROPERTIES);

  isLoading$: Observable<boolean>;
  isAuthenticated$: Observable<boolean>;
  user$: Observable<any>;
  idTokenClaims$: Observable<import("@auth0/auth0-spa-js").IdToken>;
  error$: Observable<Error>;
  accessToken$: Observable<string>

  constructor() {
    super(
      <any>{isAuthenticated: jest.fn().mockReturnValue(of(false))},
      <any>{get: jest.fn().mockReturnValue({})},
      <any>{path: jest.fn().mockReturnValue('')},
      <any>{navigateByUrl: jest.fn()});
    this.isLoading$ = this.properties.pipe(map(p => p.isLoading));
    this.isAuthenticated$ = this.properties.pipe(map(p => p.isAuthenticated));
    this.accessToken$ = <any>this.properties.pipe(
      skip(1),
      map(p => p.accessToken)
    )
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

    spyOn(this, 'loginWithRedirect').and.callThrough();
    spyOn(this, 'loginWithPopup').and.callThrough();
    spyOn(this, 'logout').and.callThrough();
    spyOn(this, 'getAccessTokenWithPopup').and.callThrough();
    spyOn(this, 'getAccessTokenSilently').and.callThrough();
  }


  getAccessTokenSilently(options?: any): Observable<string> {
    return this.accessToken$;
  }

  ngOnDestroy() {
  }

  loginWithRedirect(options?: any): Observable<void> {
    return of();
  }

  loginWithPopup(options?: any, config?: any): Observable<void> {
    return of();
  }

  logout(options?: any) {
    this.properties.next(INITIAL_AUTH_PROPERTIES);
  }

  getAccessTokenWithPopup(options?: any): Observable<string> {
    return this.accessToken$;
  }

  setUser(user: any) {
    this.properties.next({
      ...this.properties.value,
      user
    })
  }

  setIsLoading(isLoading: boolean) {
    this.properties.next({
      ...this.properties.value,
      isLoading
    })
  }
}
