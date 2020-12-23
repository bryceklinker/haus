import {Observable, Subject} from "rxjs";
import {takeUntil} from "rxjs/operators";

export function useDestroyable<T>(source$: Observable<T>, destroyable: DestroyableSubject): Observable<T> {
  return source$.pipe(
    takeUntil(destroyable)
  )
}
export class DestroyableSubject extends Subject<void> {
  register<T>(source$: Observable<T>): Observable<T> {
    return useDestroyable(source$, this);
  }

  destroy() {
    this.next();
    this.complete();
  }
}
