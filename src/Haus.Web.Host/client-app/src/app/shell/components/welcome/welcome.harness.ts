import {HausComponentHarness, renderAppComponent, RenderComponentResult} from '../../../../testing';
import {WelcomeComponent} from './welcome.component';
import {screen} from '@testing-library/angular';
import { Action } from '@ngrx/store';

export class WelcomeHarness extends HausComponentHarness<WelcomeComponent> {
  get header(): HTMLElement {
    return screen.getByLabelText('welcome header');
  }

  constructor(result: RenderComponentResult<WelcomeComponent>) {
    super(result);
  }

  static async render(...actions: Action[]) {
    const result = await renderAppComponent(WelcomeComponent, {actions});
    return new WelcomeHarness(result);
  }
}
