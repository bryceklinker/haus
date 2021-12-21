import {NavDrawerComponent} from "./nav-drawer.component";
import {ModelFactory, TestingEventEmitter, TestingSettingsService} from "../../../../testing";
import {NavDrawerHarness} from "./nav-drawer.harness";
import {TestBed} from "@angular/core/testing";
import {SettingsService} from "../../../shared/settings";
import {ShellActions} from "../../state";

describe('NavDrawerComponent', () => {
  let drawerClosed: TestingEventEmitter;

  beforeEach(() => {
    drawerClosed = new TestingEventEmitter();
  })

  test('should notify when sidenav closes', async () => {
    const harness = await NavDrawerHarness.render({drawerClosed});

    await harness.closeDrawer();

    expect(drawerClosed.emit).toHaveBeenCalled()
  });

  test('should not notify sidenav closed when opened', async () => {
    const harness = await NavDrawerHarness.render({drawerClosed});

    await harness.openDrawer();

    expect(drawerClosed.emit).not.toHaveBeenCalled()
  })

  test('should show available routes only', async () => {
    const harness = await NavDrawerHarness.render();

    expect(harness.container).toHaveTextContent('Diagnostics');
    expect(harness.container).toHaveTextContent('Devices');
    expect(harness.container).toHaveTextContent('Rooms');
  })

  test('should show version number', async () => {
    const harness = await NavDrawerHarness.render();
    const settingsService = TestBed.inject(SettingsService) as TestingSettingsService;

    settingsService.updateSettings({...settingsService.settings, version: '9.9.9'});
    await harness.whenRenderingDone();

    expect(harness.container).toHaveTextContent('9.9.9');
  })

  test('should request to load latest version', async () => {
    const harness = await NavDrawerHarness.render();

    expect(harness.dispatchedActions).toContainEqual(ShellActions.loadLatestVersion.request());
  })

  test('should show that an update is available', async () => {
    const latestVersion = ModelFactory.createApplicationVersion({isNewer: true});
    const harness = await NavDrawerHarness.render({}, ShellActions.loadLatestVersion.success(latestVersion));

    expect(harness.isShowingUpdateAvailable).toEqual(true);
  })

  test('should show update error when load latest version failed', async () => {
    const harness = await NavDrawerHarness.render({}, ShellActions.loadLatestVersion.failed(new Error('idk')));

    expect(harness.isShowingUpdateError).toEqual(true);
  })
})
