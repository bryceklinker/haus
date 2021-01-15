import {Component, OnDestroy, OnInit} from "@angular/core";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {AppState} from "../../../app.state";
import {HealthActions, selectHealthReport} from "../../state";
import {HausHealthReportModel} from "../../../shared/models";

@Component({
  selector: 'health-root',
  templateUrl: './health-root.component.html',
  styleUrls: ['./health-root.component.scss']
})
export class HealthRootComponent implements OnInit, OnDestroy {

  report$: Observable<HausHealthReportModel | null>;

  constructor(private readonly store: Store<AppState>) {
    this.report$ = this.store.select(selectHealthReport);
  }

  ngOnInit(): void {
    this.store.dispatch(HealthActions.start());
  }

  ngOnDestroy(): void {
    this.store.dispatch(HealthActions.stop());
  }
}
