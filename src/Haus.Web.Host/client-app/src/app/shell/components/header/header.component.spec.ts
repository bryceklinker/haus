import {renderComponent} from "../../../../testing";
import {HeaderComponent} from "./header.component";
import {TestingEventEmitter} from "../../../../testing/testing-event-emitter";

describe('HeaderComponent', () => {
  it('should notify when menu clicked', async () => {
    const menuClick = new TestingEventEmitter();

    const {getByTestId, fireEvent} = await renderComponent(HeaderComponent, {
      componentProperties: {menuClick}
    });

    await fireEvent.click(getByTestId('menu-btn'));
    expect(menuClick.emit).toHaveBeenCalled();
  })
})
