import {Component, OnDestroy, OnInit} from "@angular/core";
import {Observable, Subject} from "rxjs";
import {DeviceModel, DevicesService} from "../../../shared/devices";
import {takeUntil} from "rxjs/operators";
import {DestroyableSubject} from "../../../shared/destroyable-subject";

@Component({
  selector: 'devices-root',
  templateUrl: './devices-root.component.html',
  styleUrls: ['./devices-root.component.scss']
})
export class DevicesRootComponent implements OnInit, OnDestroy {
  private readonly _unsubscribe$ = new DestroyableSubject();
  devices$: Observable<DeviceModel[]>;

  constructor(private service: DevicesService) {
    this.devices$ = this.service.devices$;
  }

  ngOnInit(): void {
    this._unsubscribe$.register(this.service.getAll()).subscribe();
  }

  ngOnDestroy(): void {
    this._unsubscribe$.destroy();
  }
}
