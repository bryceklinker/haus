import {TestBed} from "@angular/core/testing";
import {screen} from "@testing-library/dom";

import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {HeaderComponent} from "./header.component";
import {ThemeService} from "../../../shared/theming/theme.service";
import {MatMenuHarness} from "@angular/material/menu/testing";

export class HeaderHarness extends HausComponentHarness<HeaderComponent> {
  themeService: ThemeService;

  get profileImage() {
    return screen.queryByTestId('profile-img');
  }

  get userMenu() {
    return screen.queryByTestId('user-menu');
  }

  async clickMenu() {
    await this.clickButtonByTestId('menu-btn');
  }

  async logout() {
    const menu = await this.getMatHarnessByTestId(MatMenuHarness.with, 'user-menu-btn');
    await menu.open();
    await menu.clickItem({selector: '[data-testid="logout-btn"]'});
  }

  async toggleTheme() {
    const toggle = await this.getSlideToggleByTestId('theme-toggle');
    await toggle.toggle();
  }

  private constructor(result: RenderComponentResult<HeaderComponent>) {
    super(result);

    this.themeService = TestBed.inject(ThemeService);
    spyOn(this.themeService, 'toggleTheme').and.callThrough();
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new HeaderHarness(result);
  }

  static async render(props: Partial<HeaderComponent> = {}) {
    const result = await renderAppComponent(HeaderComponent, {
      componentProperties: props
    })

    return new HeaderHarness(result);
  }
}
