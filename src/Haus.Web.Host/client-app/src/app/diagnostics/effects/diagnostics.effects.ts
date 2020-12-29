import {Injectable, OnInit} from "@angular/core";
import {Actions, createEffect, ofType, OnInitEffects} from "@ngrx/effects";
import {Action, ActionsSubject, Store} from "@ngrx/store";
import {map, mergeMap} from "rxjs/operators";

import {KNOWN_HUB_NAMES, SignalrService, SignalrServiceFactory} from "../../shared/signalr";
import {DiagnosticsActions} from "../state";
import {DiagnosticsMessageModel} from "../models";
import {HausApiClient} from "../../shared/rest-api";

@Injectable()
export class DiagnosticsEffects {
  private service: SignalrService | null = null;

  private get hub(): SignalrService {
    return this.service || (this.service = this.initializeHub());
  }

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
  }

  initializeHub(): SignalrService {
    const service = this.signalrServiceFactory.create(KNOWN_HUB_NAMES.diagnostics);
    service.on<DiagnosticsMessageModel>('OnMqttMessage', msg => {
      this.actionsSubject.next(DiagnosticsActions.messageReceived(msg))
    });
    return service;
  }
}
