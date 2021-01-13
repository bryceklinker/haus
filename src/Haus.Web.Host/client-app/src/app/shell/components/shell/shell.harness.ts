import {HausComponentHarness, renderAppComponent} from "../../../../testing";
import {ShellComponent} from "./shell.component";
import {screen} from "@testing-library/dom";

export class ShellHarness extends HausComponentHarness<ShellComponent> {
  get headerElement() {
    return this.container.querySelector('shell-header');
  }

  get navDrawerElement() {
    return this.container.querySelector('shell-nav-drawer');
  }

  get navDrawerStyle() {
    return screen.getByTestId('nav-drawer').getAttribute('style');
  }

  async clickMenu() {
    await this.clickButtonByTestId('menu-btn');
  }

  static async render() {
    const result = await renderAppComponent(ShellComponent);

    return new ShellHarness(result);
  }
}
