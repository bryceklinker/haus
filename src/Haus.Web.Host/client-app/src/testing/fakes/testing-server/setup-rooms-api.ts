import {ModelFactory} from '../../model-factory';
import {setupHttpGet, setupHttpPost, setupHttpPut} from './setup-http';
import {RequestOptions} from './request-options';
import {HttpStatusCodes} from '../../../app/shared/rest-api';
import {RoomModel} from '../../../app/shared/models';

const BASE_URL = '/api/rooms'

export function setupGetAllRooms(rooms: Array<RoomModel> = [], options?: RequestOptions) {
  setupHttpGet(BASE_URL, ModelFactory.createListResult(...rooms), options);
}

export function setupAddRoom(result: RoomModel = ModelFactory.createRoomModel(), options?: RequestOptions) {
  setupHttpPost(BASE_URL, result, {...options, status: HttpStatusCodes.Created});
}

export function setupUpdateRoom(roomId: number, options?: RequestOptions) {
  setupHttpPut(`${BASE_URL}/${roomId}`, null, {...options, status: HttpStatusCodes.NoContent});
}

export function setupChangeRoomLighting(roomId: number, options?: RequestOptions) {
  setupHttpPut(`${BASE_URL}/${roomId}/lighting`, null, options);
}

export function setupAssignDevicesToRoom(roomId: number, options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/${roomId}/add-devices`, null, options);
}
