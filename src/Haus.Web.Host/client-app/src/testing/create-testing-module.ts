import {TestModuleMetadata} from "@angular/core/testing";
import {Routes} from "@angular/router";
import {ReplaySubject, Subject} from "rxjs";
import {Action, combineReducers, Store, StoreModule} from "@ngrx/store";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {RouterTestingModule} from "@angular/router/testing";
import {EffectsModule} from "@ngrx/effects";
import {SignalREffects} from "ngrx-signalr-core";
import {provideMockActions} from "@ngrx/effects/testing";
import {AuthModule, AuthService} from "@auth0/auth0-angular";
import {SpyLocation} from "@angular/common/testing";
import {EntityDataModule} from "@ngrx/data";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";

import {TestingStore, TestingAuthService} from "./fakes";
import {ENTITY_METADATA} from "../app/entity-metadata";
import {AppState} from "../app/app.state";
import {appReducerMap} from "../app/app-reducer-map";
import {TestingActions} from "./testing-actions";
import {TestingMatDialog} from "./fakes/testing-mat-dialog";
import {TestingMatDialogRef} from "./fakes/testing-mat-dialog-ref";

export interface TestModuleOptions extends TestModuleMetadata {
  routes?: Routes;
  actions$?: Subject<Action>;
  actions?: Action[];
}

export function createTestingModule({
                                      routes = [],
                                      actions$ = new ReplaySubject<Action>(),
                                      actions = [],
                                      ...rest
                                    }: TestModuleOptions = {}) {
  return {
    ...rest,
    imports: [
      ...getTestingImports(routes, actions),
      ...(rest.imports || [])
    ],
    providers: [
      ...getTestingProviders(actions$),
      ...(rest.providers || [])
    ],
    routes,
    actions$,
    actions
  }
}

export function getTestingImports(routes: Routes, actions: Action[]) {
  return [
    NoopAnimationsModule,
    RouterTestingModule.withRoutes(routes),
    EffectsModule.forRoot([SignalREffects]),
    StoreModule.forRoot(appReducerMap, {initialState: createAppStateIfSetRouterActionProvided(actions)}),
    AuthModule.forRoot(),
    EntityDataModule.forRoot(ENTITY_METADATA),
  ];
}

export function getTestingProviders(actions$: Subject<Action>) {
  return [
    {provide: Store, useClass: TestingStore},
    {provide: Location, useFactory: () => new SpyLocation()},
    {provide: AuthService, useClass: TestingAuthService},
    {provide: MatDialog, useClass: TestingMatDialog},
    {provide: MatDialogRef, useClass: TestingMatDialogRef}
  ]
}

function createAppStateIfSetRouterActionProvided(actions: Action[]): AppState | undefined {
  const setRouterStateActions = actions.filter(a => a.type == TestingActions.setRouterState.type);
  if (setRouterStateActions.length === 0) {
    return undefined;
  }

  const reducer = combineReducers<AppState>(appReducerMap);
  return setRouterStateActions.reduce((state, action) => ({
    ...state || {},
    router: {
      ...state.router,
      state: {
        url: (<ReturnType<typeof TestingActions.setRouterState>>action).payload.url || '',
        queryParams: (<ReturnType<typeof TestingActions.setRouterState>>action).payload.queryParams || {},
        params: (<ReturnType<typeof TestingActions.setRouterState>>action).payload.params || {}
      }

    }
  }), reducer(undefined, TestingActions.initAction))
}
