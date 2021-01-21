import {HausComponentHarness, renderAppComponent} from "../../../../testing";
import {NavDrawerComponent} from "./nav-drawer.component";
import {MatSidenav} from "@angular/material/sidenav";
import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";

export class NavDrawerHarness extends HausComponentHarness<NavDrawerComponent> {
  get isUpdateAvailable() {
    return screen.queryAllByTestId('update-available').length > 0;
  }

  async closeDrawer() {
    await this.triggerEventHandler(MatSidenav, 'openedChange', false);
  }

  async openDrawer() {
    await this.triggerEventHandler(MatSidenav, 'openedChange', true);
  }

  static async render(props: Partial<NavDrawerComponent> = {}, ...actions: Action[]) {
    const result = await renderAppComponent(NavDrawerComponent, {
      componentProperties: props,
      actions
    });

    return new NavDrawerHarness(result);
  }
}
