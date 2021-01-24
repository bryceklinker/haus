import {Component, OnInit} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {ApplicationPackageModel, ApplicationVersionModel} from "../../../shared/models";
import {
  selectHasLatestVersion,
  selectHasLatestVersionError, selectIsDownloadingPackage,
  selectLatestPackages,
  selectLatestVersion, selectLatestVersionError,
  ShellActions
} from "../../state";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {DownloadingPackageDialogComponent} from "../downloading-package-dialog/downloading-package-dialog.component";

@Component({
  selector: 'latest-version-details-root',
  templateUrl: './latest-version-details-root.component.html',
  styleUrls: ['./latest-version-details-root.component.scss']
})
export class LatestVersionDetailsRootComponent implements OnInit {
  latestVersion$: Observable<ApplicationVersionModel | null>;
  latestPackage$: Observable<Array<ApplicationPackageModel>>;
  isDownloadingPackage$: Observable<boolean>;
  hasLatestVersion$: Observable<boolean>;
  latestVersionError$: Observable<any>;

  constructor(private readonly store: Store<AppState>,
              private readonly dialog: MatDialog) {
    this.latestVersion$ = store.select(selectLatestVersion);
    this.latestPackage$ = store.select(selectLatestPackages);
    this.hasLatestVersion$ = store.select(selectHasLatestVersion);
    this.latestVersionError$ = store.select(selectLatestVersionError);
    this.isDownloadingPackage$ = store.select(selectIsDownloadingPackage);
  }

  ngOnInit(): void {
    this.store.dispatch(ShellActions.loadLatestPackages.request());
  }

  onDownloadPackage(model: ApplicationPackageModel) {
    this.dialog.open(DownloadingPackageDialogComponent, { data: model});
    this.store.dispatch(ShellActions.downloadPackage.request(model));
  }

  onRetry() {
    this.store.dispatch(ShellActions.loadLatestVersion.request());
    this.store.dispatch(ShellActions.loadLatestPackages.request());
  }
}
