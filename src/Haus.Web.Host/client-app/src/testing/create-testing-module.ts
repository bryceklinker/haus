import {TestModuleMetadata} from "@angular/core/testing";
import {Routes} from "@angular/router";
import {ReplaySubject, Subject} from "rxjs";
import {Action, Store, StoreModule} from "@ngrx/store";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {RouterTestingModule} from "@angular/router/testing";
import {EffectsModule} from "@ngrx/effects";
import {SignalREffects} from "ngrx-signalr-core";
import {provideMockStore} from "@ngrx/store/testing";
import {provideMockActions} from "@ngrx/effects/testing";
import {AuthModule, AuthService} from "@auth0/auth0-angular";
import {SpyLocation} from "@angular/common/testing";

import {AppState} from "../app/app.state";
import {createTestingState} from "./create-testing-state";
import {TestingStore, TestingAuthService} from "./fakes";
import {routerReducer, StoreRouterConnectingModule} from "@ngrx/router-store";

export interface TestModuleOptions extends TestModuleMetadata {
  state?: AppState;
  routes?: Routes;
  actions$?: Subject<Action>
}

export function createTestingModule({routes = [], state = createTestingState(), actions$ = new ReplaySubject<Action>(), ...rest}: TestModuleOptions = {}) {
  return {
    ...rest,
    imports: [
      ...getTestingImports(routes),
      ...(rest.imports || [])
    ],
    providers: [
      ...getTestingProviders(state, actions$),
      ...(rest.providers || [])
    ],
    state,
    routes,
    actions$
  }
}

export function getTestingImports(routes: Routes) {
  return [
    NoopAnimationsModule,
    HttpClientTestingModule,
    RouterTestingModule.withRoutes(routes),
    EffectsModule.forRoot([SignalREffects]),
    StoreModule.forRoot({}),
    StoreRouterConnectingModule.forRoot(),
    AuthModule.forRoot()
  ];
}

export function getTestingProviders(state: AppState, actions$: Subject<Action>) {
  return [
    ...provideMockStore({initialState: state}),
    provideMockActions(() => actions$),
    {provide: Store, useClass: TestingStore},
    {provide: Location, useFactory: () => new SpyLocation()},
    {provide: AuthService, useClass: TestingAuthService}
  ]
}
