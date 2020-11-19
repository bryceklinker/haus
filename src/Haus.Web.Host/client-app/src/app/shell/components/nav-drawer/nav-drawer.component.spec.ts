import {NavDrawerComponent} from "./nav-drawer.component";
import {MatSidenav} from "@angular/material/sidenav";
import {renderAppComponent, TestingEventEmitter} from "../../../../testing";

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
})
