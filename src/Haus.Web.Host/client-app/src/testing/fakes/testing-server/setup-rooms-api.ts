import {RoomModel} from "../../../app/rooms/models";
import {ModelFactory} from "../../model-factory";
import {setupHttpGet, setupHttpPost} from "./setup-http";
import {RequestOptions} from "./request-options";
import {DeviceModel} from "../../../app/devices/models";
import {HttpStatusCodes} from "../../../app/shared/rest-api";

const BASE_URL = '/api/rooms'

export function setupGetAllRooms(rooms: Array<RoomModel> = [], options?: RequestOptions) {
  setupHttpGet(BASE_URL, ModelFactory.createListResult(...rooms), options);
}

export function setupAddRoom(result: RoomModel = ModelFactory.createRoomModel(), options?: RequestOptions) {
  setupHttpPost(BASE_URL, result, {...options, status: HttpStatusCodes.Created});
}

export function setupChangeRoomLighting(roomId: number, options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/${roomId}/lighting`, null, options);
}
