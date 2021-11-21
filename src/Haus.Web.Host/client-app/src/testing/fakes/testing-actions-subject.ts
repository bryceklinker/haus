import { Injectable } from "@angular/core";
import {Action, ActionsSubject} from "@ngrx/store";

@Injectable()
export class TestingActionsSubject extends ActionsSubject {
  publishedActions: Array<Action> = [];

  next(action: Action): void {
    this.publishedActions.push(action);
    super.next(action);
  }
}
