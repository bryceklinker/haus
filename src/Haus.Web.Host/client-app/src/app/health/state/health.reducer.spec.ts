import {generateStateFromActions} from "../../../testing/app-state-generator";
import {healthReducer} from "./health.reducer";

describe('healthReducer', () => {
  it('should have empty events when initialized', () => {
    const state = generateStateFromActions(healthReducer);

  })
})
