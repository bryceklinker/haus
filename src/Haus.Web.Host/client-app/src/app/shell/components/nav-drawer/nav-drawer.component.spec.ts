import {NavDrawerComponent} from "./nav-drawer.component";
import {TestingEventEmitter, TestingSettingsService} from "../../../../testing";
import {NavDrawerHarness} from "./nav-drawer.harness";
import {TestBed} from "@angular/core/testing";
import {SettingsService} from "../../../shared/settings";

describe('NavDrawerComponent', () => {
  let drawerClosed: TestingEventEmitter;

  beforeEach(() => {
    drawerClosed = new TestingEventEmitter();
  })

  it('should notify when sidenav closes', async () => {
    const harness = await NavDrawerHarness.render({drawerClosed});

    await harness.closeDrawer();

    expect(drawerClosed.emit).toHaveBeenCalled()
  });

  it('should not notify sidenav closed when opened', async () => {
    const harness = await NavDrawerHarness.render({drawerClosed});

    await harness.openDrawer();

    expect(drawerClosed.emit).not.toHaveBeenCalled()
  })

  it('should show available routes only', async () => {
    const harness = await NavDrawerHarness.render();

    expect(harness.container).toHaveTextContent('Diagnostics');
    expect(harness.container).toHaveTextContent('Devices');
    expect(harness.container).toHaveTextContent('Rooms');
  })

  it('should show version number', async () => {
    const harness = await NavDrawerHarness.render();
    const settingsService = TestBed.inject(SettingsService) as TestingSettingsService;


    settingsService.updateSettings({...settingsService.settings, version: '9.9.9'});
    await harness.whenRenderingDone();

    expect(harness.container).toHaveTextContent('9.9.9');
  })
})
