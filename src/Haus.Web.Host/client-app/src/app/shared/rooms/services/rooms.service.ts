import {Injectable, OnDestroy} from "@angular/core";
import {Observable} from "rxjs";
import {RoomModel} from "../models";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {map, takeUntil, withLatestFrom} from "rxjs/operators";
import {ActivatedRoute} from "@angular/router";
import {DestroyableSubject} from "../../destroyable-subject";

@Injectable({
  providedIn: 'root'
})
export class RoomsService implements OnDestroy {
  private readonly _entityService = new SortingEntityService<RoomModel>(r => r.name);
  private readonly destroyable = new DestroyableSubject();

  get rooms$(): Observable<RoomModel[]> {
    return this.destroyable.register(this._entityService.entitiesArray$);
  }

  get selectedRoom$(): Observable<RoomModel | null> {
    return this.destroyable.register(this._entityService.entitiesById$.pipe(
      withLatestFrom(this.route.paramMap),
      map(([roomsById, paramMap]) => {
        const roomId = paramMap.has('roomId') ? paramMap.get('roomId') : null;
        return roomId ? roomsById[roomId] : null;
      }),
    ));
  }

  get isLoading$(): Observable<boolean> {
    return this.destroyable.register(this.api.isLoading$);
  }

  constructor(private api: HausApiClient,
              private route: ActivatedRoute) {
  }

  getAll(): Observable<RoomModel[]> {
    return this._entityService.executeGetAll(() => this.api.getRooms());
  }

  add(room: RoomModel): Observable<RoomModel> {
    return this._entityService.executeAdd(() => this.api.addRoom(room));
  }

  ngOnDestroy(): void {
    this._entityService.destroy();
    this.destroyable.destroy();
  }
}
