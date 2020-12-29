export enum SortDirection {
  Ascending = 'asc',
  Descending = 'desc'
}

export type Sortable = string | number;

export function sortArrayByAscending<T>(items: T[], property: (item: T) => Sortable) {
  return sortArrayBy(items, property, SortDirection.Ascending);
}

export function sortArrayByDescending<T>(items: T[], property: (item: T) => Sortable) {
  return sortArrayBy(items, property, SortDirection.Descending);
}

export function sortArrayBy<T>(items: T[], property: (item: T) => Sortable, direction: SortDirection): Array<T> {
  return [...items].sort(createComparer(property, direction));
}

export function createComparer<T>(property: (item: T) => any, sortDirection = SortDirection.Ascending): (a: T, b: T) => number {
  return (a: T, b: T) => {
    const propA = property(a);
    const propB = property(b);

    const valueA = propA instanceof String ? propA.toLowerCase() : propA;
    const valueB = propB instanceof String ? propB.toLowerCase() : propB;

    return sortDirection === SortDirection.Ascending
      ? ascendingComparer(valueA, valueB)
      : descendingComparer(valueA, valueB);
  }
}

function ascendingComparer(a: Sortable, b: Sortable): number {
  if (a < b)
    return -1;

  if (a > b)
    return 1;

  return 0;
}

function descendingComparer(a: Sortable, b: Sortable): number {
  if (b < a)
    return -1;

  if (b > a)
    return 1;

  return 0;
}
