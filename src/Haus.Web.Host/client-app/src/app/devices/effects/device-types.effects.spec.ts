import {
  createAppTestingService,
  eventually,
  ModelFactory,
  setupGetAllDeviceTypes,
  TestingActionsSubject
} from "../../../testing";
import {DeviceTypesEffects} from "./device-types.effects";
import {DeviceTypesActions} from "../state";

describe('DeviceTypesEffects', () => {
  let actions$: TestingActionsSubject;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(DeviceTypesEffects);
    actions$ = actionsSubject;
  })

  it('should get device types from api when load device types requested', async () => {
    const expected = ['one', 'two', 'three'];
    setupGetAllDeviceTypes(expected);

    actions$.next(DeviceTypesActions.loadDeviceTypes.request());

    const expectedAction = DeviceTypesActions.loadDeviceTypes.success(ModelFactory.createListResult(...expected));
    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(expectedAction);
    })
  })
})
