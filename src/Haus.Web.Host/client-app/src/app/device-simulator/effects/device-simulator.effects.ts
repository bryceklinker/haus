import {Injectable} from "@angular/core";
import {KNOWN_HUB_NAMES, SignalrEffectsFactory, SignalrService} from "../../shared/signalr";
import {ActionsSubject} from "@ngrx/store";
import {DeviceSimulatorActions, DeviceSimulatorState} from "../state";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {map, mergeMap} from "rxjs/operators";
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
