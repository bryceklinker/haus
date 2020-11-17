import {SpectatorOptions} from "@ngneat/spectator";
import {Action, Store} from "@ngrx/store";
import {createTestingState} from "./create-testing-state";
import {AppTestingModule} from "./app-testing.module";
import {provideMockStore} from "@ngrx/store/testing";
import {TestingStore} from "./testing-store";

interface SpectatorRenderOptions<T> extends SpectatorOptions<T>  {
  actions?: Array<Action>;
  props?: Partial<T>;
}

export function createSpectatorOptions<T>(options: SpectatorRenderOptions<T>): SpectatorRenderOptions<T> {
  const initialState = createTestingState(...(options.actions || []));
  const declarations = (options.declarations || []);
  return {
    ...options,
    declarations: declarations,
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
