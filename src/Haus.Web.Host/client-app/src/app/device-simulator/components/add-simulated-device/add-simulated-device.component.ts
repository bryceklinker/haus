import {Component, OnDestroy, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {DeviceSimulatorService} from "../../services";
import {DestroyableSubject} from "../../../shared/destroyable-subject";

@Component({
  selector: 'device-simulator-add-simulated-device',
  templateUrl: './add-simulated-device.component.html',
  styleUrls: ['./add-simulated-device.component.scss']
})
export class AddSimulatedDeviceComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  deviceTypes$: Observable<string[]>;

  constructor(private service: DeviceSimulatorService) {
    this.deviceTypes$ = this.destroyable.register(service.deviceTypes$);
  }

  ngOnInit(): void {
    this.destroyable.register(this.service.getDeviceTypes()).subscribe();
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }
}
