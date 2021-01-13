import {HausComponentHarness, renderAppComponent} from "../../../../testing";
import {NavDrawerComponent} from "./nav-drawer.component";
import {MatSidenav} from "@angular/material/sidenav";

export class NavDrawerHarness extends HausComponentHarness<NavDrawerComponent> {
  async closeDrawer() {
    await this.triggerEventHandler(MatSidenav, 'openedChange', false);
  }

  async openDrawer() {
    await this.triggerEventHandler(MatSidenav, 'openedChange', true);
  }

  static async render(props: Partial<NavDrawerComponent> = {}) {
    const result = await renderAppComponent(NavDrawerComponent, {
      componentProperties: props
    });

    return new NavDrawerHarness(result);
  }
}
