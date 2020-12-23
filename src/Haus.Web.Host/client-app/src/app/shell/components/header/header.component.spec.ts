import {TestBed} from "@angular/core/testing";
import {MatSlideToggle} from "@angular/material/slide-toggle";
import userEvent from "@testing-library/user-event";
import {screen} from '@testing-library/dom';

import {HeaderComponent} from "./header.component";
import {TestingEventEmitter, renderAppComponent} from "../../../../testing";
import {ThemeService} from "../../../shared/theming/theme.service";

describe('HeaderComponent', () => {

  it('should notify when menu clicked', async () => {
    const menuClick = new TestingEventEmitter();

    await renderAppComponent(HeaderComponent, {
      componentProperties:{menuClick}
    });

    userEvent.click(screen.getByTestId('menu-btn'));

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
