import {HeaderComponent} from "./header.component";
import {TestingEventEmitter, appComponentFactory} from "../../../../testing";
import {byTestId} from "@ngneat/spectator";
import {ThemeService} from "../../../shared/theming/theme.service";

describe('HeaderComponent', () => {
  const createComponent = appComponentFactory(HeaderComponent)

  it('should notify when menu clicked', () => {
    const menuClick = new TestingEventEmitter();

    const spectator = createComponent({
      props: {menuClick}
    });

    spectator.click(byTestId('menu-btn'));

    expect(menuClick.emit).toHaveBeenCalled();
  })

  it('should toggle theme when theme toggle clicked', done => {
    const spectator = createComponent();
    const themeService = spectator.inject(ThemeService);
    spyOn(themeService, 'toggleTheme').and.callThrough();

    spectator.click(byTestId('theme-toggle'));

    expect(themeService.toggleTheme).toHaveBeenCalled();
  })
})
