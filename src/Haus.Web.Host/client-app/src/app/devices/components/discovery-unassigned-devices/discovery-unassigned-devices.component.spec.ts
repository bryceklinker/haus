import {By} from "@angular/platform-browser";
import {CdkDrag, CdkDropList} from "@angular/cdk/drag-drop";
import {DiscoveryUnassignedDevicesComponent} from "./discovery-unassigned-devices.component";
import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DevicesModule} from "../../devices.module";

describe('DiscoveryUnassignedDevicesComponent', () => {
  it('should show each device', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
    ]
    const {container, queryAllByTestId} = await renderUnassignedDevices({devices});

    expect(queryAllByTestId('unassigned-device-item')).toHaveLength(3);
    expect(container).toHaveTextContent(devices[0].name);
    expect(container).toHaveTextContent(devices[1].name);
    expect(container).toHaveTextContent(devices[2].name);
  })

  it('should allow each device to be dragged', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ]
    const {fixture} = await renderUnassignedDevices({devices});

    expect(fixture.debugElement.queryAll(By.directive(CdkDrag))).toHaveLength(3);
  })

  it('should have drop list', async () => {
    const {fixture} = await renderUnassignedDevices({roomIds: [1, 2, 4]});

    const dropList = fixture.debugElement.query(By.directive(CdkDropList));
    expect(dropList.nativeElement).toBeInTheDocument();
    expect(dropList.nativeElement.getAttribute('ng-reflect-connected-to')).toEqual('1,2,4')
  })

  function renderUnassignedDevices(props: Partial<DiscoveryUnassignedDevicesComponent> = {}) {
    return renderFeatureComponent(DiscoveryUnassignedDevicesComponent, {
      imports: [DevicesModule],
      componentProperties: props
    })
  }
})
