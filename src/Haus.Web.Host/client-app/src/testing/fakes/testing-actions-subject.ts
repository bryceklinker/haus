import {Action, ActionsSubject} from "@ngrx/store";

export class TestingActionsSubject extends ActionsSubject {
  publishedActions: Array<Action> = [];

  next(action: Action): void {
    this.publishedActions.push(action);
    super.next(action);
  }
}
