import {Observable, Subscription} from "rxjs";
import {take} from "rxjs/operators";

export function fromObservableToPromise<T>(observable: Observable<T>): Promise<T> {
  return new Promise((resolve, reject) => {
    observable.pipe(take(1)).subscribe(
      resolve,
      reject
    );
  });
}
