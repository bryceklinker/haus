import {Component, Inject, OnDestroy, OnInit} from "@angular/core";
import {Actions, ofType} from "@ngrx/effects";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {ApplicationPackageModel} from "../../../shared/models";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {ShellActions} from "../../state";
import {tap} from "rxjs/operators";

@Component({
  selector: 'downloading-package-dialog',
  templateUrl: './downloading-package-dialog.component.html',
  styleUrls: ['./downloading-package-dialog.component.scss']
})
export class DownloadingPackageDialogComponent implements OnInit, OnDestroy {
  private readonly destroyable: DestroyableSubject;

  get packageName(): string {
    return this.model.name;
  }

  constructor(private readonly actions$: Actions,
              private readonly dialogRef: MatDialogRef<DownloadingPackageDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private readonly model: ApplicationPackageModel) {
    this.destroyable = new DestroyableSubject();
  }

  ngOnInit(): void {
    this.dialogRef.disableClose = true;
    this.destroyable.register(this.actions$.pipe(
      ofType(ShellActions.downloadPackage.success),
      tap(() => this.dialogRef.close())
    )).subscribe();
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }
}
