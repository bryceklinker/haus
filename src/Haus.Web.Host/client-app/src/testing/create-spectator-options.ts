import {SpectatorOptions, SpectatorServiceOptions} from "@ngneat/spectator";
import {Action, Store} from "@ngrx/store";
import {createTestingState} from "./create-testing-state";
import {provideMockStore} from "@ngrx/store/testing";
import {TestingStore} from "./testing-store";
import {Type} from "@angular/core";
import {BaseSpectatorOptions} from "@ngneat/spectator/lib/base/options";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {SharedModule} from "../app/shared/shared.module";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {RouterTestingModule} from "@angular/router/testing";
import {ReplaySubject, Subject} from "rxjs";
import {provideMockActions} from "@ngrx/effects/testing";
import {AppState} from "../app/app.state";
import {Routes} from "@angular/router";
import {AuthService} from "@auth0/auth0-angular";
import {createTestingAuthService} from "./testing-auth-service";
import {SignalREffects} from "ngrx-signalr-core";
import {EffectsModule} from "@ngrx/effects";

export interface BaseOptions extends BaseSpectatorOptions {
  actions$?: Subject<Action>;
  state?: AppState;
  routes?: Routes;
}

export interface ComponentOptions<T> extends SpectatorOptions<T>, BaseOptions {
  props?: Partial<T>;
}

export interface ServiceOptions<T> extends SpectatorServiceOptions<T>, BaseOptions {
}


export function createSpectatorOptions(options: Partial<BaseOptions>): BaseOptions {
  const state = options.state || createTestingState();
  const actions$ = options.actions$ || new ReplaySubject<Action>();
  const routes = options.routes || [];
  return {
    ...options,
    imports: [
      ...(options.imports || []),
      NoopAnimationsModule,
      SharedModule,
      HttpClientTestingModule,
      RouterTestingModule.withRoutes(routes),
      EffectsModule.forRoot([SignalREffects])
    ],
    providers: [
      ...(options.providers || []),
      ...provideMockStore({initialState: state}),
      provideMockActions(() => actions$.asObservable()),
      {provide: Store, useClass: TestingStore},
      {provide: AuthService, useFactory: createTestingAuthService}
    ],
    state,
    actions$,
    routes
  }
}

export function createServiceOptions<T>(service: Type<T>, module: any, options?: Partial<ServiceOptions<T>>) {
  const baseOptions = createSpectatorOptions({...(options || {})});
  return {
    ...baseOptions,
    service,
    imports: [
      ...(baseOptions.imports || []),
      module
    ]
  }
}

export function createComponentOptions<T>(options: ComponentOptions<T>): ComponentOptions<T> {
  const baseOptions = createSpectatorOptions(options);
  return {
    ...baseOptions,
    imports: [
      ...(baseOptions.imports || []),
      ...(options.imports || [])
    ],
    declarations: [
      ...(baseOptions.declarations || []),
      ...(options.declarations || [])
    ],
    component: options.component
  }
}
