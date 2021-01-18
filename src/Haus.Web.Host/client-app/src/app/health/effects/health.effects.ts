import {Injectable} from "@angular/core";
import {ActionsSubject} from "@ngrx/store";
import {KNOWN_HUB_NAMES, SignalrEffectsFactory, SignalrService} from "../../shared/signalr";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {HealthActions} from "../state";
import {HausHealthReportModel} from "../../shared/models";
import {HausApiClient} from "../../shared/rest-api";
import {map, mergeMap, takeUntil} from "rxjs/operators";
import {interval} from "rxjs";

@Injectable()
export class HealthEffects {
  start$;
  stop$;

  refreshLogs$ = createEffect(() => this.actions$.pipe(
    ofType(HealthActions.start),
    mergeMap(() => interval(3000).pipe(
      map(() => HealthActions.loadRecentLogs.request()),
      takeUntil(this.actions$.pipe(ofType(HealthActions.stop))),
    )),
  ))

  loadLogs$ = createEffect(() => this.actions$.pipe(
    ofType(HealthActions.loadRecentLogs.request),
    mergeMap(() => this.api.getLogs().pipe(
      map(result => HealthActions.loadRecentLogs.success(result))
    ))
  ))

  constructor(private readonly actionsSubject: ActionsSubject,
              private readonly signalrEffectsFactory: SignalrEffectsFactory,
              private readonly actions$: Actions,
              private readonly api: HausApiClient) {
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
