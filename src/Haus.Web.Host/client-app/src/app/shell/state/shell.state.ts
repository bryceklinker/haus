import {ApplicationPackageModel, ApplicationVersionModel} from "../../shared/models";

export interface ShellState {
  latestVersion: ApplicationVersionModel | null;
  latestPackages: Array<ApplicationPackageModel>;
}
