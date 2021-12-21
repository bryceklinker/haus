import {
  createAppTestingService,
  eventually,
  ModelFactory, setupFailedDownloadPackage, setupFailedLatestPackages, setupFailedLatestVersion, setupGetLatestPackages,
  setupGetLatestVersion,
  TestingActionsSubject
} from "../../../testing";
import {ShellEffects} from "./shell.effects";
import {ShellActions} from "../state";

describe('ShellEffects', () => {
  let actions$: TestingActionsSubject;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(ShellEffects);
    actions$ = actionsSubject;
  })

  test('should get latest version from api', async () => {
    const expected = ModelFactory.createApplicationVersion();
    setupGetLatestVersion(expected);

    actions$.next(ShellActions.loadLatestVersion.request());
    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(ShellActions.loadLatestVersion.success(expected));
    })
  })

  test('should get latest packages from api', async () => {
    const packages = [
      ModelFactory.createApplicationPackage(),
      ModelFactory.createApplicationPackage(),
      ModelFactory.createApplicationPackage()
    ];
    setupGetLatestPackages(packages);

    actions$.next(ShellActions.loadLatestPackages.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(ShellActions.loadLatestPackages.success(ModelFactory.createListResult(...packages)));
    })
  })

  test('should notify getting latest packages failed', async () => {
    setupFailedLatestPackages();

    actions$.next(ShellActions.loadLatestPackages.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(expect.objectContaining({
        type: ShellActions.loadLatestPackages.failed.type
      }))
    })
  })

  test('should notify getting latest version failed', async () => {
    setupFailedLatestVersion();

    actions$.next(ShellActions.loadLatestVersion.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(expect.objectContaining({
        type: ShellActions.loadLatestVersion.failed.type
      }))
    })
  })

  test('should notify downloading package failed', async () => {
    setupFailedDownloadPackage(12);

    actions$.next(ShellActions.downloadPackage.request(ModelFactory.createApplicationPackage({id: 12})));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(expect.objectContaining({
        type: ShellActions.downloadPackage.failed.type
      }))
    })
  })

})
