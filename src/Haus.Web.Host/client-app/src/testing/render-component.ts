import {render, RenderComponentOptions, RenderResult} from '@testing-library/angular';
import {Type} from "@angular/core";
import {ActivatedRoute, Router, Routes} from "@angular/router";
import {TestBed} from "@angular/core/testing";
import {By} from "@angular/platform-browser";

import {SharedModule} from "../app/shared/shared.module";
import {SHELL_COMPONENTS} from "../app/shell/components";
import {createTestingModule} from "./create-testing-module";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {TestingMatDialog} from "./fakes/testing-mat-dialog";
import {TestingMatDialogRef} from "./fakes/testing-mat-dialog-ref";
import {TestingSettingsService, TestingSignalrConnectionServiceFactory} from "./fakes";
import {SignalrHubConnectionFactory} from "../app/shared/signalr/signalr-hub-connection-factory.service";
import {TestingActivatedRoute} from "./fakes/testing-activated-route";
import {SettingsService} from "../app/shared/settings";

export interface RenderAppComponentOptions<T> extends RenderComponentOptions<T> {
  routes?: Routes;
}

export interface RenderFeatureComponentOptions<T> extends RenderAppComponentOptions<T> {
  imports: Array<any>
}

export interface RenderComponentResult<T> extends RenderResult<T> {
  triggerEventHandler: <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => void;
  router: Router;
  matDialog: TestingMatDialog;
  matDialogRef: TestingMatDialogRef;
  signalrConnectionFactory: TestingSignalrConnectionServiceFactory;
  activatedRoute: TestingActivatedRoute;
  settingsService: TestingSettingsService;
}

export async function renderAppComponent<T>(component: Type<T>, options?: RenderAppComponentOptions<T>): Promise<RenderComponentResult<T>> {
  const appOptions = createTestingModule({
    ...options,
    imports: [...(options?.imports || []), SharedModule],
    declarations: [
      (options?.declarations || []),
      ...SHELL_COMPONENTS
    ]
  });
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
  return {
    ...result,
    triggerEventHandler: createEventTriggerHandler(result),
    router: TestBed.inject(Router),
    matDialog: <TestingMatDialog>TestBed.inject(MatDialog),
    matDialogRef: <TestingMatDialogRef>TestBed.inject(MatDialogRef),
    signalrConnectionFactory: <TestingSignalrConnectionServiceFactory>TestBed.inject(SignalrHubConnectionFactory),
    activatedRoute: <TestingActivatedRoute>TestBed.inject(ActivatedRoute),
    settingsService: <TestingSettingsService>TestBed.inject(SettingsService)
  };
}

function createEventTriggerHandler<TComponent, TDirective>({fixture}: RenderResult<TComponent>) {
  return <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => {
      fixture.debugElement.query(By.directive(directive)).triggerEventHandler(eventName, eventArg);
  }
}
