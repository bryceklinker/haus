import {ModelFactory} from "../../../../testing";
import {DevicesRootComponent} from "./devices-root.component";
import {DevicesActions} from "../../state";
import {DevicesRootHarness} from "./devices-root.harness";

describe('DevicesRootComponent', () => {
  test('should get all devices when rendered', async () => {
    const harness = await DevicesRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(DevicesActions.loadDevices.request());
  })

  test('should show all devices', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];

    const harness = await DevicesRootHarness.render(
      DevicesActions.loadDevices.success(ModelFactory.createListResult(...devices))
    );

    expect(harness.deviceItems).toHaveLength(3);
  })
})
