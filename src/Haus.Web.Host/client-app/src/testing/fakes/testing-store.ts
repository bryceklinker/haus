import {Action, ActionsSubject, INITIAL_STATE, ReducerManager, StateObservable, Store} from "@ngrx/store";
import {FunctionIsNotAllowed} from "@ngrx/store/src/models";
import {Inject, Injectable} from "@angular/core";
import {MockState, MockStore} from "@ngrx/store/testing";

@Injectable()
export class TestingStore<TState> extends MockStore<TState> {
  private _actions: Array<Action>;

  get actions(): Array<Action> {
    return this._actions;
  }

  constructor(state$: MockState<TState>,
              actionsObserver: ActionsSubject,
              reducerManager: ReducerManager,
              @Inject(INITIAL_STATE) initialState: TState) {
    super(state$, actionsObserver, reducerManager, initialState, []);
    this._actions = [];
  }

  dispatch<V extends Action = Action>(action: V & FunctionIsNotAllowed<V, "Functions are not allowed to be dispatched. Did you forget to call the action creator function?">) {
    this._actions.push(action);
    // @ts-ignore
    super.dispatch(action);
  }
}
