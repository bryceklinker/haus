import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {DeviceModel, DevicesService} from "../../../shared/devices";

@Component({
  selector: 'devices-root',
  templateUrl: './devices-root.component.html',
  styleUrls: ['./devices-root.component.scss']
})
export class DevicesRootComponent implements OnInit {
  devices$: Observable<DeviceModel[]>;

  constructor(private service: DevicesService) {
    this.devices$ = this.service.devices$;
  }

  ngOnInit(): void {
    this.service.getAll();
  }
}
