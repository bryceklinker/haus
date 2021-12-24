import {Component, Input, EventEmitter, Output, OnChanges, SimpleChanges} from '@angular/core';
import {DeviceModel, LightingModel, RoomModel, RoomLightingChangedEvent} from '../../../shared/models';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
  selector: 'room-detail',
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent implements OnChanges {
  @Input() room: RoomModel | undefined | null = null;
  @Input() devices: Array<DeviceModel> | undefined | null = [];
  @Output() lightingChange = new EventEmitter<RoomLightingChangedEvent>();
  @Output() assignDevices = new EventEmitter<RoomModel>();
  @Output() saveRoom = new EventEmitter<RoomModel>();
  form = new FormGroup({});

  get lighting(): LightingModel | null {
    return this.room && this.room.lighting ? this.room.lighting : null;
  }

  get name(): string {
    return this.room ? this.room.name : 'N/A';
  }

  get occupancyTimeoutInSeconds(): number {
    return this.room ? this.room.occupancyTimeoutInSeconds : 0;
  }

  constructor(private readonly formBuilder: FormBuilder) {

  }

  ngOnChanges(changes: SimpleChanges): void {
    this.form = this.formBuilder.group({
      name: [
        {
          value: this.name,
          disabled: this.room === null,
        },
        [
          Validators.required
        ]
      ],
      occupancyTimeoutInSeconds: [
        {
          value: this.occupancyTimeoutInSeconds,
          disabled: this.room === null
        },
        [
          Validators.required,
          Validators.min(0),
        ]
      ]
    });
  }

  onLightingChanged($event: LightingModel) {
    if (!this.room) {
      return;
    }

    this.lightingChange.emit({
      room: this.room,
      lighting: $event
    });
  }

  onAssignDevices() {
    if (!this.room) {
      return;
    }
    this.assignDevices.emit(this.room);
  }

  onSaveRoom() {
    if (!this.room) {
      return;
    }

    this.saveRoom.emit({
      ...this.room,
      ...this.form.getRawValue()
    });
  }
}
