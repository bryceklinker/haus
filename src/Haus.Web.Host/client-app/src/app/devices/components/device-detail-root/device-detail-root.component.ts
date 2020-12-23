import {Component} from "@angular/core";
import {DeviceModel, DevicesService} from "../../../shared/devices";
import {Observable} from "rxjs";

@Component({
  selector: 'device-detail-root',
  templateUrl: './device-detail-root.component.html',
  styleUrls: ['./device-detail-root.component.scss']
})
export class DeviceDetailRootComponent {
  selectedDevice$: Observable<DeviceModel | null>;

  constructor(private service: DevicesService) {
    this.selectedDevice$ = service.selectedDevice$;
  }
}
