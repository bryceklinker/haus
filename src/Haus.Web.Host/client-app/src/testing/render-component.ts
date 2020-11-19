import {render, RenderComponentOptions, RenderResult, fireEvent} from '@testing-library/angular';
import {Type} from "@angular/core";
import {Routes} from "@angular/router";
import {AppState} from "../app/app.state";
import {TestingStore} from "./testing-store";
import {Store} from "@ngrx/store";
import {SharedModule} from "../app/shared/shared.module";
import {TestBed} from "@angular/core/testing";
import {SHELL_COMPONENTS} from "../app/shell/components";
import {createTestingModule} from "./create-testing-module";
import {By} from "@angular/platform-browser";

export interface RenderAppComponentOptions<T> extends RenderComponentOptions<T> {
  routes?: Routes;
  state?: AppState;
}

export interface RenderFeatureComponentOptions<T> extends RenderAppComponentOptions<T> {
  imports: Array<any>
}

export interface RenderComponentResult<T> extends RenderResult<T> {
  store: TestingStore<AppState>;
  fireEvent: typeof fireEvent,
  triggerEventHandler: <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => void;
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

async function renderComponent<T>(component: Type<T>, options: RenderComponentOptions<T>): Promise<RenderComponentResult<T>> {
  const result = <RenderComponentResult<T>>await render(component, {
    ...options,
    excludeComponentDeclaration: true,
  });
  result.store = <TestingStore<AppState>>TestBed.inject(Store);
  result.fireEvent = fireEvent;
  result.triggerEventHandler = <TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) => {
    result.fixture.debugElement.query(By.directive(directive))
      .triggerEventHandler(eventName, eventArg);
  }

  return result;
}

