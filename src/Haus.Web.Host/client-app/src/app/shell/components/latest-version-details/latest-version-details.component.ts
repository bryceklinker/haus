import {Component, Input, Output, EventEmitter} from "@angular/core";
import {ApplicationPackageModel, ApplicationVersionModel} from "../../../shared/models";
import {HausApiClient} from "../../../shared/rest-api";

@Component({
  selector: 'latest-version-details',
  templateUrl: './latest-version-details.component.html',
  styleUrls: ['./latest-version-details.component.scss']
})
export class LatestVersionDetailsComponent {
  @Input() version: ApplicationVersionModel | null = null;
  @Input() packages: Array<ApplicationPackageModel> | null = [];
  @Output() downloadPackage = new EventEmitter<ApplicationPackageModel>();

  get versionNumber(): string {
    return this.version ? this.version.version : 'N/A';
  }

  get description(): string {
    return this.version ? this.version.description : 'N/A';
  }

  get isStableRelease(): boolean {
    return this.version ? this.version.isOfficialRelease : false;
  }

  get isNewerVersion(): boolean {
    return this.version ? this.version.isNewer : false;
  }

  get releaseDate(): string {
    return this.version ? this.version.creationDate : 'N/A';
  }

  onDownloadPackage(model: ApplicationPackageModel) {
    this.downloadPackage.emit(model);
  }
}
