import {
  createAppTestingService,
  eventually,
  ModelFactory,
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
})
