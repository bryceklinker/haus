import {ApplicationPackageModel, ApplicationVersionModel} from "../../shared/models";

export interface ShellState {
  latestVersion: ApplicationVersionModel | null;
  latestPackages: Array<ApplicationPackageModel>;
  loadVersionError: any;
  loadPackagesError: any;
  downloadPackageError: any;
}
