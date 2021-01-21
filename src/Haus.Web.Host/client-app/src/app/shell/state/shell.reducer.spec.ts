import {generateStateFromActions} from "../../../testing/app-state-generator";
import {shellReducer} from "./shell.reducer";
import {ShellActions} from "./actions";
import {ModelFactory} from "../../../testing";

describe('shellReducer', () => {
  it('should update latest version', () => {
    const version = ModelFactory.createApplicationVersion();
    const state = generateStateFromActions(shellReducer,
      ShellActions.loadLatestVersion.success(version)
    );

    expect(state.latestVersion).toEqual(version);
  })
})
