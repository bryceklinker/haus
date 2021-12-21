import {ModelFactory} from "../../../testing";
import {LightType} from "../../shared/models";
import {generateStateFromActions} from "../../../testing/app-state-generator";
import {lightTypesReducer} from "./light-types.reducer";
import {LightTypesActions} from "./actions";

describe('lightTypesReducer', () => {
  test('should populate light types in ascending order', () => {
    const result = ModelFactory.createListResult(
      LightType.Temperature,
      LightType.Color,
      LightType.Level,
    );

    const state = generateStateFromActions(lightTypesReducer,
      LightTypesActions.loadLightTypes.success(result)
    );

    expect(state.lightTypes[0]).toEqual(LightType.Color);
    expect(state.lightTypes[1]).toEqual(LightType.Level);
    expect(state.lightTypes[2]).toEqual(LightType.Temperature);
  })
})
