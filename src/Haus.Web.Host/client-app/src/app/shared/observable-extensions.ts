import {Observable} from "rxjs";
import {take} from "rxjs/operators";

export function subscribeOnce<T>(obs$: Observable<T>, handler: (value: T) => void = (() => {})): Observable<T> {
  obs$.pipe(
    take(1)
  ).subscribe({
    next: handler
  })
  return obs$;
}
