import {Component, EventEmitter, Input, Output} from "@angular/core";
import {DeviceModel} from "../../models";
import {DevicesModule} from "../../devices.module";
import {Router} from "@angular/router";

@Component({
  selector: 'devices-list',
  templateUrl: './devices-list.component.html',
  styleUrls: ['./devices-list.component.scss']
})
export class DevicesListComponent {
  @Input() devices: Array<DeviceModel> | null = [];
  @Output() deviceSelected = new EventEmitter<number>();

  constructor(private router: Router) {
  }
}
