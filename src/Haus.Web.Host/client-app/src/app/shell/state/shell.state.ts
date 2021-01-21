import {ApplicationVersionModel} from "../../shared/models";

export interface ShellState {
  latestVersion: ApplicationVersionModel | null;
}
