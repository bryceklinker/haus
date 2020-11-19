import {signalrReducer} from "ngrx-signalr-core";

export interface AppState {
  signalr: ReturnType<typeof signalrReducer>;
}
