import {generateStateFromActions} from "../../../testing/app-state-generator";
import {healthReducer} from "./health.reducer";
import {HealthActions} from "./actions";
import {ModelFactory} from "../../../testing";

describe('healthReducer', () => {
  it('should have null report when initialized', () => {
    const state = generateStateFromActions(healthReducer);

    expect(state.report).toEqual(null);
    expect(state.logs).toEqual([]);
  })

  it('should update report when health report received', () => {
    const report = ModelFactory.createHealthReportModel();

    const state = generateStateFromActions(healthReducer,
      HealthActions.healthReceived(report)
    );

    expect(state.report).toEqual(report);
  })

  it('should have recent logs when logs are loaded successfully', () => {
    const result = ModelFactory.createListResult(
      ModelFactory.createLogEntry(),
      ModelFactory.createLogEntry(),
      ModelFactory.createLogEntry()
    )
    const state = generateStateFromActions(healthReducer,
      HealthActions.loadRecentLogs.success(result)
    );

    expect(state.logs).toEqual(result.items);
  })
})
