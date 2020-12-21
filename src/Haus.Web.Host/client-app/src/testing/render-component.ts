import {render, RenderComponentOptions, RenderResult, fireEvent} from '@testing-library/angular';
import {Type} from "@angular/core";
import {Router, Routes} from "@angular/router";
import {TestBed} from "@angular/core/testing";
import {Action, Store} from "@ngrx/store";
import {By} from "@angular/platform-browser";
import userEvent from '@testing-library/user-event';

import {AppState} from "../app/app.state";
import {SharedModule} from "../app/shared/shared.module";
import {SHELL_COMPONENTS} from "../app/shell/components";
import {createTestingModule} from "./create-testing-module";
import {TestingStore} from "./fakes";
import {TestingActions} from "./testing-actions";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {TestingMatDialog} from "./fakes/testing-mat-dialog";
import {TestingMatDialogRef} from "./fakes/testing-mat-dialog-ref";

export interface RenderAppComponentOptions<T> extends RenderComponentOptions<T> {
  routes?: Routes;
  actions?: Action[];
}

export interface RenderFeatureComponentOptions<T> extends RenderAppComponentOptions<T> {
  imports: Array<any>
}

export interface RenderComponentResult<T> extends RenderResult<T> {
  store: TestingStore<AppState>;
  fireEvent: typeof fireEvent,
  triggerEventHandler: <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => void;
  router: Router;
  matDialog: TestingMatDialog,
  matDialogRef: TestingMatDialogRef,
  userEvent: typeof userEvent
}

export async function renderAppComponent<T>(component: Type<T>, options?: RenderAppComponentOptions<T>): Promise<RenderComponentResult<T>> {
  const appOptions = createTestingModule({
    ...options,
    imports: [SharedModule],
    declarations: [
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
    excludeComponentDeclaration: true,
  });
  dispatchActions(options.actions)
  result.fixture.detectChanges();
  return {
    ...result,
    triggerEventHandler: createEventTriggerHandler(result),
    store: <TestingStore<AppState>>TestBed.inject(Store),
    fireEvent,
    router: TestBed.inject(Router),
    matDialog: <TestingMatDialog>TestBed.inject(MatDialog),
    matDialogRef: <TestingMatDialogRef>TestBed.inject(MatDialogRef),
    userEvent: userEvent
  };
}

function dispatchActions(actions: Action[] = []) {
  const store = TestBed.inject(Store);
  store.dispatch(TestingActions.initAction());
  for (let action of actions) {
    store.dispatch(action);
  }
}

function createEventTriggerHandler<TComponent, TDirective>({fixture}: RenderResult<TComponent>) {
  return <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => {
      fixture.debugElement.query(By.directive(directive)).triggerEventHandler(eventName, eventArg);
  }
}
