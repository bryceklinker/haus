import {createServiceFactory} from "@ngneat/spectator/jest";
import {Type} from "@angular/core";
import {createServiceOptions, ServiceOptions} from "./create-spectator-options";

export function createFeatureServiceFactory<T>(service: Type<T>, module: any, options?: Partial<ServiceOptions<T>>) {
  const opts = createServiceOptions(service, module, options);
  return createServiceFactory(opts);
}
