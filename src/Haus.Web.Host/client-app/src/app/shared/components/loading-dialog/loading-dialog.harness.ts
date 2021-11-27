import {screen} from "@testing-library/dom";
import {
  HausComponentHarness,
  RenderComponentResult,
  renderFeatureComponent
} from "../../../../testing";
import {LoadingDialogComponent} from "./loading-dialog.component";
import {SharedModule} from "../../shared.module";
import {LoadingDialogOptions} from "./loading-dialog.options";

export class LoadingDialogHarness extends HausComponentHarness<LoadingDialogComponent> {

  get loadingElement() {
    return screen.getByLabelText('loading indicator');
  }

  get isClosingDialogDisabled() {
    return this.dialogRef.disableClose;
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new LoadingDialogHarness(result);
  }

  static async render(dialogData?: LoadingDialogOptions) {
    const result = await renderFeatureComponent(LoadingDialogComponent,{
      imports: [SharedModule],
      dialogData
    });
    return LoadingDialogHarness.fromResult(result);
  }
}
