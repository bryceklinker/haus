import {DefaultUrlSerializer, Router} from "@angular/router";
import {Injectable, Injector} from "@angular/core";

@Injectable()
export class TestingRouter extends Router {
  constructor(injector: Injector) {
    super(jest.fn(), new DefaultUrlSerializer(), <any>jest.fn(), <any>jest.fn(), injector, <any>jest.fn(), <any>jest.fn(), []);
  }

  isActive = jest.fn();
  navigate = jest.fn();
  navigateByUrl = jest.fn();
}
