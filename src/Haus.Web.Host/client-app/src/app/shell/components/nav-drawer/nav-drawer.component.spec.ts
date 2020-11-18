import {NavDrawerComponent} from "./nav-drawer.component";
import {MatSidenav} from "@angular/material/sidenav";
import {TestingEventEmitter, appComponentFactory} from "../../../../testing";
import {byTestId} from "@ngneat/spectator";
import {ThemeService} from "../../../shared/theming/theme.service";

describe('NavDrawerComponent', () => {
  let drawerClosed: TestingEventEmitter;
  const createComponent = appComponentFactory(NavDrawerComponent)

  beforeEach(() => {
    drawerClosed = new TestingEventEmitter();
  })

  it('should notify when sidenav closes', () => {
    const spectator = createComponent({
      props: {drawerClosed}
    });

    spectator.triggerEventHandler(MatSidenav, 'openedChange', false);

    expect(drawerClosed.emit).toHaveBeenCalled()
  });

  it('should not notify sidenav closed when opened', () => {
    const spectator = createComponent({
      props: {drawerClosed}
    });

    spectator.triggerEventHandler(MatSidenav, 'openedChange', true);

    expect(drawerClosed.emit).not.toHaveBeenCalled()
  })
})
