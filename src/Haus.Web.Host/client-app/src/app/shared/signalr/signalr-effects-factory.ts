import {Injectable} from "@angular/core";
import {SignalrServiceFactory} from "./signalr-service.factory";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {Action, ActionCreator} from "@ngrx/store";
import {map, mergeMap} from "rxjs/operators";
import {SignalrService} from "./signalr.service";

interface SignalREffectsOptions {
  hubName: string;
  startAction: ActionCreator;
  stopAction: ActionCreator;
  connectedAction: () => Action;
  disconnectedAction: () => Action;
  hubInitializer?: (hub: SignalrService) => void;
}

export type SignalrEffects = ReturnType<typeof SignalrEffectsFactory.prototype.createEffects>
@Injectable({
  providedIn: 'root'
})
export class SignalrEffectsFactory {
  constructor(private readonly signalrFactory: SignalrServiceFactory,
              private readonly actions$: Actions) {

  }

  createEffects(options: SignalREffectsOptions) {
    let service: SignalrService | null = null;
    const initializeHub = () => {
        const hubService = this.signalrFactory.create(options.hubName);
        if (options.hubInitializer) {
          options?.hubInitializer(hubService);
        }
        return hubService;
    };
    const getHub = () => service || (service = initializeHub());

    return {
      start$: createEffect(() => this.actions$.pipe(
        ofType(options.startAction),
        mergeMap(() => getHub().start().pipe(
          map(() => options.connectedAction())
        ))
      )),
      stop$: createEffect(() => this.actions$.pipe(
        ofType(options.stopAction),
        mergeMap(() => getHub().stop().pipe(
          map(() => options.disconnectedAction())
        ))
      ))
    }
  }
}
