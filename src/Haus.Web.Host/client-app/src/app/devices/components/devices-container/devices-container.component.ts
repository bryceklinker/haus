import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {DeviceModel} from "../../models";
import {EntityCollectionService, EntityCollectionServiceFactory} from "@ngrx/data";
import {ENTITY_NAMES} from "../../../entity-metadata";

@Component({
  selector: 'devices-container',
  templateUrl: './devices-container.component.html',
  styleUrls: ['./devices-container.component.scss']
})
export class DevicesContainerComponent implements OnInit {
  private readonly service: EntityCollectionService<DeviceModel>;
  devices$: Observable<DeviceModel[]> | null = null;

  constructor(private factory: EntityCollectionServiceFactory) {
    this.service = factory.create<DeviceModel>(ENTITY_NAMES.Device);
    this.devices$ = this.service.entities$;
  }

  ngOnInit(): void {
    this.service.getAll();
  }
}
