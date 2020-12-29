import {TestModuleMetadata} from "@angular/core/testing";
import {ActivatedRoute, Routes} from "@angular/router";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {RouterTestingModule} from "@angular/router/testing";
import {AuthModule, AuthService} from "@auth0/auth0-angular";
import {SpyLocation} from "@angular/common/testing";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";

import {
  TestingActionsSubject,
  TestingAuthService,
  TestingSettingsService,
  TestingSignalrHubConnectionFactory
} from "./fakes";
import {TestingMatDialog} from "./fakes/testing-mat-dialog";
import {TestingMatDialogRef} from "./fakes/testing-mat-dialog-ref";
import {SignalrHubConnectionFactory} from "../app/shared/signalr";
import {TestingActivatedRoute} from "./fakes";
import {HttpClientModule} from "@angular/common/http";
import {SettingsService} from "../app/shared/settings";
import {SHELL_PROVIDERS} from "../app/shell/services";
import {SHELL_COMPONENTS} from "../app/shell/components";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../app/shared/shared.module";
import {Action, ActionsSubject, INITIAL_STATE, Store, StoreModule} from "@ngrx/store";
import {EffectsModule} from "@ngrx/effects";
import {generateAppStateFromActions} from "./app-state-generator";
import {TestingStore} from "./fakes/testing-store";

export interface TestModuleOptions extends TestModuleMetadata {
  routes?: Routes;
  actions?: Action[];
}

export function createTestingModule({
                                      routes = [],
                                      actions = [],
                                      ...rest
                                    }: TestModuleOptions = {}) {
  return {
    ...rest,
    imports: [
      ...getTestingImports(routes),
      ...(rest.imports || [])
    ],
    providers: [
      ...getTestingProviders(actions),
      ...(rest.providers || [])
    ],
    routes
  }
}

export function createAppTestingModule({
                                         imports = [],
                                         providers = [],
                                         declarations = [],
                                         ...rest
                                       }: TestModuleOptions = {}) {
  return createTestingModule({
    ...rest,
    imports: [
      CommonModule,
      SharedModule,
      ...imports,
    ],
    providers: [
      ...SHELL_PROVIDERS,
      ...providers,
    ],
    declarations: [
      ...SHELL_COMPONENTS,
      ...declarations
    ]
  });
}

export function getTestingImports(routes: Routes) {
  return [
    HttpClientModule,
    NoopAnimationsModule,
    RouterTestingModule.withRoutes(routes),
    AuthModule
  ];
}

export function getTestingProviders(actions: Action[]) {
  return [
    {provide: Location, useFactory: () => new SpyLocation()},
    {provide: AuthService, useClass: TestingAuthService},
    {provide: MatDialog, useClass: TestingMatDialog},
    {provide: MatDialogRef, useClass: TestingMatDialogRef},
    {provide: SignalrHubConnectionFactory, useClass: TestingSignalrHubConnectionFactory},
    {provide: ActivatedRoute, useClass: TestingActivatedRoute},
    {provide: SettingsService, useClass: TestingSettingsService},
    {provide: ActionsSubject, useClass: TestingActionsSubject},
    {provide: INITIAL_STATE, useValue: generateAppStateFromActions(...actions)},
    {provide: Store, useClass: TestingStore}
  ]
}
