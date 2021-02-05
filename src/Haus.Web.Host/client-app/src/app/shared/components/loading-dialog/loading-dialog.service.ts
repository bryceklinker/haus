import {Injectable} from "@angular/core";
import {LoadingDialogOptions} from "./loading-dialog.options";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {LoadingDialogComponent} from "./loading-dialog.component";

@Injectable()
export class LoadingDialogService {
  constructor(private readonly matDialog: MatDialog) {
  }

  open(options: LoadingDialogOptions): MatDialogRef<LoadingDialogComponent> {
    return this.matDialog.open(LoadingDialogComponent, {data: options});
  }
}
