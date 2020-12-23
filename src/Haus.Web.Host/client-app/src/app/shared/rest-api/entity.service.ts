import {BehaviorSubject, Observable} from "rxjs";
import {map, shareReplay, takeUntil, tap} from "rxjs/operators";

import {ListResult} from "../models";
import {DestroyableSubject} from "../destroyable-subject";

export type EntitySet<T> = {[id: string]: T};
export class EntityService<T> {
  private readonly destroyable = new DestroyableSubject();
  private readonly entitiesSubject = new BehaviorSubject<EntitySet<T>>({});
  private get entitiesById(): EntitySet<T> {
    return this.entitiesSubject.getValue();
  }

  public get entitiesById$(): Observable<EntitySet<T>> {
    return this.destroyable.register(this.entitiesSubject.asObservable());
  }

  public get entitiesArray$(): Observable<Array<T>> {
    return this.destroyable.register(this.entitiesById$.pipe(
      map(entities => this.convertSetToArray(entities)),
    ));
  }

  constructor(private readonly keySelector: (i: T) => any = ((i: any) => i.id)) {
  }

  executeGetAll(getAll: () => Observable<ListResult<T>>): Observable<T[]> {
    return this.destroyable.register(getAll().pipe(
      tap(result => {
        const entities = this.mergeResultWithEntities(result, this.entitiesById);
        this.entitiesSubject.next(entities);
      }),
      map(() => this.convertSetToArray(this.entitiesById)),
    ))
  }

  executeAdd(add: () => Observable<T>): Observable<T> {
    return this.destroyable.register(add().pipe(
      tap(result => this.entitiesSubject.next(this.addOrUpdateEntity(result, this.entitiesById))),
    ));
  }

  destroy(): void {
    this.destroyable.destroy();
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

