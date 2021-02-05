import {Component, Inject, OnInit} from "@angular/core";
import {Actions} from "@ngrx/effects";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {LoadingDialogOptions} from "./loading-dialog.options";

@Component({
  selector: 'loading-dialog',
  templateUrl: './loading-dialog.component.html',
  styleUrls: ['./loading-dialog.component.scss']
})
export class LoadingDialogComponent implements OnInit {

  get text(): string {
    return this.model.text;
  }

  constructor(private readonly actions$: Actions,
              private readonly dialogRef: MatDialogRef<LoadingDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private readonly model: LoadingDialogOptions) {
  }

  ngOnInit(): void {
    this.dialogRef.disableClose = true;
  }
}
