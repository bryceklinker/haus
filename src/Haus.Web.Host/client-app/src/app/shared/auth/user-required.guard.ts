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

@Injectable()
export class UserRequiredGuard implements CanActivate, CanLoad, CanActivateChild{
  constructor(private readonly authGuard: AuthGuard) {

  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.authGuard.canActivate(route, state);
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.authGuard.canActivateChild(childRoute, state);
  }

  canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> {
    return this.authGuard.canLoad(route, segments);
  }
}
