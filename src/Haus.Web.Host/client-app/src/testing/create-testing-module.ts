import {TestModuleMetadata} from "@angular/core/testing";
import {ActivatedRoute, Routes} from "@angular/router";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {RouterTestingModule} from "@angular/router/testing";
import {AuthModule, AuthService} from "@auth0/auth0-angular";
import {SpyLocation} from "@angular/common/testing";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";

import {
  TestingActionsSubject,
  TestingAuthService, TestingSaveFileService,
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
import {Action, ActionsSubject, Store, StoreModule} from "@ngrx/store";
import {EffectsModule} from "@ngrx/effects";
import {generateAppStateFromActions} from "./app-state-generator";
import {TestingStore} from "./fakes/testing-store";
import {appReducerMap} from "../app/app-reducer-map";
import {AppState} from "../app/app.state";
import {APP_EFFECTS} from "../app/app-effects";
import {Provider} from "@angular/core";
import {SaveFileService} from "../app/shared/save-file.service";
import {MatSnackBar} from "@angular/material/snack-bar";
import {TestingSnackBar} from "./fakes/testing-snack-bar";

export interface TestModuleOptions extends TestModuleMetadata {
  routes?: Routes;
  actions?: Action[];
  dialogData?: any
}

export function createTestingModule({
                                      routes = [],
                                      actions = [],
                                      dialogData = undefined,
                                      ...rest
                                    }: TestModuleOptions = {}) {
  return {
    ...rest,
    imports: [
      ...getTestingImports(routes, actions),
      ...(rest.imports || [])
    ],
    providers: [
      ...getTestingProviders(dialogData),
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

export function getTestingImports(routes: Routes, actions: Action[]) {
  const initialState = actions.length > 0 ? generateAppStateFromActions(...actions) : undefined
  return [
    HttpClientModule,
    NoopAnimationsModule,
    RouterTestingModule.withRoutes(routes),
    StoreModule.forRoot<AppState>(appReducerMap, {initialState}),
    EffectsModule.forRoot(APP_EFFECTS),
    AuthModule
  ];
}

export function getTestingProviders(dialogData: any) {
  const providers: Array<Provider> = [
    {provide: Location, useFactory: () => new SpyLocation()},
    {provide: AuthService, useClass: TestingAuthService},
    {provide: MatDialog, useClass: TestingMatDialog},
    {provide: MatDialogRef, useClass: TestingMatDialogRef},
    {provide: SignalrHubConnectionFactory, useClass: TestingSignalrHubConnectionFactory},
    {provide: ActivatedRoute, useClass: TestingActivatedRoute},
    {provide: SettingsService, useClass: TestingSettingsService},
    {provide: ActionsSubject, useClass: TestingActionsSubject},
    {provide: Store, useClass: TestingStore},
    {provide: SaveFileService, useClass: TestingSaveFileService},
    {provide: MatSnackBar, useClass: TestingSnackBar}
  ];

  if (dialogData) {
    providers.push({provide: MAT_DIALOG_DATA, useValue: dialogData});
  }

  return providers
}
