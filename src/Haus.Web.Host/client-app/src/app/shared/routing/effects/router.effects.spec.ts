import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {TestBed} from "@angular/core/testing";
import {Router} from "@angular/router";

import {RouterEffects} from "./router.effects";
import {createTestingEffect, eventually, TestingRouter} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {RouterActions} from "../actions";

describe('RouterEffects', () => {
  let effects: RouterEffects;
  let actions$: Subject<Action>;
  let router: TestingRouter;

  beforeEach(() => {
    const result = createTestingEffect(RouterEffects, {imports: [SharedModule]});
    effects = result.effects;
    actions$ = result.actions$;
    router = <TestingRouter>TestBed.inject(Router);
  })

  it('should navigate to url', async () => {
    actions$.next(RouterActions.navigate('/devices/65'));

    await eventually(() => {
      expect(router.navigateByUrl).toHaveBeenCalledWith('/devices/65');
    })
  })
})
