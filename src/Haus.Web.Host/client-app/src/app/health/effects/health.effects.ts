import {Injectable} from "@angular/core";
import {ActionsSubject} from "@ngrx/store";
import {KNOWN_HUB_NAMES, SignalrEffectsFactory, SignalrService} from "../../shared/signalr";
import {Actions} from "@ngrx/effects";
import {HealthActions} from "../state";
import {HausHealthReportModel} from "../../shared/models";

@Injectable()
export class HealthEffects {
  start$;
  stop$;

  constructor(private readonly actionsSubject: ActionsSubject,
              private readonly signalrEffectsFactory: SignalrEffectsFactory) {
    const {start$, stop$} = this.signalrEffectsFactory.createEffects({
      hubName: KNOWN_HUB_NAMES.health,
      stopAction: HealthActions.stop,
      startAction: HealthActions.start,
      connectedAction: HealthActions.connected,
      disconnectedAction: HealthActions.disconnected,
      hubInitializer: hub => this.initializeHub(hub)
    })
    this.start$ = start$;
    this.stop$ = stop$;
  }

  private initializeHub(hub: SignalrService) {
    hub.on<HausHealthReportModel>("OnHealth", report => {
      this.actionsSubject.next(HealthActions.healthReceived(report));
    })
  }
}
