import {Injectable, OnInit} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {Action, ActionsSubject, Store} from "@ngrx/store";
import {map, mergeMap} from "rxjs/operators";

import {KNOWN_HUB_NAMES, SignalrService} from "../../shared/signalr";
import {DiagnosticsActions} from "../state";
import {DiagnosticsMessageModel} from "../models";
import {HausApiClient} from "../../shared/rest-api";
import {SignalrEffectsFactory} from "../../shared/signalr";

@Injectable()
export class DiagnosticsEffects {
  replay$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.replayMessage.request),
    mergeMap(({payload}) => this.api.replayMessage(payload).pipe(
      map(() => DiagnosticsActions.replayMessage.success(payload))
    ))
  ))

  start$;
  stop$;

  constructor(private readonly actions$: Actions,
              private readonly actionsSubject: ActionsSubject,
              private readonly api: HausApiClient,
              private readonly signalREffectsFactory: SignalrEffectsFactory) {
    const {start$, stop$} = this.signalREffectsFactory.createEffects({
      hubName: KNOWN_HUB_NAMES.diagnostics,
      disconnectedAction: DiagnosticsActions.disconnected,
      connectedAction: DiagnosticsActions.connected,
      startAction: DiagnosticsActions.start,
      stopAction: DiagnosticsActions.stop,
      hubInitializer: hub => this.initializeHub(hub)
    });

    this.start$ = start$;
    this.stop$ = stop$;
  }

  initializeHub(hub: SignalrService) {
    hub.on<DiagnosticsMessageModel>('OnMqttMessage', msg => {
      this.actionsSubject.next(DiagnosticsActions.messageReceived(msg))
    });
  }
}
