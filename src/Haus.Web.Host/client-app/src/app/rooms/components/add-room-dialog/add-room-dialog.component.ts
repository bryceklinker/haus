import {Component} from "@angular/core";
import {EntityCollectionService, EntityCollectionServiceFactory} from "@ngrx/data";
import {RoomModel} from "../../models/room.model";
import {ENTITY_NAMES} from "../../../entity-metadata";
import {FormBuilder, Validators} from "@angular/forms";

@Component({
  selector: 'add-room-dialog',
  templateUrl: './add-room-dialog.component.html',
  styleUrls: ['./add-room-dialog.component.scss']
})
export class AddRoomDialogComponent {
  private readonly service: EntityCollectionService<RoomModel>
  form = new FormBuilder().group({
    name: ['', Validators.required]
  });

  constructor(private factory: EntityCollectionServiceFactory) {
    this.service = factory.create(ENTITY_NAMES.Room);
  }

  onSave() {

    this.service.add(this.form.getRawValue());
  }
}
