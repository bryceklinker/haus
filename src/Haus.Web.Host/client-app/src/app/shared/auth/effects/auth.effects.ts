import {Injectable} from "@angular/core";
import {AuthService} from "@auth0/auth0-angular";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {map} from "rxjs/operators";

import {AuthActions} from "../actions";

@Injectable()
export class AuthEffects {
  userLoggedIn$ = createEffect(() => this.auth.user$.pipe(
    map(user => {
      return user
        ? AuthActions.userLoggedIn(user)
        : AuthActions.userLoggedOut();
    })
  ));

  logout$ = createEffect(() => this.actions$.pipe(
    ofType(AuthActions.logout),
    map(() => this.auth.logout())
  ), {dispatch: false});

  constructor(private readonly auth: AuthService,
              private readonly actions$: Actions) {
  }
}
