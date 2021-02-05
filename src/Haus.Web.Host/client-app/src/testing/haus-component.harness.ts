import {RenderComponentResult} from "./render-component";
import {BaseHarnessFilters, ComponentHarness, HarnessPredicate} from "@angular/cdk/testing";
import {Type} from "@angular/core";
import {MatButtonHarness} from "@angular/material/button/testing";
import {MatInputHarness} from "@angular/material/input/testing";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {MatSelectHarness} from "@angular/material/select/testing";
import {By} from "@angular/platform-browser";
import {MatListItemHarness} from "@angular/material/list/testing";

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

  get actionsSubject() {
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
    return await this.getMatHarnessByTestId(MatButtonHarness.with, testId);
  }

  protected async isButtonDisabledByTestId(testId: string) {
    const button = await this.getButtonByTestId(testId);
    return await button.isDisabled();
  }

  protected async clickButtonByTestId(testId: string) {
    const button = await this.getButtonByTestId(testId);
    await button.click();
  }

  protected async getInputByTestId(testId: string) {
    return await this.getMatHarnessByTestId(MatInputHarness.with, testId);
  }

  protected async getInputValueByTestId(testId: string) {
    const input = await this.getInputByTestId(testId);
    return await input.getValue();
  }

  protected async changeInputByTestId(value: string, testId: string) {
    const input = await this.getInputByTestId(testId);
    await input.setValue(value);
  }

  protected async isInputDisabledByTestId(testId: string) {
    const input = await this.getInputByTestId(testId);
    return await input.isDisabled();
  }

  protected async getSelectByTestId(testId: string) {
    return await this.getMatHarnessByTestId(MatSelectHarness.with, testId);
  }

  protected async getSliderByTestId(testId: string) {
    return this.getMatHarnessByTestId(MatSliderHarness.with, testId);
  }

  protected async changeSelectedOptionByTestId(text: string, testId: string) {
    const select = await this.getSelectByTestId(testId);
    await select.open();
    await select.clickOptions({text});
  }

  protected async getSelectOptionsByTestId(testId: string) {
    const select = await this.getSelectByTestId(testId);
    await select.open();
    return await select.getOptions();
  }

  protected async getSlideToggleByTestId(testId: string) {
    return await this.getMatHarnessByTestId(MatSlideToggleHarness.with, testId);
  }

  protected async checkSlideToggleByTestId(testId: string) {
    const toggle = await this.getSlideToggleByTestId(testId);
    await toggle.check();
  }

  protected async isSlideToggleCheckedByTestId(testId: string) {
    const toggle = await this.getSlideToggleByTestId(testId);
    return await toggle.isChecked();
  }

  protected async isSlideToggleDisabledByTestId(testId: string) {
    const toggle = await this.getSlideToggleByTestId(testId);
    return await toggle.isDisabled();
  }

  protected async isSliderDisabledByTestId(testId: string) {
    const slider = await this.getSliderByTestId(testId);
    return await slider.isDisabled();
  }

  protected async getSliderValueByTestId(testId: string) {
    const slider = await this.getSliderByTestId(testId);
    return await slider.getValue();
  }

  protected async getSliderMinByTestId(testId: string) {
    const slider = await this.getSliderByTestId(testId);
    return await slider.getMinValue();
  }

  protected async getSliderMaxByTestId(testId: string) {
    const slider = await this.getSliderByTestId(testId);
    return await slider.getMaxValue();
  }

  protected async changeSliderValueByTestId(value: number, testId: string) {
    this.fixture.debugElement.query(By.css(`[data-testid="${testId}"]`))
      .triggerEventHandler('input', {value});

    await this.whenRenderingDone();
  }
}
