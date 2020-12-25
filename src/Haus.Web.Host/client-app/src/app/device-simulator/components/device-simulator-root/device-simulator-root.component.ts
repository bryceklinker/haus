import {Component, OnDestroy, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";

import {DeviceModel} from "../../../shared/devices";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {DeviceSimulatorService} from "../../services";

@Component({
  selector: 'device-simulator-root',
  templateUrl: './device-simulator-root.component.html',
  styleUrls: ['./device-simulator-root.component.scss']
})
export class DeviceSimulatorRootComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();

  get devices$(): Observable<DeviceModel[]> {
    return this.destroyable.register(this.service.state$.pipe(
      map(s => s.devices)
    ))
  }

  constructor(private readonly service: DeviceSimulatorService) {
  }

  ngOnInit(): void {
    this.destroyable.register(this.service.start()).subscribe();
  }

  ngOnDestroy(): void {
    this.destroyable.register(this.service.stop()).subscribe();
    this.destroyable.destroy();
  }
}
