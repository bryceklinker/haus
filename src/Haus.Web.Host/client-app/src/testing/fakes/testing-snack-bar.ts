import {MatSnackBar} from "@angular/material/snack-bar";
import {Injectable} from "@angular/core";

@Injectable()
export class TestingSnackBar extends MatSnackBar {
  constructor() {
    super(<any>{
      position: jest.fn()
    },
      <any>jest.fn(),
      <any>jest.fn(),
      <any>jest.fn(),
      <any>jest.fn(),
      <any>jest.fn()
    );

    jest.spyOn(this as TestingSnackBar, 'open');
    jest.spyOn(this  as TestingSnackBar, 'openFromComponent').mockReturnValue({global: jest.fn()} as any);
    jest.spyOn(this  as TestingSnackBar, 'openFromTemplate');
  }
}
