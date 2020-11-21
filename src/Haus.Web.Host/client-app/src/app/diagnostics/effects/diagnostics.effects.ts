import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType, OnInitEffects} from "@ngrx/effects";
import {Action} from "@ngrx/store";
import {catchError, map, mergeMap, take, tap} from "rxjs/operators";
import {
  createSignalRHub,
  mergeMapHubToAction,
  ofHub,
  SIGNALR_HUB_UNSTARTED,
  startSignalRHub
} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "./diagnostics-hub";
import {of, merge} from "rxjs";
import {DiagnosticsActions} from "../actions";
import {HttpClient} from "@angular/common/http";
import {AuthService} from "@auth0/auth0-angular";
import {fromObservableToPromise} from "../../shared/rxjs";
import {DiagnosticsMessageModel} from "../models";

@Injectable()
export class DiagnosticsEffects implements OnInitEffects {
  init$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.initEffects),
    map(() => createSignalRHub({
      ...DIAGNOSTICS_HUB,
      options: {
        accessTokenFactory: () => fromObservableToPromise(this.auth.getAccessTokenSilently())
      }
    }))
  ));

  start$ = createEffect(() => this.actions$.pipe(
    ofType(SIGNALR_HUB_UNSTARTED),
    ofHub(DIAGNOSTICS_HUB),
    mergeMapHubToAction(({hub}) => {
      const onMessage$ = hub.on<DiagnosticsMessageModel>('OnMqttMessage').pipe(
        map((model) => DiagnosticsActions.messageReceived(model))
      );
      return merge(onMessage$, of(startSignalRHub(hub)));
    })
  ))

  replayMessage$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.replayMessageRequest),
    mergeMap(({payload}) => this.http.post('/api/diagnostics/replay', payload).pipe(
      map(() => DiagnosticsActions.replayMessageSuccess()),
      catchError(err => of(DiagnosticsActions.replayMessageFailed(payload, err)))
    ))
  ))
  constructor(private actions$: Actions, private auth: AuthService, private http: HttpClient) {

  }

  ngrxOnInitEffects(): Action {
    return DiagnosticsActions.initEffects();
  }
}
