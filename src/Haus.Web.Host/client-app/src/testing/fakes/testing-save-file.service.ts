import {SaveFileService} from "../../app/shared/save-file.service";

export class TestingSaveFileService extends SaveFileService {

  constructor() {
    super();

    spyOn(this, 'saveAs');
  }

  saveAs(blob: Blob | string, filename: string) {

  }
}
