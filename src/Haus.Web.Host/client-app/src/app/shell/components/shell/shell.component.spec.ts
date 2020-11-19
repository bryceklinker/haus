import {ShellComponent} from "./shell.component";
import {NavDrawerComponent} from "../nav-drawer/nav-drawer.component";
import {DARK_THEME_CLASS_NAME} from "../../../shared/theming/theme-palettes";
import {renderAppComponent, RenderComponentResult} from "../../../../testing";
import {MatSidenav} from "@angular/material/sidenav";
import {By} from "@angular/platform-browser";

describe('ShellComponent', () => {
  it('should open side nav when menu clicked', async () => {
    const result = await renderAppComponent(ShellComponent);

    await clickMenuButton(result);

    expect(result.getByTestId('nav-drawer').getAttribute('style')).toContain('visible');
  })

  it('should close side nav when menu clicked', async () => {
    const result = await renderAppComponent(ShellComponent);

    await clickMenuButton(result);
    await clickMenuButton(result);

    expect(result.getByTestId('nav-drawer').getAttribute('style')).not.toContain('visible');
  })

  it('should be dark theme when rendered', async () => {
    const {container} = await renderAppComponent(ShellComponent);

    expect(container.querySelector('shell-header')).toHaveClass(DARK_THEME_CLASS_NAME);
    expect(container.querySelector('shell-nav-drawer')).toHaveClass(DARK_THEME_CLASS_NAME);
  })

  async function clickMenuButton({fireEvent, getByTestId, fixture}: RenderComponentResult<ShellComponent>) {
    fireEvent.click(getByTestId('menu-btn'));
    await fixture.whenRenderingDone();
  }
})
