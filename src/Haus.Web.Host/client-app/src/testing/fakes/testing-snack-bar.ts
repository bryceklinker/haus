import {MatSnackBar} from "@angular/material/snack-bar";
import {Injectable} from "@angular/core";

@Injectable()
export class TestingSnackBar extends MatSnackBar {
  constructor() {
    super(<any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn());

    spyOn(this, 'open');
    spyOn(this, 'openFromComponent');
    spyOn(this, 'openFromTemplate');
  }
}
