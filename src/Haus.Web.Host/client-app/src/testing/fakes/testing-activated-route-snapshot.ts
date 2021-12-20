import {ActivatedRouteSnapshot} from '@angular/router';

export class TestingActivatedRouteSnapshot extends ActivatedRouteSnapshot {
  constructor() {
    super();
    this.url = [];
    this.params = {};
    this.queryParams = {};
    this.outlet = 'outlet';
  }
}
