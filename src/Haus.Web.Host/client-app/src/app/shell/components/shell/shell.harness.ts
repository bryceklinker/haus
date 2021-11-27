import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {ShellComponent} from "./shell.component";
import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";
import {HeaderHarness} from "../header/header.harness";

export class ShellHarness extends HausComponentHarness<ShellComponent> {
  private readonly _headerHarness: HeaderHarness;

  get headerElement() {
    return this.container.querySelector('shell-header');
  }

  get navDrawerElement() {
    return this.container.querySelector('shell-nav-drawer');
  }

  get navDrawerStyle() {
    return screen.getByLabelText('nav drawer').getAttribute('style');
  }

  get userMenu() {
    return this._headerHarness.userMenu;
  }

  get isLoading() {
    return screen.queryAllByLabelText('loading indicator').length > 0;
  }

  async clickMenu() {
    await this.clickButtonByLabel('menu');
  }

  async logout() {
    return this._headerHarness.logout();
  }

  constructor(result: RenderComponentResult<ShellComponent>) {
    super(result);

    this._headerHarness = HeaderHarness.fromResult(result);
  }

  static async render(...actions: Action[]) {
    const result = await renderAppComponent(ShellComponent, {
      actions
    });

    return new ShellHarness(result);
  }
}
