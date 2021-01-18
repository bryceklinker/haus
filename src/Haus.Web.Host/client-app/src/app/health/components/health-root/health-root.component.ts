import {Component, OnDestroy, OnInit} from "@angular/core";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {AppState} from "../../../app.state";
import {HealthActions, selectHealthReport, selectRecentLogs} from "../../state";
import {HausEvent, HausHealthReportModel, LogEntryModel} from "../../../shared/models";
import {selectAllEvents} from "../../../shared/events";

@Component({
  selector: 'health-root',
  templateUrl: './health-root.component.html',
  styleUrls: ['./health-root.component.scss']
})
export class HealthRootComponent implements OnInit, OnDestroy {
  report$: Observable<HausHealthReportModel | null>;
  events$: Observable<Array<HausEvent>>;
  logs$: Observable<Array<LogEntryModel>>;

  constructor(private readonly store: Store<AppState>) {
    this.report$ = this.store.select(selectHealthReport);
    this.events$ = this.store.select(selectAllEvents);
    this.logs$ = this.store.select(selectRecentLogs);
  }

  ngOnInit(): void {
    this.store.dispatch(HealthActions.start());
  }

  ngOnDestroy(): void {
    this.store.dispatch(HealthActions.stop());
  }
}
