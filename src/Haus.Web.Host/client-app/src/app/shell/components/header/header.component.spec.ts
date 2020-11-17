import {HeaderComponent} from "./header.component";
import {TestingEventEmitter, appComponentFactory} from "../../../../testing";
import {byTestId} from "@ngneat/spectator";

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
})
