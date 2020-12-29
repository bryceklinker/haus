import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {ActionsSubject, Store} from "@ngrx/store";
import {map, mergeMap} from "rxjs/operators";

import {KNOWN_HUB_NAMES, SignalrService, SignalrServiceFactory} from "../../shared/signalr";
import {DiagnosticsActions} from "../state";
import {DiagnosticsMessageModel} from "../models";
import {HausApiClient} from "../../shared/rest-api";

@Injectable()
export class DiagnosticsEffects {
  private readonly hub: SignalrService;

  start$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.start),
    mergeMap(() => this.hub.start().pipe(
      map(() => DiagnosticsActions.connected())
    ))
  ))

  stop$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.stop),
    mergeMap(() => this.hub.stop().pipe(
      map(() => DiagnosticsActions.disconnected())
    ))
  ))

  replay$ = createEffect(() => this.actions$.pipe(
    ofType(DiagnosticsActions.replayMessage.request),
    mergeMap(({payload}) => this.api.replayMessage(payload).pipe(
      map(() => DiagnosticsActions.replayMessage.success(payload))
    ))
  ))

  constructor(private readonly actions$: Actions,
              private readonly signalrServiceFactory: SignalrServiceFactory,
              private readonly actionsSubject: ActionsSubject,
              private readonly api: HausApiClient) {
    this.hub = this.signalrServiceFactory.create(KNOWN_HUB_NAMES.diagnostics);
    this.hub.on<DiagnosticsMessageModel>('OnMqttMessage', msg => {
      this.actionsSubject.next(DiagnosticsActions.messageReceived(msg))
    });
  }
}
