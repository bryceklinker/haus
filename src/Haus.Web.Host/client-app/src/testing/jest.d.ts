import {Action} from "@ngrx/store";
import {EntityAction} from "@ngrx/data";

declare global {
  namespace jest {
    interface Matchers<R> {
      toContainEntityAction(entityAction: EntityAction): CustomMatcherResult;
    }
  }
}
