import {Component} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";
import {ActivatedRoute} from "@angular/router";
import {map, mergeMap} from "rxjs/operators";

import {AppState} from "../../../app.state";
import {DevicesActions, selectDeviceById} from "../../state";
import {DeviceLightingConstraintsChangedEvent, DeviceModel} from "../../../shared/models";

@Component({
  selector: 'device-detail-root',
  templateUrl: './device-detail-root.component.html',
  styleUrls: ['./device-detail-root.component.scss']
})
export class DeviceDetailRootComponent {
  selectedDevice$: Observable<DeviceModel | undefined | null>;

  constructor(private store: Store<AppState>, private route: ActivatedRoute) {
    this.selectedDevice$ = this.route.paramMap.pipe(
      map(paramMap => paramMap.get('deviceId') || ''),
      mergeMap(deviceId => this.store.select(selectDeviceById(deviceId)))
    )
  }

  onSaveLightingConstraints($event: DeviceLightingConstraintsChangedEvent) {
    this.store.dispatch(DevicesActions.changeDeviceLightingConstraints.request($event));
  }
}
