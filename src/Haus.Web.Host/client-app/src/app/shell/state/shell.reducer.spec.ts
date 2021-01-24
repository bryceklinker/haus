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

  it('should update latest packages', () => {
    const result = ModelFactory.createListResult(
      ModelFactory.createApplicationPackage(),
      ModelFactory.createApplicationPackage(),
      ModelFactory.createApplicationPackage()
    );

    const state = generateStateFromActions(shellReducer,
      ShellActions.loadLatestPackages.success(result)
    );

    expect(state.latestPackages).toEqual(result.items);
  })

  it('should have error when loading latest version fails', () => {
    const error = new Error('Something bad happened');
    const state = generateStateFromActions(shellReducer,
      ShellActions.loadLatestVersion.failed(error)
    )

    expect(state.loadVersionError).toEqual(error);
  })

  it('should have error when loading latest packages fails', () => {
    const error = new Error('Something bad happened');
    const state = generateStateFromActions(shellReducer,
      ShellActions.loadLatestPackages.failed(error)
    )

    expect(state.loadPackagesError).toEqual(error);
  })

  it('should have error when downloading package fails', () => {
    const error = new Error('Something bad happened');
    const state = generateStateFromActions(shellReducer,
      ShellActions.downloadPackage.failed(error)
    )

    expect(state.downloadPackageError).toEqual(error);
  })

  it('should clear error when getting latest version successful', () => {
    const error = new Error('idk');

    const state = generateStateFromActions(shellReducer,
      ShellActions.loadLatestVersion.failed(error),
      ShellActions.loadLatestVersion.success(ModelFactory.createApplicationVersion())
    );

    expect(state.loadVersionError).toEqual(null);
  })

  it('should clear error when getting latest packages successful', () => {
    const error = new Error('idk');

    const state = generateStateFromActions(shellReducer,
      ShellActions.loadLatestPackages.failed(error),
      ShellActions.loadLatestPackages.success(ModelFactory.createListResult())
    );

    expect(state.loadPackagesError).toEqual(null);
  })

  it('should clear error when downloading package successful', () => {
    const error = new Error('idk');

    const state = generateStateFromActions(shellReducer,
      ShellActions.downloadPackage.failed(error),
      ShellActions.downloadPackage.success()
    );

    expect(state.downloadPackageError).toEqual(null);
  })
})
