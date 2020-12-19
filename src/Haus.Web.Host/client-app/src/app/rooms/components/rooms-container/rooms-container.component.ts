import {Component, OnInit} from "@angular/core";
import {RoomModel} from "../../models/room.model";
import {Observable} from "rxjs";
import {EntityCollectionService, EntityCollectionServiceFactory} from "@ngrx/data";
import {ENTITY_NAMES} from "../../../entity-metadata";

@Component({
  selector: 'rooms-container',
  templateUrl: './rooms-container.component.html',
  styleUrls: ['./rooms-container.component.scss']
})
export class RoomsContainerComponent implements OnInit{
  private readonly service: EntityCollectionService<RoomModel>;
  rooms$: Observable<RoomModel[]> | null = null;

  constructor(private factory: EntityCollectionServiceFactory) {
    this.service = factory.create<RoomModel>(ENTITY_NAMES.Room);
    this.rooms$ = this.service.entities$;
  }

  ngOnInit(): void {
    this.service.getAll();
  }
}
