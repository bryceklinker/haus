import {TestModuleMetadata} from "@angular/core/testing";
import {ActivatedRoute, Routes} from "@angular/router";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {RouterTestingModule} from "@angular/router/testing";
import {AuthModule, AuthService} from "@auth0/auth0-angular";
import {SpyLocation} from "@angular/common/testing";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";

import {TestingAuthService, TestingSignalrConnectionServiceFactory} from "./fakes";
import {TestingMatDialog} from "./fakes/testing-mat-dialog";
import {TestingMatDialogRef} from "./fakes/testing-mat-dialog-ref";
import {SignalrHubConnectionFactory} from "../app/shared/signalr";
import {TestingActivatedRoute} from "./fakes/testing-activated-route";
import {HttpClientModule} from "@angular/common/http";

export interface TestModuleOptions extends TestModuleMetadata {
  routes?: Routes;
}

export function createTestingModule({
                                      routes = [],
                                      ...rest
                                    }: TestModuleOptions = {}) {
  return {
    ...rest,
    imports: [
      ...getTestingImports(routes),
      ...(rest.imports || [])
    ],
    providers: [
      ...getTestingProviders(),
      ...(rest.providers || [])
    ],
    routes
  }
}

export function getTestingImports(routes: Routes) {
  return [
    HttpClientModule,
    NoopAnimationsModule,
    RouterTestingModule.withRoutes(routes),
    AuthModule.forRoot(),
  ];
}

export function getTestingProviders() {
  return [
    {provide: Location, useFactory: () => new SpyLocation()},
    {provide: AuthService, useClass: TestingAuthService},
    {provide: MatDialog, useClass: TestingMatDialog},
    {provide: MatDialogRef, useClass: TestingMatDialogRef},
    {provide: SignalrHubConnectionFactory, useClass: TestingSignalrConnectionServiceFactory},
    {provide: ActivatedRoute, useClass: TestingActivatedRoute}
  ]
}
