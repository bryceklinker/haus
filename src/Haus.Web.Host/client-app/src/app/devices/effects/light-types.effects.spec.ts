import {
  createAppTestingService,
  eventually,
  ModelFactory,
  setupGetAllLightTypes,
  TestingActionsSubject
} from "../../../testing";
import {LightTypesEffects} from "./light-types.effects";
import {LightType} from "../../shared/models";
import {LightTypesActions} from "../state";

describe('LightTypesEffects', () => {
  let actions$: TestingActionsSubject;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(LightTypesEffects);
    actions$ = actionsSubject;
  })

  it('should get light types from api when load light types requested', async () => {
    const expected = [LightType.Level, LightType.Color];
    setupGetAllLightTypes(expected);

    actions$.next(LightTypesActions.loadLightTypes.request());

    const expectedAction = LightTypesActions.loadLightTypes.success(ModelFactory.createListResult(...expected));
    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(expectedAction);
    })
  })
})
