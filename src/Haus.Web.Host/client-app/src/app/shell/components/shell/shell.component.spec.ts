import {ShellComponent} from "./shell.component";
import {DARK_THEME_CLASS_NAME} from "../../../shared/theming/theme-palettes";
import {SharedActions} from "../../../shared/actions";
import {ShellHarness} from "./shell.harness";

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
})
