import {RenderComponentResult} from "./render-component";
import {BaseHarnessFilters, ComponentHarness, HarnessPredicate} from "@angular/cdk/testing";
import {Type} from "@angular/core";
import {MatButtonHarness} from "@angular/material/button/testing";
import {MatInputHarness} from "@angular/material/input/testing";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {MatSliderHarness} from "@angular/material/slider/testing";

export abstract class HausComponentHarness<TComponent> {
  protected get matHarness() {
    return this.result.matHarness;
  }

  protected get store() {
    return this.result.store;
  }

  protected get fixture() {
    return this.result.fixture;
  }

  protected get actionsSubject() {
    return this.result.actionsSubject;
  }

  get dialog() {
    return this.result.matDialog;
  }

  get dialogRef() {
    return this.result.matDialogRef;
  }

  get router() {
    return this.result.router;
  }

  get container() {
    return this.result.container;
  }

  get dispatchedActions() {
    return this.store.dispatchedActions;
  }

  get componentInstance() {
    return this.result.fixture.componentInstance;
  }

  protected constructor(private readonly result: RenderComponentResult<TComponent>) {

  }

  getMatHarnessByTestId<T extends ComponentHarness>(locator: (options?: BaseHarnessFilters) => HarnessPredicate<T>, testId: string): Promise<T> {
    return this.matHarness.getHarness(locator({selector: `[data-testid="${testId}"]`}));
  }

  detectChanges() {
    this.result.detectChanges();
  }

  whenRenderingDone() {
    this.detectChanges();
    return this.result.fixture.whenRenderingDone();
  }

  async triggerEventHandler<TDirective>(directive: Type<TDirective>, eventName: string, eventArg?: any) {
    this.result.triggerEventHandler(directive, eventName, eventArg);
    this.detectChanges();
    await this.whenRenderingDone();
  }

  destroy() {
    this.result.fixture.destroy();
  }

  protected async getButtonByTestId(testId: string) {
    return this.getMatHarnessByTestId(MatButtonHarness.with, testId);
  }

  protected async getInputByTestId(testId: string) {
    return this.getMatHarnessByTestId(MatInputHarness.with, testId);
  }

  protected async getSlideToggleByTestId(testId: string) {
    return this.getMatHarnessByTestId(MatSlideToggleHarness.with, testId);
  }

  protected async getSliderByTestId(testId: string) {
    return this.getMatHarnessByTestId(MatSliderHarness.with, testId);
  }

  protected async clickButtonByTestId(testId: string) {
    const button = await this.getButtonByTestId(testId);
    await button.click();
  }
}
