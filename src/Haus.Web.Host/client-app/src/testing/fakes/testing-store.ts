import {Action, Store, StateObservable, ActionsSubject, ReducerManager} from "@ngrx/store";
import {Injectable} from "@angular/core";

@Injectable()
export class TestingStore<T> extends Store<T> {
  readonly dispatchedActions: Array<Action> = [];


  constructor(state$: StateObservable, actionsObserver: ActionsSubject, reducerManager: ReducerManager) {
    super(state$, actionsObserver, reducerManager);
  }

  dispatch(action: Action) {
    this.dispatchedActions.push(action);
  }
}
