import {
  DefaultDataService,
  HttpUrlGenerator,
  DefaultDataServiceConfig,
  DefaultDataServiceFactory,
  EntityCollectionDataService
} from "@ngrx/data";
import {Injectable, Optional} from "@angular/core";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {HttpClient} from "@angular/common/http";
import {ListResult} from "../models";

export class HausDataService<T> extends DefaultDataService<T> {
  constructor(entityName: string, http: HttpClient, httpUrlGenerator: HttpUrlGenerator, config?: DefaultDataServiceConfig) {
    super(entityName, http, httpUrlGenerator, config);
  }
  getAll(): Observable<T[]> {
    return super.getAll().pipe(
      map(r => <any>r),
      map((result: ListResult<T>) => result.items)
    );
  }
}

@Injectable()
export class HausDataServiceFactory extends DefaultDataServiceFactory {
  constructor(http: HttpClient, httpUrlGenerator: HttpUrlGenerator, @Optional() config?: DefaultDataServiceConfig) {
    super(http, httpUrlGenerator, config);
  }

  create<T>(entityName: string): EntityCollectionDataService<T> {
    return new HausDataService<T>(entityName, this.http, this.httpUrlGenerator, this.config);
  }
}
