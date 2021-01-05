import {screen} from "@testing-library/dom";
import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DeviceDetailComponent} from "./device-detail.component";
import {DevicesModule} from "../../devices.module";
import {DeviceModel, DeviceType} from "../../../shared/models";

describe('DeviceDetailComponent', () => {
  it('should show device information', async () => {
    const device = ModelFactory.createDeviceModel({
      deviceType: DeviceType.MotionSensor
    });

    await renderDeviceDetail(device);

    expect(screen.getByTestId('device-name-field')).toHaveValue(device.name);
    expect(screen.getByTestId('device-external-id-field')).toHaveValue(device.externalId);
    expect(screen.getByTestId('device-type-field')).toHaveValue(DeviceType.MotionSensor);
  })

  it('should show not available when no device provided', async () => {
    await renderDeviceDetail();

    expect(screen.getByTestId('device-name-field')).toHaveValue('N/A');
    expect(screen.getByTestId('device-external-id-field')).toHaveValue('N/A');
  })

  it('should keep readonly data inputs disabled', async () => {
    const device = ModelFactory.createDeviceModel();

    await renderDeviceDetail(device);

    expect(screen.getByTestId('device-external-id-field')).toBeDisabled();
    expect(screen.getByTestId('device-type-field')).toBeDisabled();
  })

  it('should be readonly when no device provided', async () => {
    await renderDeviceDetail();

    expect(screen.getByTestId('device-name-field')).toBeDisabled();
    expect(screen.getByTestId('device-external-id-field')).toBeDisabled();
  })

  it('should show device metadata', async () => {
    const device = ModelFactory.createDeviceModel({
      metadata: [
        {key: 'Vendor', value: 'Philips'},
        {key: 'Model', value: '2342'}
      ]
    });

    const {container} = await renderDeviceDetail(device);

    expect(screen.queryAllByTestId('device-metadata')).toHaveLength(2);
    expect(container).toHaveTextContent('Philips');
    expect(container).toHaveTextContent('2342');
  })

  function renderDeviceDetail(device?: DeviceModel) {
    return renderFeatureComponent(DeviceDetailComponent, {
      imports: [DevicesModule],
      componentProperties: {device}
    })
  }
})
