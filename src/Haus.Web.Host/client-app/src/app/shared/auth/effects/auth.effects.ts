import {Injectable, Injector} from "@angular/core";
import {AuthService} from "@auth0/auth0-angular";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {map, mergeMap} from "rxjs/operators";

import {AuthActions} from "../actions";
import {ShellActions} from "../../../shell/state";
import {SharedActions} from "../../actions";

@Injectable({
  providedIn: 'root'
})
export class AuthEffects {
  isAuthLoading$ = createEffect(() => this.actions$.pipe(
    ofType(SharedActions.initApp),
    map(() => this.injector.get(AuthService)),
    mergeMap(auth => auth.isLoading$),
    map(isLoading => AuthActions.isLoading(isLoading))
  ))
  userLoggedIn$ = createEffect(() => this.actions$.pipe(
    ofType(SharedActions.initApp),
    map(() => this.injector.get(AuthService)),
    mergeMap(auth => auth.user$),
    map(user => {
      return user
        ? AuthActions.userLoggedIn(user)
        : AuthActions.userLoggedOut();
    })
  ));

  logout$ = createEffect(() => this.actions$.pipe(
    ofType(AuthActions.logout),
    map(() => this.injector.get(AuthService)),
    map(auth => auth.logout())
  ), {dispatch: false});

  constructor(private readonly actions$: Actions,
              private readonly injector: Injector) {
  }
}
