import {AuthClientConfig, AuthService} from '@auth0/auth0-angular';
import {Injectable, Injector} from '@angular/core';
import {Observable, of} from 'rxjs';
import {SettingsService} from '../../settings';
import {ClientSettingsModel} from '../../models';
import {map, switchMap} from 'rxjs/operators';
import {UserModel} from '../user.model';
import {GetTokenSilentlyVerboseResponse, RedirectLoginOptions} from '@auth0/auth0-spa-js';

@Injectable({
  providedIn: 'root'
})
export class HausAuthService {
  private get authService$(): Observable<AuthService> {
    return this.settingsService.settings$.pipe(
      map(() => this.createAuthService())
    );
  }

  get token$(): Observable<string> {
    return this.authService$.pipe(
      switchMap(service => HausAuthService.getAccessToken(service))
    );
  }

  get isAuthenticated$(): Observable<boolean> {
    return this.authService$.pipe(
      switchMap(service => HausAuthService.checkIfAuthenticated(service))
    );
  }

  get isLoading$(): Observable<boolean> {
    return this.authService$.pipe(
      switchMap(service => HausAuthService.checkIfLoading(service))
    );
  }

  get user$(): Observable<UserModel> {
    return this.authService$.pipe(
      switchMap(service => service.user$),
      map(user => user as UserModel)
    )
  }

  private static get cypressAuth(): {body: GetTokenSilentlyVerboseResponse} | null {
    if (!Boolean((window as any).Cypress)) {
      return null;
    }
    const item = localStorage.getItem('auth0-cypress');
    return item && JSON.parse(item);
  }

  constructor(private readonly settingsService: SettingsService,
              private readonly injector: Injector) {
  }

  loginWithRedirect(options: RedirectLoginOptions): Observable<void> {
    return this.authService$.pipe(
      switchMap(service => service.loginWithRedirect(options))
    )
  }

  logout() {
    return this.authService$.pipe(
      map(service => service.logout())
    )
  }

  private static checkIfAuthenticated(authService: AuthService): Observable<boolean> {
    return HausAuthService.cypressAuth
      ? of(true)
      : authService.isAuthenticated$;
  }

  private static getAccessToken(authService: AuthService): Observable<string> {
    const cypressAuth = HausAuthService.cypressAuth;
    if (cypressAuth) {
      return of(cypressAuth.body.access_token);
    }
    return authService.getAccessTokenSilently();
  }

  private static checkIfLoading(authService: AuthService): Observable<boolean> {
    return HausAuthService.cypressAuth
      ? of(true)
      : authService.isLoading$;
  }

  private createAuthService(): AuthService {
    return this.injector.get(AuthService);
  }

  static initialize({auth}: ClientSettingsModel, config: AuthClientConfig): void {
    config.set({
      ...auth,
      useRefreshTokens: true,
      redirectUri: window.location.origin,
      httpInterceptor: {
        allowedList: [
          '/api/*',
          'api/*'
        ]
      }
    });
  }
}
