import {Observable, Subscription} from "rxjs";
import {take} from "rxjs/operators";

export function fromObservableToPromise<T>(observable: Observable<T>): Promise<T> {
  return new Promise((resolve, reject) => {
    const sub: Subscription = observable.pipe(take(1)).subscribe(
      resolve,
      reject,
      () => sub.unsubscribe()
    );
  });
}
