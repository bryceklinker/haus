import {Injectable} from "@angular/core";
import {saveAs} from 'file-saver';

@Injectable()
export class SaveFileService {
  saveAs(blob: Blob | string, filename: string): void {
    saveAs(blob, filename);
  }
}
