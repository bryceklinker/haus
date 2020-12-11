import {Component, OnInit} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {DevicesActions} from "../../actions";
import {Observable} from "rxjs";
import {DeviceModel} from "../../models";
import {selectDevices} from "../../reducers/devices.reducer";
import {RouterActions} from "../../../shared/routing/actions";

@Component({
  selector: 'devices-container',
  templateUrl: './devices-container.component.html',
  styleUrls: ['./devices-container.component.scss']
})
export class DevicesContainerComponent implements OnInit {
  devices$: Observable<DeviceModel[]>;

  constructor(private store: Store<AppState>) {
    this.devices$ = store.select(selectDevices);
  }

  ngOnInit(): void {
    this.store.dispatch(DevicesActions.load.request());
  }

  onDeviceSelected(deviceId: number) {
    this.store.dispatch(RouterActions.navigate(`/devices/${deviceId}`));
  }
}
