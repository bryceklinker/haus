import {Params} from "@angular/router";

export interface RouterUrlState {
  url: string;
  queryParams: Params;
  params: Params;
}
