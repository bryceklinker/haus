import {createServiceFactory, SpectatorServiceOptions} from "@ngneat/spectator";
import {Type} from "@angular/core";
import {Action} from "@ngrx/store";
import {createServiceOptions} from "./create-spectator-options";

interface ServiceOptions<T> extends SpectatorServiceOptions<T> {
  actions?: Array<Action>;
}

export function createFeatureServiceFactory<T>(service: Type<T>, module: any, options?: Partial<ServiceOptions<T>>) {
  const opts = createServiceOptions(service, module, options);
  return createServiceFactory(opts);
}
