import {render, RenderComponentOptions, RenderResult} from '@testing-library/angular';
import {Type} from "@angular/core";
import {ActivatedRoute, Router, Routes} from "@angular/router";
import {TestBed} from "@angular/core/testing";
import {By} from "@angular/platform-browser";
import {HarnessLoader} from '@angular/cdk/testing';
import {TestbedHarnessEnvironment} from '@angular/cdk/testing/testbed'

import {createAppTestingModule, createTestingModule} from "./create-testing-module";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {TestingMatDialog} from "./fakes/testing-mat-dialog";
import {TestingMatDialogRef} from "./fakes/testing-mat-dialog-ref";
import {
  TestingSettingsService,
  TestingSignalrHubConnectionFactory,
  TestingActivatedRoute,
  TestingActionsSubject
} from "./fakes";
import {SignalrHubConnectionFactory} from "../app/shared/signalr";
import {SettingsService} from "../app/shared/settings";
import {Action, ActionsSubject, Store} from "@ngrx/store";
import {TestingStore} from "./fakes/testing-store";
import {AppState} from "../app/app.state";

export interface RenderAppComponentOptions<T> extends RenderComponentOptions<T> {
  routes?: Routes;
  actions?: Action[];
}

export interface RenderFeatureComponentOptions<T> extends RenderAppComponentOptions<T> {
  imports: Array<any>;
}

export interface RenderComponentResult<T> extends RenderResult<T> {
  triggerEventHandler: <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => void;
  router: Router;
  matDialog: TestingMatDialog;
  matDialogRef: TestingMatDialogRef;
  signalrConnectionFactory: TestingSignalrHubConnectionFactory;
  activatedRoute: TestingActivatedRoute;
  settingsService: TestingSettingsService;
  actionsSubject: TestingActionsSubject;
  store: TestingStore<AppState>;
  matHarness: HarnessLoader
}

export async function renderAppComponent<T>(component: Type<T>, options?: RenderAppComponentOptions<T>): Promise<RenderComponentResult<T>> {
  const appOptions = createAppTestingModule(options);
  return await renderComponent(component, appOptions);
}

export async function renderFeatureComponent<T>(component: Type<T>, options: RenderFeatureComponentOptions<T>) {
  const featureOptions = createTestingModule(options);
  return await renderComponent(component, featureOptions);
}

async function renderComponent<T>(component: Type<T>, options: RenderAppComponentOptions<T>): Promise<RenderComponentResult<T>> {
  const result = await render(component, {
    ...options,
    excludeComponentDeclaration: true
  });
  result.fixture.detectChanges();
  const matHarness = TestbedHarnessEnvironment.loader(result.fixture);
  return {
    ...result,
    triggerEventHandler: createEventTriggerHandler(result),
    router: TestBed.inject(Router),
    matDialog: TestBed.inject(MatDialog) as TestingMatDialog,
    matDialogRef: TestBed.inject(MatDialogRef) as TestingMatDialogRef,
    signalrConnectionFactory: TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrHubConnectionFactory,
    activatedRoute: TestBed.inject(ActivatedRoute) as TestingActivatedRoute,
    settingsService: TestBed.inject(SettingsService) as TestingSettingsService,
    actionsSubject: TestBed.inject(ActionsSubject) as TestingActionsSubject,
    store: TestBed.inject(Store) as TestingStore<AppState>,
    matHarness: matHarness
  };
}

function createEventTriggerHandler<TComponent, TDirective>({fixture}: RenderResult<TComponent>) {
  return <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => {
      fixture.debugElement.query(By.directive(directive)).triggerEventHandler(eventName, eventArg);
  }
}
