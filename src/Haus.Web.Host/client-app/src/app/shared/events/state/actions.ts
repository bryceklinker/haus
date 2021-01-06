import {
  DeviceCreatedEvent, DeviceLightingChangedEvent, DevicesAssignedToRoomEvent,
  DeviceUpdatedEvent,
  DiscoveryStartedEvent,
  DiscoveryStoppedEvent,
  HausEvent,
  RoomCreatedEvent,
  RoomLightingChangedEvent,
  RoomUpdatedEvent
} from "../../models";
import {createAction} from "@ngrx/store";
import {KNOWN_EVENT_TYPES} from "./known-event-types";

export const EventsActions = {
  fromHausEvent: <T>(hausEvent: HausEvent<T>) => ({type: hausEvent.type, payload: hausEvent.payload}),
  roomLightingChanged: createAction(KNOWN_EVENT_TYPES.ROOM_LIGHTING_CHANGED, (payload: RoomLightingChangedEvent) => ({payload})),
  roomCreated: createAction(KNOWN_EVENT_TYPES.ROOM_CREATED, (payload: RoomCreatedEvent) => ({payload})),
  roomUpdated: createAction(KNOWN_EVENT_TYPES.ROOM_UPDATED, (payload: RoomUpdatedEvent) => ({payload})),
  deviceCreated: createAction(KNOWN_EVENT_TYPES.DEVICE_CREATED, (payload: DeviceCreatedEvent) => ({payload})),
  deviceUpdated: createAction(KNOWN_EVENT_TYPES.DEVICE_UPDATED, (payload: DeviceUpdatedEvent) => ({payload})),
  deviceLightingChanged: createAction(KNOWN_EVENT_TYPES.DEVICE_LIGHTING_CHANGED, (payload: DeviceLightingChangedEvent) => ({payload})),
  devicesAssignedToRoom: createAction(KNOWN_EVENT_TYPES.DEVICES_ASSIGNED_TO_ROOM, (payload: DevicesAssignedToRoomEvent) => ({payload})),
  discoveryStarted: createAction(KNOWN_EVENT_TYPES.DISCOVERY_STARTED, (payload: DiscoveryStartedEvent) => ({payload})),
  discoveryStopped: createAction(KNOWN_EVENT_TYPES.DISCOVERY_STOPPED, (payload: DiscoveryStoppedEvent) => ({payload}))
}
