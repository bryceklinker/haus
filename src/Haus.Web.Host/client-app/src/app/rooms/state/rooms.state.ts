import {EntityState} from "@ngrx/entity";
import {RoomModel} from "../../shared/rooms";

export interface RoomsState extends EntityState<RoomModel> {
  isAdding: boolean;
}
