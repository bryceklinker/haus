import {Injectable} from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  CanLoad,
  Route,
  RouterStateSnapshot,
  UrlSegment
} from '@angular/router';
import {Observable, of} from 'rxjs';
import {HausAuthService} from './services';
import {map, switchMap, take} from 'rxjs/operators';

@Injectable()
export class UserRequiredGuard implements CanActivate, CanLoad, CanActivateChild {
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
      switchMap(isAuthenticated => {
        if (isAuthenticated) {
          return of(true);
        }

        return this.authService.loginWithRedirect({appState: {target: state.url}}).pipe(
          map(() => false)
        );
      })
    );
  }
}
