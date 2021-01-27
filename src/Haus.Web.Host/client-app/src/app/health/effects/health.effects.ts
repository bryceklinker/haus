import {Injectable} from "@angular/core";
import {ActionsSubject} from "@ngrx/store";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {catchError, map, mergeMap, takeUntil} from "rxjs/operators";
import {interval, of} from "rxjs";
import {KNOWN_HUB_NAMES, SignalrEffectsFactory, SignalrService} from "../../shared/signalr";
import {HealthActions} from "../state";
import {HausHealthReportModel} from "../../shared/models";
import {HausApiClient} from "../../shared/rest-api";
import {MatSnackBar} from "@angular/material/snack-bar";

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
      map(result => HealthActions.loadRecentLogs.success(result)),
      catchError(err => of(HealthActions.loadRecentLogs.failed(err)))
    ))
  ))

  showFailedLogsLoad$ = createEffect(() => this.actions$.pipe(
    ofType(HealthActions.loadRecentLogs.failed),
    map(() => this.snackBar.open('Failed to load logs'))
  ), {dispatch: false})

  constructor(private readonly actionsSubject: ActionsSubject,
              private readonly signalrEffectsFactory: SignalrEffectsFactory,
              private readonly actions$: Actions,
              private readonly api: HausApiClient,
              private readonly snackBar: MatSnackBar) {
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
