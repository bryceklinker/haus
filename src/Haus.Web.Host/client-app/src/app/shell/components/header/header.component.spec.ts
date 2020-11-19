import {HeaderComponent} from "./header.component";
import {TestingEventEmitter, appComponentFactory} from "../../../../testing";
import {byTestId} from "@ngneat/spectator";
import {ThemeService} from "../../../shared/theming/theme.service";
import {MatSlideToggle} from "@angular/material/slide-toggle";

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

  it('should toggle theme when theme toggle clicked', () => {
    const spectator = createComponent();
    const themeService = spectator.inject(ThemeService, true);
    spyOn(themeService, 'toggleTheme').and.callThrough();

    spectator.triggerEventHandler(MatSlideToggle, 'toggleChange', <any>null);

    expect(themeService.toggleTheme).toHaveBeenCalled();
  })
})
