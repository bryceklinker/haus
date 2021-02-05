import {Component, OnDestroy, OnInit} from "@angular/core";
import {ActionsSubject, Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {Actions, ofType} from "@ngrx/effects";
import {tap} from "rxjs/operators";
import {AppState} from "../../../app.state";
import {ApplicationPackageModel, ApplicationVersionModel} from "../../../shared/models";
import {
  selectHasLatestVersion,
  selectIsDownloadingPackage,
  selectLatestPackages,
  selectLatestVersion, selectLatestVersionError,
  ShellActions
} from "../../state";
import {LoadingDialogService} from "../../../shared/components/loading-dialog/loading-dialog.service";
import {DestroyableSubject} from "../../../shared/destroyable-subject";

@Component({
  selector: 'latest-version-details-root',
  templateUrl: './latest-version-details-root.component.html',
  styleUrls: ['./latest-version-details-root.component.scss']
})
export class LatestVersionDetailsRootComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  latestVersion$: Observable<ApplicationVersionModel | null>;
  latestPackage$: Observable<Array<ApplicationPackageModel>>;
  isDownloadingPackage$: Observable<boolean>;
  hasLatestVersion$: Observable<boolean>;
  latestVersionError$: Observable<any>;

  constructor(private readonly store: Store<AppState>,
              private readonly actions$: ActionsSubject,
              private readonly loadingDialog: LoadingDialogService) {
    this.latestVersion$ = store.select(selectLatestVersion);
    this.latestPackage$ = store.select(selectLatestPackages);
    this.hasLatestVersion$ = store.select(selectHasLatestVersion);
    this.latestVersionError$ = store.select(selectLatestVersionError);
    this.isDownloadingPackage$ = store.select(selectIsDownloadingPackage);
  }

  ngOnDestroy(): void {
        this.destroyable.destroy();
    }

  ngOnInit(): void {
    this.store.dispatch(ShellActions.loadLatestPackages.request());
  }

  onDownloadPackage(model: ApplicationPackageModel) {
    const dialog = this.loadingDialog.open({text: `Downloading ${model.name}...`});

    this.destroyable.register(this.actions$.pipe(
      ofType(ShellActions.downloadPackage.success),
      tap(() => dialog.close())
    )).subscribe();

    this.store.dispatch(ShellActions.downloadPackage.request(model));
  }

  onRetry() {
    this.store.dispatch(ShellActions.loadLatestVersion.request());
    this.store.dispatch(ShellActions.loadLatestPackages.request());
  }
}
