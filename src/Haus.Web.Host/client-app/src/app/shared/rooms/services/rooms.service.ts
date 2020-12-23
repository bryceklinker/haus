import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {RoomModel} from "../models";
import {HausApiClient, SortingEntityService} from "../../rest-api";

@Injectable({
  providedIn: 'root'
})
export class RoomsService {
  private readonly _entityService = new SortingEntityService<RoomModel>(r => r.name);

  get rooms$(): Observable<RoomModel[]> {
    return this._entityService.entitiesArray$;
  }

  get isLoading$(): Observable<boolean> {
    return this.api.isLoading$;
  }

  constructor(private api: HausApiClient) {
  }

  getAll(): Observable<RoomModel[]> {
    return this._entityService.executeGetAll(() => this.api.getRooms());
  }

  add(room: RoomModel): Observable<RoomModel> {
    return this._entityService.executeAdd(() => this.api.addRoom(room));
  }
}
