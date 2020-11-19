import {SpectatorOptions, SpectatorServiceOptions} from "@ngneat/spectator";
import {Action, Store} from "@ngrx/store";
import {createTestingState} from "./create-testing-state";
import {AppTestingModule} from "./app-testing.module";
import {provideMockStore} from "@ngrx/store/testing";
import {TestingStore} from "./testing-store";
import {Type} from "@angular/core";
import {BaseSpectatorOptions} from "@ngneat/spectator/lib/base/options";

interface BaseOptions extends BaseSpectatorOptions {
  actions?: Array<Action>;
}

interface ComponentOptions<T> extends SpectatorOptions<T>, BaseOptions {
  props?: Partial<T>;
}

interface ServiceOptions<T> extends SpectatorServiceOptions<T>, BaseOptions {
}

export function createSpectatorOptions(options: Partial<BaseOptions>): BaseOptions {
  const initialState = createTestingState(...(options.actions || []));
  return {
    ...options,
    imports: [
      ...(options.imports || []),
      AppTestingModule
    ],
    providers: [
      ...(options.providers || []),
      ...provideMockStore(initialState),
      {provide: Store, useClass: TestingStore}
    ]
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
