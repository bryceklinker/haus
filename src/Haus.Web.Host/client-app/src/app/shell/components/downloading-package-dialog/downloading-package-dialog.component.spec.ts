import {DownloadingPackageDialogHarness} from "./downloading-package-dialog.harness";
import {ShellActions} from "../../state";
import {eventually, ModelFactory} from "../../../../testing";

describe('DownloadingPackageDialogComponent', () => {
  test('should show downloading package', async () => {
    const model = ModelFactory.createApplicationPackage();

    const harness = await DownloadingPackageDialogHarness.render(model, ShellActions.downloadPackage.request(model));

    expect(harness.container).toHaveTextContent(model.name);
    expect(harness.loadingElement).toBeInTheDocument();
  })

  test('should close when downloading is successful', async () => {
    const model = ModelFactory.createApplicationPackage();

    const harness = await DownloadingPackageDialogHarness.render(model, ShellActions.downloadPackage.request(model));

    await harness.simulateDownloadSuccess();

    await eventually(() => {
      expect(harness.dialogRef.close).toHaveBeenCalled();
    })
  })

  test('should disable closing dialog', async () => {
    const model = ModelFactory.createApplicationPackage();

    const harness = await DownloadingPackageDialogHarness.render(model, ShellActions.downloadPackage.request(model));

    expect(harness.isClosingDialogDisabled).toEqual(true);
  })
})
