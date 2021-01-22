import {
  createAppTestingService,
  eventually,
  ModelFactory, setupGetLatestPackages,
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

  it('should get latest version from api', async () => {
    const expected = ModelFactory.createApplicationVersion();
    setupGetLatestVersion(expected);

    actions$.next(ShellActions.loadLatestVersion.request());
    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(ShellActions.loadLatestVersion.success(expected));
    })
  })

  it('should get latest packages from api', async () => {
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
})
