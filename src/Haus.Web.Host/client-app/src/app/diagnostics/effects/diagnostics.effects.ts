import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {INIT} from "@ngrx/store";
import {map, tap} from "rxjs/operators";
import {
  createSignalRHub,
  mergeMapHubToAction,
  ofHub,
  SIGNALR_HUB_UNSTARTED,
  startSignalRHub
} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "./diagnostics-hub";
import {of, merge} from "rxjs";
import {MqttDiagnosticsMessageModel} from "../models/mqtt-diagnostics-message.model";
import {DiagnosticsActions} from "../actions";

@Injectable()
export class DiagnosticsEffects {
  init$ = createEffect(() => this.actions$.pipe(
    ofType(INIT),
    map(() => createSignalRHub({
      ...DIAGNOSTICS_HUB,
      options: {}
    }))
  ));

  start$ = createEffect(() => this.actions$.pipe(
    ofType(SIGNALR_HUB_UNSTARTED),
    ofHub(DIAGNOSTICS_HUB),
    mergeMapHubToAction(({hub}) => {
      const onMessage$ = hub.on<MqttDiagnosticsMessageModel>('OnMqttMessage').pipe(
        map((model) => DiagnosticsActions.messageReceived(model))
      );
      return merge(onMessage$, of(startSignalRHub(hub)));
    })
  ))

  constructor(private actions$: Actions) {

  }
}
