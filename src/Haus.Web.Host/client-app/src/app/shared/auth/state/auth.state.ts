import {UserModel} from "../user.model";

export interface AuthState {
  user: UserModel | null;
}
