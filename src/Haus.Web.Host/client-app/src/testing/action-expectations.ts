import {EntityAction} from "@ngrx/data";
import {Action} from "@ngrx/store";
import CustomMatcherResult = jest.CustomMatcherResult;

expect.extend({
  toContainEntityAction: (actions: Array<Action>, expectedAction: EntityAction): CustomMatcherResult => ({
    message: () => `expected that ${JSON.stringify(actions)} to contain ${JSON.stringify(expectedAction)}`,
    pass: actions
      .filter(a => a.type === expectedAction.type)
      .filter(a => (<EntityAction>a).payload.entityName === expectedAction.payload.entityName)
      .filter(a => (<EntityAction>a).payload.data === expectedAction.payload.data)
      .some(a => (<EntityAction>a).payload.entityOp === expectedAction.payload.entityOp)
})
})
