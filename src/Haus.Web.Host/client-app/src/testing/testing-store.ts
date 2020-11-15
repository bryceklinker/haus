import {Action, ActionsSubject, ReducerManager, StateObservable, Store} from "@ngrx/store";
import {FunctionIsNotAllowed} from "@ngrx/store/src/models";
import {Observable} from "rxjs";
import {Injectable} from "@angular/core";

@Injectable()
export class TestingStore<TState> extends Store<TState> {
  private _actions: Array<Action>;

  get actions(): Array<Action> {
    return this._actions;
  }

  constructor(state$: StateObservable, actionsObserver: ActionsSubject, reducerManager: ReducerManager) {
    super(state$, actionsObserver, reducerManager);
    this._actions = [];
  }

  dispatch<V extends Action = Action>(action: V & FunctionIsNotAllowed<V, "Functions are not allowed to be dispatched. Did you forget to call the action creator function?">) {
    this._actions.push(action);
  }
}
