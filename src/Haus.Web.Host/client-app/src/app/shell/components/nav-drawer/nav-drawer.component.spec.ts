import {NavDrawerComponent} from "./nav-drawer.component";
import {TestingEventEmitter} from "../../../../testing";
import {NavDrawerHarness} from "./nav-drawer.harness";

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
})
