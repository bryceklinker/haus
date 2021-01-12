import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";
import {ActivatedRoute} from "@angular/router";
import {map, mergeMap} from "rxjs/operators";

import {AppState} from "../../../app.state";
import {DevicesActions, LightTypesActions, selectAllLightTypes, selectDeviceById} from "../../state";
import {DeviceModel, LightType} from "../../../shared/models";
import {DeviceLightingConstraintsModel} from "../../models";

@Component({
  selector: 'device-detail-root',
  templateUrl: './device-detail-root.component.html',
  styleUrls: ['./device-detail-root.component.scss']
})
export class DeviceDetailRootComponent implements OnInit {
  selectedDevice$: Observable<DeviceModel | undefined | null>;
  lightTypes$: Observable<Array<LightType>>;

  constructor(private store: Store<AppState>, private route: ActivatedRoute) {
    this.lightTypes$ = this.store.select(selectAllLightTypes);
    this.selectedDevice$ = this.route.paramMap.pipe(
      map(paramMap => paramMap.get('deviceId') || ''),
      mergeMap(deviceId => this.store.select(selectDeviceById(deviceId)))
    )
  }

  onSaveConstraints($event: DeviceLightingConstraintsModel) {
    this.store.dispatch(DevicesActions.changeDeviceLightingConstraints.request($event));
  }

  onSaveDevice($event: DeviceModel) {
    this.store.dispatch(DevicesActions.updateDevice.request($event));
  }

  ngOnInit(): void {
    this.store.dispatch(LightTypesActions.loadLightTypes.request());
  }
}
