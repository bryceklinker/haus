import {MatDialog} from "@angular/material/dialog";
import {Injectable} from "@angular/core";

@Injectable()
export class TestingMatDialog extends MatDialog {
  constructor() {
    super(<any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), jest.fn(), <any>jest.fn(), <any>jest.fn());
    spyOn(this, "open");
  }
}
