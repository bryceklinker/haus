import {Injectable} from "@angular/core";
import {AuthGuard} from "@auth0/auth0-angular";
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  CanLoad,
  Route,
  RouterStateSnapshot,
  UrlSegment
} from "@angular/router";
import {Observable} from "rxjs";
import {HausAuthService} from './services';
import {take, tap} from 'rxjs/operators';

@Injectable()
export class UserRequiredGuard implements CanActivate, CanLoad, CanActivateChild{
  constructor(private readonly authService: HausAuthService) {

  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.redirectIfUnauthenticated(state);
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.redirectIfUnauthenticated(state);
  }

  canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> {
    return this.authService.isAuthenticated$.pipe(take(1));
  }

  private redirectIfUnauthenticated(state: RouterStateSnapshot): Observable<boolean> {
    return this.authService.isAuthenticated$.pipe(
      tap(isAuthenticated => {
        if (isAuthenticated) {
          return;
        }

        this.authService.loginWithRedirect({
          appState: {target: state.url}
        })
      })
    )
  }
}
