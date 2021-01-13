import {HeaderComponent} from "./header.component";
import {TestingEventEmitter} from "../../../../testing";
import {HeaderHarness} from "./header.harness";

describe('HeaderComponent', () => {

  it('should notify when menu clicked', async () => {
    const menuClick = new TestingEventEmitter();

    const harness = await HeaderHarness.render({menuClick});
    await harness.clickMenu();

    expect(menuClick.emit).toHaveBeenCalled();
  })

  it('should toggle theme when theme toggle clicked', async () => {
    const harness = await HeaderHarness.render();

    await harness.toggleTheme();

    expect(harness.themeService.toggleTheme).toHaveBeenCalled();
  })
})
