import {generateStateFromActions} from "../../../testing/app-state-generator";
import {healthReducer} from "./health.reducer";
import {HealthActions} from "./actions";
import {ModelFactory} from "../../../testing";

describe('healthReducer', () => {
  it('should have null report when initialized', () => {
    const state = generateStateFromActions(healthReducer);

    expect(state.report).toEqual(null);
  })

  it('should update report when health report received', () => {
    const report = ModelFactory.createHealthReportModel();

    const state = generateStateFromActions(healthReducer,
      HealthActions.healthReceived(report)
    );

    expect(state.report).toEqual(report);
  })
})
