import {ShellComponent} from "./shell.component";
import {NavDrawerComponent} from "../nav-drawer/nav-drawer.component";
import {byTestId, Spectator} from "@ngneat/spectator";
import {appComponentFactory} from "../../../../testing";

describe('ShellComponent', () => {
  const createComponent = appComponentFactory(ShellComponent);

  it('should open side nav when menu clicked', async () => {
    const spectator = createComponent();

    await clickMenuButton(spectator);

    expect(spectator.query(byTestId('nav-drawer'))).toBeVisible()
  })

  it('should close side nav when menu clicked', async () => {
    const spectator = createComponent();

    await clickMenuButton(spectator);
    await clickMenuButton(spectator);

    expect(spectator.query(byTestId('nav-drawer'))).not.toBeVisible();
  })

  it('should close side nav when side nav closed', async () => {
    const spectator = createComponent();

    await clickMenuButton(spectator);
    spectator.triggerEventHandler(NavDrawerComponent, 'drawerClosed', null);
    await spectator.fixture.whenRenderingDone();

    expect(spectator.query(byTestId('nav-drawer'))).not.toBeVisible();
  })

  async function clickMenuButton(spectator: Spectator<ShellComponent>) {
    spectator.click(byTestId('menu-btn'));
    await spectator.fixture.whenRenderingDone();
  }
})
