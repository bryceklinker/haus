import {ShellComponent} from "./shell.component";
import {DARK_THEME_CLASS_NAME} from "../../../shared/theming/theme-palettes";
import {SharedActions} from "../../../shared/actions";
import {ShellHarness} from "./shell.harness";
import {AuthActions} from "../../../shared/auth";
import {ModelFactory} from "../../../../testing";

describe('ShellComponent', () => {
  it('should open side nav when menu clicked', async () => {
    const harness = await ShellHarness.render();

    await harness.clickMenu();

    expect(harness.navDrawerStyle).toContain('visible');
  })

  it('should close side nav when menu clicked', async () => {
    const harness = await ShellHarness.render();

    await harness.clickMenu();
    await harness.clickMenu();

    expect(harness.navDrawerStyle).not.toContain('visible');
  })

  it('should be dark theme when rendered', async () => {
    const harness = await ShellHarness.render();

    expect(harness.headerElement).toHaveClass(DARK_THEME_CLASS_NAME);
    expect(harness.navDrawerElement).toHaveClass(DARK_THEME_CLASS_NAME);
  })

  it('should dispatch init action when rendered', async () => {
    const harness = await ShellHarness.render();

    expect(harness.dispatchedActions).toContainEqual(SharedActions.initApp());
  })

  it('should show user when logged in', async () => {
    const harness = await ShellHarness.render(AuthActions.userLoggedIn(ModelFactory.createUser()));

    expect(harness.userMenu).toBeInTheDocument();
  })

  it('should dispatch logout when user logs out', async () => {
    const harness = await ShellHarness.render(AuthActions.userLoggedIn(ModelFactory.createUser()));

    await harness.logout();

    expect(harness.dispatchedActions).toContainEqual(AuthActions.logout());
  })

  it('should be loading until user is logged in', async () => {
    const harness = await ShellHarness.render();

    expect(harness.isLoading).toEqual(true);
  })
})
