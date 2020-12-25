import {DestroyableSubject} from "../destroyable-subject";
import {Injectable, OnInit} from "@angular/core";
import {BehaviorSubject, Observable} from "rxjs";
import {FeatureNames} from "./feature-names";
import {HttpClient} from "@angular/common/http";
import {ListResult} from "../models";
import {map, tap} from "rxjs/operators";

@Injectable()
export class FeaturesService  {
  private readonly destroyable = new DestroyableSubject();
  private readonly enabledFeaturesSubject = new BehaviorSubject<FeatureNames[]>([]);
  get enabledFeatures$(): Observable<FeatureNames[]> {
      return this.destroyable.register(this.enabledFeaturesSubject.asObservable());
  }

  constructor(private http: HttpClient) {
  }

  load(): Observable<FeatureNames[]> {
    return this.destroyable.register(this.http.get<ListResult<FeatureNames>>('/api/features').pipe(
      tap(result => this.enabledFeaturesSubject.next(result.items)),
      map(result => result.items)
    ));
  }


}
