import {MatDialog, MatDialogConfig, MatDialogRef} from "@angular/material/dialog";
import {Injectable, TemplateRef} from "@angular/core";
import {TestingMatDialogRef} from "./testing-mat-dialog-ref";
import {ComponentType} from "@angular/cdk/overlay";

@Injectable()
export class TestingMatDialog extends MatDialog {
  private _dialogRef: TestingMatDialogRef | null = null;

  get dialogRef(): TestingMatDialogRef | null {
    return this._dialogRef;
  }

  constructor() {
    super(<any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), jest.fn(), <any>jest.fn(), <any>jest.fn());
    jest.spyOn(this as TestingMatDialog, "open");
  }


  open<T, D = any, R = any>(template: ComponentType<T> | TemplateRef<T>, config?: MatDialogConfig<D>): MatDialogRef<T, R> {
    this._dialogRef = new TestingMatDialogRef<T, R>();
    return this._dialogRef;
  }
}
