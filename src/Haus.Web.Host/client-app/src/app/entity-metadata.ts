import {EntityDataModuleConfig} from "@ngrx/data";


export const ENTITY_NAMES = {
  Room: 'Room',
  Device: 'Device'
}

export const ENTITY_METADATA: EntityDataModuleConfig = {
  entityMetadata: {
    [ENTITY_NAMES.Room]: {},
    [ENTITY_NAMES.Device]: {}
  }
}
