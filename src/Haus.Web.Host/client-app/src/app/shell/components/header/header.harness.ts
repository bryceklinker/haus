import {TestBed} from "@angular/core/testing";
import {screen} from "@testing-library/dom";

import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {HeaderComponent} from "./header.component";
import {ThemeService} from "../../../shared/theming/theme.service";
import {MatMenuHarness} from "@angular/material/menu/testing";

export class HeaderHarness extends HausComponentHarness<HeaderComponent> {
  themeService: ThemeService;

  get profileImage() {
    return screen.queryByLabelText('profile img');
  }

  get userMenu() {
    return screen.queryByRole('button', {name: 'user menu'});
  }

  async clickMenu() {
    await this.clickButtonByLabel('menu');
  }

  async logout() {
    const menu = await this.getMatHarnessByLabel(MatMenuHarness.with, 'user menu');
    await menu.open();
    await menu.clickItem({selector: '[aria-label="logout"]'});
  }

  async toggleTheme() {
    await this.toggleSlideByLabel('theme toggle');
  }

  private constructor(result: RenderComponentResult<HeaderComponent>) {
    super(result);

    this.themeService = TestBed.inject(ThemeService);
    jest.spyOn(this.themeService, 'toggleTheme');
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
