import { Injectable } from "@angular/core";
import {SaveFileService} from "../../app/shared/save-file.service";

@Injectable()
export class TestingSaveFileService extends SaveFileService {

  constructor() {
    super();

    jest.spyOn(this as TestingSaveFileService, 'saveAs');
  }

  saveAs(blob: Blob | string, filename: string) {

  }
}
