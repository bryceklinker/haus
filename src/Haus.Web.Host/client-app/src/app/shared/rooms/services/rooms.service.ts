import {Injectable, OnDestroy, OnInit} from "@angular/core";
import {BehaviorSubject, Observable} from "rxjs";
import {filter, map, tap, withLatestFrom} from "rxjs/operators";
import {ActivatedRoute} from "@angular/router";

import {RoomModel} from "../models";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {DestroyableSubject} from "../../destroyable-subject";
import {DeviceModel} from "../../devices";

const ROOM_ID_ROUTE_PARAM = 'roomId';
@Injectable({
  providedIn: 'root'
})
export class RoomsService implements OnInit, OnDestroy {
  private readonly _entityService = new SortingEntityService<RoomModel>(r => r.name);
  private readonly devicesByRoomSubject = new BehaviorSubject<{[roomId: string]: Array<DeviceModel>}>({});
  private readonly destroyable = new DestroyableSubject();

  get selectedRoomId$() : Observable<string> {
    return this.destroyable.register(this.route.paramMap.pipe(
      map(paramMap =>  paramMap.get(ROOM_ID_ROUTE_PARAM) || ''),
      filter(roomId => roomId !== '')
    ));
  }

  get rooms$(): Observable<RoomModel[]> {
    return this.destroyable.register(this._entityService.entitiesArray$);
  }

  get selectedRoom$(): Observable<RoomModel | null> {
    return this.destroyable.register(this._entityService.entitiesById$.pipe(
      withLatestFrom(this.selectedRoomId$),
      map(([roomsById, roomId]) => roomId ? roomsById[roomId] : null),
    ));
  }

  get selectedRoomDevices$(): Observable<Array<DeviceModel>> {
    return this.destroyable.register(this.devicesByRoomSubject.asObservable().pipe(
      withLatestFrom(this.selectedRoomId$),
      map(([devicesByRoomId, roomId]) => roomId ? devicesByRoomId[roomId] : [])
    ))
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

  ngOnInit(): void {
    this.destroyable.register(this.selectedRoomId$).subscribe(roomId => this.onRoomChanged(roomId))
  }

  private onRoomChanged(roomId: string) {
    this.destroyable.register(this.api.getDevicesInRoom(roomId).pipe(
      map(result => result.items),
      tap(devices => this.devicesByRoomSubject.next({
        ...this.devicesByRoomSubject.getValue(),
        [roomId]: devices
      }))
    )).subscribe();
  }
}
