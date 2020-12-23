import {ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params} from "@angular/router";
import {Injectable, OnDestroy} from "@angular/core";
import {BehaviorSubject, Observable, of} from "rxjs";
import {distinctUntilChanged, map, shareReplay} from "rxjs/operators";

import {TestingParamMap} from "./testing-param-map";
import {DestroyableSubject} from "../../app/shared/destroyable-subject";

class TestingActivatedRouteSnapshot extends ActivatedRouteSnapshot {

}

@Injectable()
export class TestingActivatedRoute extends ActivatedRoute implements OnDestroy{
  private readonly destroyable = new DestroyableSubject();
  private _paramsSubject = new BehaviorSubject<Params>({});

  get paramMap(): Observable<ParamMap> {
    return this.destroyable.register(this.params.pipe(
      map(params => new TestingParamMap(params))
    ));
  }

  constructor() {
    super();
    this.url = of([]);
    this.fragment = of('');
    this.data = of({});
    this.queryParams = of({});
    this.params = this._paramsSubject;
    this.snapshot = new ActivatedRouteSnapshot();
  }

  triggerParamsChange(params: { [key: string]: string }) {
    this._paramsSubject.next(params);
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }

}
