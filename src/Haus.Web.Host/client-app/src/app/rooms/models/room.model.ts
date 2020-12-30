import {LightingModel} from "../../shared/models";

export interface CreateRoomModel {
  name: string;
}

export interface RoomModel extends CreateRoomModel {
  id: number;
  lighting: LightingModel;
}
