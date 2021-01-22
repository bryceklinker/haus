import {Component, OnInit} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {ApplicationPackageModel, ApplicationVersionModel} from "../../../shared/models";
import {selectLatestPackages, selectLatestVersion, ShellActions} from "../../state";

@Component({
  selector: 'latest-version-details-root',
  templateUrl: './latest-version-details-root.component.html',
  styleUrls: ['./latest-version-details-root.component.scss']
})
export class LatestVersionDetailsRootComponent implements OnInit {
  latestVersion$: Observable<ApplicationVersionModel | null>;
  latestPackage$: Observable<Array<ApplicationPackageModel>>;

  constructor(private readonly store: Store<AppState>) {
    this.latestVersion$ = store.select(selectLatestVersion);
    this.latestPackage$ = store.select(selectLatestPackages);
  }

  ngOnInit(): void {
    this.store.dispatch(ShellActions.loadLatestPackages.request());
  }

  onDownloadPackage(model: ApplicationPackageModel) {
    this.store.dispatch(ShellActions.downloadPackage.request(model));
  }
}
