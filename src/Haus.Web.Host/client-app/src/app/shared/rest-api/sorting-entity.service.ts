import {Sortable, sortArrayBy, SortDirection} from "../sort-array-by";
import {EntityService, EntitySet} from "./entity.service";

export class SortingEntityService<T> extends EntityService<T> {
    constructor(private readonly sortSelector: (i: T) => Sortable,
                private readonly sortDirection: SortDirection = SortDirection.Ascending,
                keySelector: (i: T) => any = ((i: any) => i.id)) {
        super(keySelector);
    }

  protected convertSetToArray(entities: EntitySet<T>): Array<T> {
    return sortArrayBy(super.convertSetToArray(entities), this.sortSelector, this.sortDirection);
  }
}
