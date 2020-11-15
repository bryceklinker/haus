import {Type} from "@angular/core";
import {render, fireEvent} from '@testing-library/angular';
import {RenderComponentOptions} from "@testing-library/angular/src/lib/models";
import {provideMockStore} from "@ngrx/store/testing";
import {Action, Store} from "@ngrx/store";
import {TestingStore} from "./testing-store";
import {createTestingState} from "./create-testing-state";
import {AppTestingModule} from "./app-testing.module";

interface RenderOptions<T> extends RenderComponentOptions<T> {
  actions?: Array<Action>;
}

export async function renderComponent<T>(component: Type<T>, options?: RenderOptions<T>) {
  const opts = {
    ...(options || {})
  }
  const initialState = createTestingState(...(opts.actions || []));
  const result = await render(component, {
    ...opts,
    imports: [
      AppTestingModule,
      ...(opts.imports || [])
    ],
    providers: [
      ...(opts.providers || []),
      ...provideMockStore(initialState),
      { provide: Store, useClass: TestingStore}
    ]
  });

  return {
    ...result,
    fireEvent
  }
}
