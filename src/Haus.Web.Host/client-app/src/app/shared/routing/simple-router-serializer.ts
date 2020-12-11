import {RouterStateSerializer} from "@ngrx/router-store";
import {RouterUrlState} from "./router-url.state";
import {RouterStateSnapshot} from "@angular/router";

export class SimpleRouterSerializer implements RouterStateSerializer<RouterUrlState> {
  serialize(routerState: RouterStateSnapshot): RouterUrlState {
    let route = routerState.root;
    while (route.firstChild) {
      route = route.firstChild;
    }
    const {
      url,
      root: {queryParams}
    } = routerState;
    const {params} = route;

    return {url, params, queryParams};
  }

}
