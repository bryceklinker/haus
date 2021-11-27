import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {DownloadingPackageDialogComponent} from "./downloading-package-dialog.component";
import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";
import {ShellActions} from "../../state";

export class DownloadingPackageDialogHarness extends HausComponentHarness<DownloadingPackageDialogComponent> {

  get loadingElement() {
    return screen.queryByLabelText('loading indicator');
  }

  get isClosingDialogDisabled() {
    return this.dialogRef.disableClose;
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new DownloadingPackageDialogHarness(result);
  }

  static async render(dialogData?: any, ...actions: Action[]) {
    const result = await renderAppComponent(DownloadingPackageDialogComponent,{
      actions,
      dialogData
    });
    return DownloadingPackageDialogHarness.fromResult(result);
  }

  async simulateDownloadSuccess() {
    this.actionsSubject.next(ShellActions.downloadPackage.success());
  }
}
