import {TestBed} from "@angular/core/testing";

import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {HeaderComponent} from "./header.component";
import {ThemeService} from "../../../shared/theming/theme.service";

export class HeaderHarness extends HausComponentHarness<HeaderComponent> {
  themeService: ThemeService;
  async clickMenu() {
    await this.clickButtonByTestId('menu-btn');
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

  static async render(props: Partial<HeaderComponent> = {}) {
    const result = await renderAppComponent(HeaderComponent, {
      componentProperties: props
    })

    return new HeaderHarness(result);
  }
}
