import {Type} from "@angular/core";
import {createComponentFactory} from "@ngneat/spectator/jest";
import {
  CreateComponentOptions,
  createComponentOptions,
  getTestingProviders
} from "./create-spectator-options";
import {createTestingState} from "./create-testing-state";

export function createFeatureComponentFactory<T>(component: Type<T>, module: any) {
  const opts = createComponentOptions({
    component,
    imports: [module]
  });

  const factory = createComponentFactory(opts);
  return (options?: CreateComponentOptions<T>) => {
    const state = createTestingState(...(options?.actions || []));
    const {providers} = getTestingProviders({state});
    return factory({
      ...opts,
      ...(options || {}),
      providers: providers
    })
  };
}
