import {Injectable} from "@angular/core";
import {map, mergeMap} from "rxjs/operators";
import {ActionsSubject} from "@ngrx/store";
import {Actions, createEffect, ofType} from "@ngrx/effects";

import {KNOWN_HUB_NAMES, SignalrEffectsFactory, SignalrService} from "../../shared/signalr";
import {DeviceSimulatorActions, DeviceSimulatorState} from "../state";
import {HausApiClient} from "../../shared/rest-api";

@Injectable()
export class DeviceSimulatorEffects {
  start$;
  stop$;

  addSimulatedDevice$ = createEffect(() => this.actions$.pipe(
    ofType(DeviceSimulatorActions.addSimulatedDevice.request),
    mergeMap(({payload}) => this.api.addSimulatedDevice(payload).pipe(
      map(() => DeviceSimulatorActions.addSimulatedDevice.success())
    ))
  ))

  triggerOccupancyChange$ = createEffect(() => this.actions$.pipe(
    ofType(DeviceSimulatorActions.triggerOccupancyChange.request),
    mergeMap(({payload}) => this.api.triggerOccupancyChange(payload.id).pipe(
      map(() => DeviceSimulatorActions.triggerOccupancyChange.success())
    ))
  ))

  constructor(private readonly actionsSubject: ActionsSubject,
              private readonly signalrEffectsFactory: SignalrEffectsFactory,
              private readonly actions$: Actions,
              private readonly api: HausApiClient) {
    const {start$, stop$} = this.signalrEffectsFactory.createEffects({
      hubName: KNOWN_HUB_NAMES.deviceSimulator,
      stopAction: DeviceSimulatorActions.stop,
      startAction: DeviceSimulatorActions.start,
      connectedAction: DeviceSimulatorActions.connected,
      disconnectedAction: DeviceSimulatorActions.disconnected,
      hubInitializer: hub => this.initializeHub(hub)
    })
    this.start$ = start$;
    this.stop$ = stop$;
  }

  private initializeHub(hub: SignalrService) {
    hub.on<DeviceSimulatorState>('OnState', state => {
      this.actionsSubject.next(DeviceSimulatorActions.stateReceived(state));
    });
  }
}
