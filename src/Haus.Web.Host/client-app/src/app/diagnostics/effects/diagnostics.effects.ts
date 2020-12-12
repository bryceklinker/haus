import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {catchError, map, mergeMap} from "rxjs/operators";
import {createSignalRHub, mergeMapHubToAction, ofHub, SIGNALR_HUB_UNSTARTED, startSignalRHub, } from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "./diagnostics-hub";
import {merge, of} from "rxjs";
import {DiagnosticsActions} from "../actions";
import {HttpClient} from "@angular/common/http";
import {AuthService} from "@auth0/auth0-angular";
import {fromObservableToPromise} from "../../shared/rxjs";
import {DiagnosticsMessageModel} from "../models";

@Injectable()
export class DiagnosticsEffects {
  init$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.initHub),
    map(() => createSignalRHub({
      ...DIAGNOSTICS_HUB,
      options: {
        accessTokenFactory: async () => await fromObservableToPromise(this.auth.getAccessTokenSilently())
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
    ofType(DiagnosticsActions.replay.request),
    mergeMap(({payload}) => this.http.post('/api/diagnostics/replay', payload).pipe(
      map(() => DiagnosticsActions.replay.success(payload)),
      catchError(err => of(DiagnosticsActions.replay.failed(payload, err)))
    ))
  ))
  constructor(private actions$: Actions, private auth: AuthService, private http: HttpClient) {

  }
}
