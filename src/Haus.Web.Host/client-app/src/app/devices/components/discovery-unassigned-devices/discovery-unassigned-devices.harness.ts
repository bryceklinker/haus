import {screen} from "@testing-library/dom";
import {CdkDrag, CdkDropList} from "@angular/cdk/drag-drop";
import {By} from "@angular/platform-browser";

import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {DiscoveryUnassignedDevicesComponent} from "./discovery-unassigned-devices.component";
import {DevicesModule} from "../../devices.module";

export class DiscoveryUnassignedDevicesHarness extends HausComponentHarness<DiscoveryUnassignedDevicesComponent> {
  get unassignedDevices() {
    return screen.queryAllByTestId('unassigned-device-item');
  }

  get draggableDevices() {
    return this.fixture.debugElement.queryAll(By.directive(CdkDrag));
  }

  get unassignedDevicesZone() {
    return this.fixture.debugElement.query(By.directive(CdkDropList)).nativeElement;
  }

  get zonesUnassignedDevicesIsConnectedTo() {
    return this.unassignedDevicesZone.getAttribute('ng-reflect-connected-to');
  }

  static async render(props: Partial<DiscoveryUnassignedDevicesComponent>) {
    const result = await renderFeatureComponent(DiscoveryUnassignedDevicesComponent, {
      imports: [DevicesModule],
      componentProperties: props
    });

    return new DiscoveryUnassignedDevicesHarness(result);
  }
}
