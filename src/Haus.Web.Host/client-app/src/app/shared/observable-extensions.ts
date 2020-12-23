import {Observable, ReplaySubject, Subject} from "rxjs";
import {shareReplay, switchMap, take} from "rxjs/operators";

export function subscribeOnce<T>(obs$: Observable<T>, handler: (value: T) => void = (() => {}), errorHandler: (err: any) => void = (() => {})): Observable<T> {
  const subject = new ReplaySubject<T>(1);
  obs$.pipe(take(1)).subscribe({
    next: value => {
      handler(value);
      subject.next(value);
    },
    error: err => {
      errorHandler(err);
      subject.error(err);
    },
    complete: () => {
      subject.complete();
    }
  })
  return subject.asObservable();
}

export function fromObservableToPromise<T>(observable: Observable<T>): Promise<T> {
  return new Promise((resolve, reject) => {subscribeOnce(observable, resolve, reject)});
}
