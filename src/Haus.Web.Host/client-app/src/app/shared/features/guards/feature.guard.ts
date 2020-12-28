import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, RouterStateSnapshot, UrlTree} from "@angular/router";
import {Observable, of} from "rxjs";
import {Injectable} from "@angular/core";
import {FeaturesService} from "../services/features.service";
import {map} from "rxjs/operators";

@Injectable()
export class FeatureGuard implements CanActivate, CanActivateChild {
  constructor(private service: FeaturesService) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    this.service.load().subscribe();
    return this.service.enabledFeatures$.pipe(
      map(features => features.includes((route as any).featureName))
    );
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return of(false);
  }

}
