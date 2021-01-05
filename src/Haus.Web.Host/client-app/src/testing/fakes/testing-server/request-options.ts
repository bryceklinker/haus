import {HttpStatusCodes} from "../../../app/shared/rest-api";

export interface RequestOptions {
  status?: HttpStatusCodes,
  delay?: number;
}
export const DEFAULT_DELAY = 0;
export const DEFAULT_POST_STATUS = HttpStatusCodes.NoContent;
export const DEFAULT_GET_STATUS = HttpStatusCodes.OK;
