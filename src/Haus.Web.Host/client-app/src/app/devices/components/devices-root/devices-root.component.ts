import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {DeviceModel} from "../../../shared/devices";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {DevicesActions, selectAllDevices} from "../../state";

@Component({
  selector: 'devices-root',
  templateUrl: './devices-root.component.html',
  styleUrls: ['./devices-root.component.scss']
})
export class DevicesRootComponent implements OnInit {
  devices$: Observable<DeviceModel[]>;

  constructor(private store: Store<AppState>) {
    this.devices$ = this.store.select(selectAllDevices);
  }

  ngOnInit(): void {
    this.store.dispatch(DevicesActions.loadDevices.request());
  }
}
