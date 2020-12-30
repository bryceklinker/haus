import {Injectable} from "@angular/core";
import {KNOWN_HUB_NAMES, SignalrEffectsFactory, SignalrService} from "../../shared/signalr";
import {ActionsSubject} from "@ngrx/store";
import {DeviceSimulatorActions, DeviceSimulatorState} from "../state";

@Injectable()
export class DeviceSimulatorEffects {
  start$;
  stop$;

  constructor(private readonly actionsSubject: ActionsSubject,
              private readonly signalrEffectsFactory: SignalrEffectsFactory) {
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
