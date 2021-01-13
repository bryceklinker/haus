import {Injectable} from "@angular/core";
import {Route} from "@angular/router";
import {BehaviorSubject, Observable} from "rxjs";
import {map} from "rxjs/operators";

import {NavigationLinkModel} from "../models";
import {MAIN_ROUTE} from "../../app-routes";
import {toTitleCase} from "../../shared/humanize";

@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  private readonly childrenSubject = new BehaviorSubject(MAIN_ROUTE.children || [])

  get links$(): Observable<Array<NavigationLinkModel>> {
    return this.childrenSubject.asObservable().pipe(
      map(children => NavigationService.createNavigationLinks(children))
    )
  }

  private static createNavigationLinks(routes: Array<Route>): Array<NavigationLinkModel> {
    return routes
      .filter(r => !!r.path)
      .map(route => NavigationService.createNavigationLink(route as any));
  }

  private static createNavigationLink(route: Route & {path: string}): NavigationLinkModel {
    return {
      name: toTitleCase(route.path),
      path: route.path
    }
  }
}
