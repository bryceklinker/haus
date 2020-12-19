import {Component} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";

import {DeviceModel} from "../../models";
import {AppState} from "../../../app.state";
import {selectDeviceById} from "../../reducers/devices.reducer";

@Component({
  selector: 'device-detail-container',
  templateUrl: './device-detail-container.component.html',
  styleUrls: ['./device-detail-container.component.scss']
})
export class DeviceDetailContainerComponent {
  selectedDevice$: Observable<DeviceModel | undefined | null>;

  constructor(private store: Store<AppState>) {
    this.selectedDevice$ = this.store.select(selectDeviceById);
  }
}
