import {LatestVersionDetailsHarness} from "./latest-version-details.harness";
import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {ApplicationPackageModel} from "../../../shared/models";

describe('LatestVersionDetailsComponent', () => {
  it('should show version details', async () => {
    const version = ModelFactory.createApplicationVersion({
      isNewer: true,
      isOfficialRelease: true,
      version: '7.5.1',
      description: 'big release',
      creationDate: '2020-09-23T12:34:89.123Z'
    })
    const harness = await LatestVersionDetailsHarness.render({version});

    expect(harness.descriptionElement).toHaveTextContent('big release');
    expect(harness.getIsNewerRelease()).toEqual(true);
    expect(harness.getIsOfficialRelease()).toEqual(true);
    expect(harness.releaseDate).toHaveTextContent('2020-09-23T12:34:89.123Z');
    expect(harness.versionElement).toHaveTextContent('7.5.1');
  })

  it('should show packages', async () => {
    const packages = [
      ModelFactory.createApplicationPackage(),
      ModelFactory.createApplicationPackage(),
      ModelFactory.createApplicationPackage()
    ];

    const harness = await LatestVersionDetailsHarness.render({packages});

    expect(harness.packages).toHaveLength(3);
  })

  it('should notify when package is downloaded', async () => {
    const packages = [
      ModelFactory.createApplicationPackage({id: 77})
    ];
    const emitter = new TestingEventEmitter<ApplicationPackageModel>();

    const harness = await LatestVersionDetailsHarness.render({packages, downloadPackage: emitter});
    await harness.downloadPackage();

    expect(emitter.emit).toHaveBeenCalledWith(packages[0]);
  })
})
