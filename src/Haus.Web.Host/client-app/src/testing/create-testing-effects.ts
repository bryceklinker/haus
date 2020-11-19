import {Type} from "@angular/core";
import {createFeatureServiceFactory} from "./create-feature-service-factory";
import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {SpectatorServiceFactory} from "@ngneat/spectator/jest";

interface EffectsSpector<T> {
  factory: SpectatorServiceFactory<T>;
  actions$: Subject<Action>;
}

export function createFeatureTestingEffects<T>(effects: Type<T>, module: any): EffectsSpector<T> {
  const actions$ = new Subject<Action>();
  const factory = createFeatureServiceFactory(effects, module, {actions$});

  return { factory, actions$ }
}
