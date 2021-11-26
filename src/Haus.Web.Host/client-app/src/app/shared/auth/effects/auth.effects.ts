import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {map, mergeMap} from 'rxjs/operators';

import {AuthActions} from "../actions";
import {SharedActions} from "../../actions";
import {UserModel} from "../user.model";
import {HausAuthService} from '../services';

@Injectable({
  providedIn: 'root'
})
export class AuthEffects {
  isAuthLoading$ = createEffect(() => this.actions$.pipe(
    ofType(SharedActions.initApp),
    mergeMap(() => this.authService.isLoading$),
    map(isLoading => AuthActions.isLoading(isLoading))
  ))

  userLoggedIn$ = createEffect(() => this.actions$.pipe(
    ofType(SharedActions.initApp),
    mergeMap(() => this.authService.user$),
    map(user => {
      return user
        ? AuthActions.userLoggedIn(user as UserModel)
        : AuthActions.userLoggedOut();
    })
  ));

  logout$ = createEffect(() => this.actions$.pipe(
    ofType(AuthActions.logout),
    mergeMap(() => this.authService.logout())
  ), {dispatch: false});

  constructor(private readonly actions$: Actions,
              private readonly authService: HausAuthService) {
  }
}
