import {BehaviorSubject, Observable} from "rxjs";
import {map} from "rxjs/operators";

import {ListResult} from "../models";
import {subscribeOnce} from "../observable-extensions";

export type EntitySet<T> = {[id: string]: T};
export class EntityService<T> {
  private readonly entitiesSubject = new BehaviorSubject<EntitySet<T>>({});

  private get entitiesById(): EntitySet<T> {
    return this.entitiesSubject.getValue();
  }

  public get entitiesById$(): Observable<EntitySet<T>> {
    return this.entitiesSubject.asObservable();
  }

  public get entitiesArray$(): Observable<Array<T>> {
    return this.entitiesById$.pipe(
      map(entities => this.convertSetToArray(entities))
    );
  }

  constructor(private readonly keySelector: (i: T) => any = ((i: any) => i.id)) {
  }

  executeGetAll(getAll: () => Observable<ListResult<T>>): Observable<T[]> {
    const getAll$ = subscribeOnce(getAll(), (result) => {
      const entities = this.mergeResultWithEntities(result, this.entitiesById);
      this.entitiesSubject.next(entities);
    })
    return getAll$.pipe(map(() => this.convertSetToArray(this.entitiesById)));
  }

  executeAdd(add: () => Observable<T>): Observable<T> {
    return subscribeOnce(add(), (result) => {
      this.entitiesSubject.next(this.addOrUpdateEntity(result, this.entitiesById));
      return result;
    });
  }

  protected convertSetToArray(entities: EntitySet<T>): Array<T> {
    return Object.keys(entities)
      .filter(key => entities.hasOwnProperty(key))
      .map(key => entities[key]);
  }

  private addOrUpdateEntity(entity: T, entities: EntitySet<T>): EntitySet<T> {
    return {
      ...entities,
      [this.keySelector(entity)]: entity
    }
  }

  private mergeResultWithEntities(result: ListResult<T>, entities: EntitySet<T>): EntitySet<T> {
    const merged = {...entities};
    for (const item of result.items) {
      merged[this.keySelector(item)] = item;
    }
    return merged;
  }
}

