import {Actions, createEffect, ofType} from "@ngrx/effects";
import {Injectable} from "@angular/core";
import {RouterActions} from "../actions";
import {map} from "rxjs/operators";
import {Router} from "@angular/router";

@Injectable()
export class RouterEffects {
  navigate$ = createEffect(() => this.actions$.pipe(
    ofType(RouterActions.navigate),
    map(({payload}) => this.router.navigateByUrl(payload))
  ), {dispatch: false})

  constructor(private actions$: Actions, private router: Router) {
  }
}
