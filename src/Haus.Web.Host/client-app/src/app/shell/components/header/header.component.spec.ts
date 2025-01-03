import {HeaderComponent} from "./header.component";
import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {HeaderHarness} from "./header.harness";

describe('HeaderComponent', () => {

  test('should notify when menu clicked', async () => {
    const menuClick = new TestingEventEmitter();

    const harness = await HeaderHarness.render({toggleMenu: menuClick});
    await harness.clickMenu();

    expect(menuClick.emit).toHaveBeenCalled();
  })

  test('should toggle theme when theme toggle clicked', async () => {
    const harness = await HeaderHarness.render();

    await harness.toggleTheme();

    expect(harness.themeService.toggleTheme).toHaveBeenCalled();
  })

  test('should show current user', async () => {
    const user = ModelFactory.createUser();

    const harness = await HeaderHarness.render({user});

    expect(harness.profileImage).toHaveAttribute('src', user.picture);
  })

  test('should hide current user', async () => {
    const harness = await HeaderHarness.render();

    expect(harness.profileImage).not.toBeInTheDocument();
    expect(harness.userMenu).not.toBeInTheDocument();
  })

  test('should notify when user logs out', async () => {
    const user = ModelFactory.createUser();
    const emitter = new TestingEventEmitter<void>();

    const harness = await HeaderHarness.render({user, logout: emitter});
    await harness.logout();

    expect(emitter.emit).toHaveBeenCalled();
  })
})
