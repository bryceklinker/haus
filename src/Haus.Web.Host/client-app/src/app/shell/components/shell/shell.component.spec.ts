import {ShellComponent} from "./shell.component";
import {DARK_THEME_CLASS_NAME} from "../../../shared/theming/theme-palettes";
import {renderAppComponent} from "../../../../testing";
import userEvent from "@testing-library/user-event";
import {screen} from "@testing-library/dom";
import {ComponentFixture} from "@angular/core/testing";
import {SharedActions} from "../../../shared/actions";

describe('ShellComponent', () => {
  it('should open side nav when menu clicked', async () => {
    const {fixture} = await renderAppComponent(ShellComponent);

    await clickMenuButton(fixture);
    fixture.detectChanges();
    await fixture.whenRenderingDone();

    expect(screen.getByTestId('nav-drawer').getAttribute('style')).toContain('visible');
  })

  it('should close side nav when menu clicked', async () => {
    const {fixture} = await renderAppComponent(ShellComponent);

    await clickMenuButton(fixture);
    await clickMenuButton(fixture);

    expect(screen.getByTestId('nav-drawer').getAttribute('style')).not.toContain('visible');
  })

  it('should be dark theme when rendered', async () => {
    const {container} = await renderAppComponent(ShellComponent);

    expect(container.querySelector('shell-header')).toHaveClass(DARK_THEME_CLASS_NAME);
    expect(container.querySelector('shell-nav-drawer')).toHaveClass(DARK_THEME_CLASS_NAME);
  })

  it('should dispatch init action when rendered', async () => {
    const {store} = await renderAppComponent(ShellComponent);

    expect(store.dispatchedActions).toContainEqual(SharedActions.initApp());
  })

  async function clickMenuButton(fixture: ComponentFixture<ShellComponent>) {
    userEvent.click(screen.getByTestId('menu-btn'));
    await fixture.whenRenderingDone();
  }
})
