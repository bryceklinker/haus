import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";
import {map, tap} from "rxjs/operators";

import {DestroyableSubject} from "../../destroyable-subject";
import {FeatureName} from "../feature-name";
import {ListResult} from "../../models";

@Injectable()
export class FeaturesService  {
  private readonly destroyable = new DestroyableSubject();
  private readonly enabledFeaturesSubject = new BehaviorSubject<FeatureName[]>([]);
  private isInitialized = false;

  get enabledFeatures$(): Observable<FeatureName[]> {
      return this.destroyable.register(this.enabledFeaturesSubject.asObservable());
  }

  constructor(private http: HttpClient) {
  }

  load(): Observable<FeatureName[]> {
    if (this.isInitialized) {
      return this.enabledFeaturesSubject.asObservable();
    }
    this.isInitialized = true;
    return this.destroyable.register(this.http.get<ListResult<FeatureName>>('/api/features').pipe(
      tap(result => this.enabledFeaturesSubject.next(result.items)),
      map(result => result.items)
    ));
  }

  isFeatureEnabled(featureName: FeatureName | undefined): boolean {
    if (!featureName) {
      return true;
    }

    return this.enabledFeaturesSubject.getValue().includes(featureName);
  }
}
