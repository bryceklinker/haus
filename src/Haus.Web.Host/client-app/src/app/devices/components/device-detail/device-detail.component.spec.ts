import {DeviceModel} from "../../models";
import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DeviceDetailComponent} from "./device-detail.component";
import {DevicesModule} from "../../devices.module";

describe('DeviceDetailComponent', () => {
  it('should show device information', async () => {
    const device = ModelFactory.createDeviceModel({
      deviceType: 'LightSensor, MotionSensor, TemperatureSensor'
    });

    const {getByTestId} = await renderDeviceDetail(device);

    expect(getByTestId('device-name-field')).toHaveValue(device.name);
    expect(getByTestId('device-external-id-field')).toHaveValue(device.externalId);
    expect(getByTestId('device-type-field')).toHaveValue('LightSensor, MotionSensor, TemperatureSensor');
  })

  it('should show not available when no device provided', async () => {
    const {getByTestId} = await renderDeviceDetail();

    expect(getByTestId('device-name-field')).toHaveValue('N/A');
    expect(getByTestId('device-external-id-field')).toHaveValue('N/A');
  })

  it('should keep readonly data inputs disabled', async () => {
    const device = ModelFactory.createDeviceModel();

    const {getByTestId} = await renderDeviceDetail(device);

    expect(getByTestId('device-external-id-field')).toBeDisabled();
    expect(getByTestId('device-type-field')).toBeDisabled();
  })

  it('should be readonly when no device provided', async () => {
    const {getByTestId} = await renderDeviceDetail();

    expect(getByTestId('device-name-field')).toBeDisabled();
    expect(getByTestId('device-external-id-field')).toBeDisabled();
  })

  it('should show device metadata', async () => {
    const device = ModelFactory.createDeviceModel({
      metadata: [
        {key: 'Vendor', value: 'Philips'},
        {key: 'Model', value: '2342'}
      ]
    });

    const {queryAllByTestId, container} = await renderDeviceDetail(device);

    expect(queryAllByTestId('device-metadata')).toHaveLength(2);
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
