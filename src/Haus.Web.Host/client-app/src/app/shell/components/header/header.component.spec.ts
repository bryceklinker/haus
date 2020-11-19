import {HeaderComponent} from "./header.component";
import {TestingEventEmitter, renderAppComponent} from "../../../../testing";
import {ThemeService} from "../../../shared/theming/theme.service";
import {MatSlideToggle} from "@angular/material/slide-toggle";
import {TestBed} from "@angular/core/testing";

describe('HeaderComponent', () => {

  it('should notify when menu clicked', async () => {
    const menuClick = new TestingEventEmitter();

    const {fireEvent, getByTestId} = await renderAppComponent(HeaderComponent, {
      componentProperties:{menuClick}
    });

    fireEvent.click(getByTestId('menu-btn'));

    expect(menuClick.emit).toHaveBeenCalled();
  })

  it('should toggle theme when theme toggle clicked', async () => {
    const {triggerEventHandler} = await renderAppComponent(HeaderComponent);
    const themeService = TestBed.inject(ThemeService);
    spyOn(themeService, 'toggleTheme').and.callThrough();

    triggerEventHandler(MatSlideToggle, 'toggleChange', null);

    expect(themeService.toggleTheme).toHaveBeenCalled();
  })
})
