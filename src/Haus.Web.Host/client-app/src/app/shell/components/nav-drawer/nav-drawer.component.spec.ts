import {NavDrawerComponent} from "./nav-drawer.component";
import {MatSidenav} from "@angular/material/sidenav";
import {eventually, ModelFactory, renderAppComponent, TestingEventEmitter, TestingServer} from "../../../../testing";
import {FeatureName} from "../../../shared/features";

describe('NavDrawerComponent', () => {
  let drawerClosed: TestingEventEmitter;

  beforeEach(() => {
    TestingServer.setupGet('/api/features', ModelFactory.createListResult());

    drawerClosed = new TestingEventEmitter();
  })

  it('should notify when sidenav closes', async () => {
    const {triggerEventHandler} = await renderAppComponent(NavDrawerComponent, {
      componentProperties: {drawerClosed}
    });

    triggerEventHandler(MatSidenav, 'openedChange', false);

    expect(drawerClosed.emit).toHaveBeenCalled()
  });

  it('should not notify sidenav closed when opened', async () => {
    const {triggerEventHandler} = await renderAppComponent(NavDrawerComponent, {
      componentProperties: {drawerClosed}
    });

    triggerEventHandler(MatSidenav, 'openedChange', true);

    expect(drawerClosed.emit).not.toHaveBeenCalled()
  })

  it('should show available routes only', async () => {
    const {container} = await renderAppComponent(NavDrawerComponent);

    expect(container).toHaveTextContent('Diagnostics');
    expect(container).toHaveTextContent('Devices');
    expect(container).toHaveTextContent('Rooms');
    expect(container).not.toHaveTextContent('Device Simulator');
  })

  it('should hide routes for features that are not enabled', async () => {
    TestingServer.setupGet('/api/features', ModelFactory.createListResult(FeatureName.DeviceSimulator));

    const {container, detectChanges} = await renderAppComponent(NavDrawerComponent);

    await eventually(() => {
      detectChanges();
      expect(container).toHaveTextContent('Device Simulator');
    })
  })
})
