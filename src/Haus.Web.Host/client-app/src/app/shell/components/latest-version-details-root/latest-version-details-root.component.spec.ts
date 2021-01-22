import {LatestVersionDetailsRootHarness} from "./latest-version-details-root.harness";
import {ShellActions} from "../../state";
import {ModelFactory} from "../../../../testing";

describe('LatestVersionDetailsComponent', () => {
  it('should show latest version information', async () => {
    const latestVersion = ModelFactory.createApplicationVersion({
      description: 'my description',
      version: '3.4.5',
      isOfficialRelease: true,
      isNewer: true
    });
    const packageModel = ModelFactory.createApplicationPackage();

    const harness = await LatestVersionDetailsRootHarness.render(
      ShellActions.loadLatestVersion.success(latestVersion),
      ShellActions.loadLatestPackages.success(ModelFactory.createListResult(packageModel))
    );

    expect(harness.descriptionElement).toHaveTextContent('my description');
    expect(harness.versionElement).toHaveTextContent('3.4.5');
    expect(harness.packages).toHaveLength(1);
  })

  it('should request application packages', async () => {
    const harness = await LatestVersionDetailsRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(ShellActions.loadLatestPackages.request());
  })

  it('should request package download when package is clicked', async () => {
    const packageModel = ModelFactory.createApplicationPackage({id: 88});

    const harness = await LatestVersionDetailsRootHarness.render(
      ShellActions.loadLatestPackages.success(ModelFactory.createListResult(packageModel))
    );
    await harness.downloadPackage();

    expect(harness.dispatchedActions).toContainEqual(ShellActions.downloadPackage.request(packageModel));
  })
})
