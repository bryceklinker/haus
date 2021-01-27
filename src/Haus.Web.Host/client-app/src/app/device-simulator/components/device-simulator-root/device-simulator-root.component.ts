import {Component, OnDestroy, OnInit} from "@angular/core";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";

import {AppState} from "../../../app.state";
import {DeviceSimulatorActions, selectAllSimulatedDevices, selectIsDeviceSimulatorConnected} from "../../state";
import {SimulatedDeviceModel} from "../../../shared/models";

@Component({
  selector: 'device-simulator-root',
  templateUrl: './device-simulator-root.component.html',
  styleUrls: ['./device-simulator-root.component.scss']
})
export class DeviceSimulatorRootComponent implements OnInit, OnDestroy {
  simulatedDevices$: Observable<Array<SimulatedDeviceModel>>;
  isConnected$: Observable<boolean | undefined>;

  constructor(private readonly store: Store<AppState>) {
    this.simulatedDevices$ = store.select(selectAllSimulatedDevices);
    this.isConnected$ = store.select(selectIsDeviceSimulatorConnected);
  }

  ngOnInit(): void {
    this.store.dispatch(DeviceSimulatorActions.start());
  }

  ngOnDestroy(): void {
    this.store.dispatch(DeviceSimulatorActions.stop());
  }

  onOccupancyChange(model: SimulatedDeviceModel) {
    this.store.dispatch(DeviceSimulatorActions.triggerOccupancyChange.request(model));
  }
}
