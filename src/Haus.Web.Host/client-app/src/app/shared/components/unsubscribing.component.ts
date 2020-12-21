import {OnDestroy} from "@angular/core";
import {Observable, PartialObserver, Subscription} from "rxjs";

export class UnsubscribingComponent implements OnDestroy {
  private readonly _subscriptions: Array<Subscription> = [];

  ngOnDestroy(): void {
    for (const subscription of this._subscriptions) {
      subscription.unsubscribe();
    }
  }

  protected safeSubscribe<T>(observable: Observable<T>, next: (value: T) => void) {
    this._subscriptions.push(observable.subscribe(next));
  }
}
