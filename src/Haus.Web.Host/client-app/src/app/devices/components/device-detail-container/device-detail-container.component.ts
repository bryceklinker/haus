import {Component} from "@angular/core";
import {DeviceModel} from "../../models";
import {Observable} from "rxjs";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {selectDeviceById} from "../../reducers/devices.reducer";

@Component({
  selector: 'device-detail-container',
  templateUrl: './device-detail-container.component.html',
  styleUrls: ['./device-detail-container.component.scss']
})
export class DeviceDetailContainerComponent {
  selectedDevice$: Observable<DeviceModel | null>;

  constructor(private store: Store<AppState>) {
    this.selectedDevice$ = store.select(selectDeviceById);
  }
}
