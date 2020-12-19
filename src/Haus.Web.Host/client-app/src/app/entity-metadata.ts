import {EntityDataModuleConfig} from "@ngrx/data";


export const ENTITY_NAMES = {
  Room: 'Room'
}

export const ENTITY_METADATA: EntityDataModuleConfig = {
  entityMetadata: {
    [ENTITY_NAMES.Room]: {}
  }
}
