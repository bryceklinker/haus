import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType, OnInitEffects} from "@ngrx/effects";
import {Action, INIT} from "@ngrx/store";
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
import {AuthService} from "@auth0/auth0-angular";

@Injectable()
export class DiagnosticsEffects implements OnInitEffects {
  init$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.initEffects),
    map(() => createSignalRHub({
      ...DIAGNOSTICS_HUB,
      options: {
        // accessTokenFactory: () => this.auth.getAccessTokenSilently().toPromise()
      }
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

  // constructor(private actions$: Actions, private auth: AuthService) {
  constructor(private actions$: Actions) {

  }

  ngrxOnInitEffects(): Action {
    return DiagnosticsActions.initEffects();
  }
}
