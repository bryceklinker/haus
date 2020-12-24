import {NavDrawerComponent} from "./nav-drawer.component";
import {MatSidenav} from "@angular/material/sidenav";
import {renderAppComponent, TestingEventEmitter} from "../../../../testing";
import {SettingsService} from "../../../shared/settings";

describe('NavDrawerComponent', () => {
  let drawerClosed: TestingEventEmitter;

  beforeEach(() => {
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
    spyOn(SettingsService, 'getSettings').and.returnValue({
      auth: {},
      deviceSimulator: {
        isEnabled: false
      }
    })

    const {container} = await renderAppComponent(NavDrawerComponent);

    expect(container).toHaveTextContent('Diagnostics');
    expect(container).toHaveTextContent('Devices');
    expect(container).toHaveTextContent('Rooms');
    expect(container).not.toHaveTextContent('Device Simulator');
  })

  it('should show device simulator route when available', async () => {
    spyOn(SettingsService, 'getSettings').and.returnValue({
      auth: {},
      deviceSimulator: {
        isEnabled: true
      }
    })

    const {container} = await renderAppComponent(NavDrawerComponent);

    expect(container).toHaveTextContent('Device Simulator');
  })
})
