import {Component} from "@angular/core";
import {DeviceModel} from "../../models";
import {Observable} from "rxjs";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {selectDeviceById} from "../../state";
import {ActivatedRoute} from "@angular/router";
import {map, mergeMap} from "rxjs/operators";

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
}
