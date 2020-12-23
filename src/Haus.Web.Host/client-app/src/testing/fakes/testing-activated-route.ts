import {ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params} from "@angular/router";
import {Injectable} from "@angular/core";
import {BehaviorSubject, Observable, of} from "rxjs";
import {map} from "rxjs/operators";

import {TestingParamMap} from "./testing-param-map";

class TestingActivatedRouteSnapshot extends ActivatedRouteSnapshot {

}

@Injectable()
export class TestingActivatedRoute extends ActivatedRoute {
  private _paramsSubject = new BehaviorSubject<Params>({});

  get paramMap(): Observable<ParamMap> {
    return this.params.pipe(
      map(params => new TestingParamMap(params))
    );
  }

  constructor() {
    super();
    this.url = of([]);
    this.fragment = of('');
    this.data = of({});
    this.queryParams = of({});
    this.params = this._paramsSubject.asObservable();
    this.snapshot = new ActivatedRouteSnapshot();
  }

  triggerParamsChange(params: {[key: string]: string}) {
    this._paramsSubject.next(params);
  }
}
