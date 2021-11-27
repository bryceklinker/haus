import {RenderComponentResult} from "./render-component";
import {BaseHarnessFilters, ComponentHarness, HarnessPredicate} from "@angular/cdk/testing";
import {Type} from "@angular/core";
import {MatInputHarness} from "@angular/material/input/testing";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {MatSelectHarness} from "@angular/material/select/testing";
import {MatExpansionPanelHarness} from '@angular/material/expansion/testing';
import {By} from "@angular/platform-browser";
import userEvent from '@testing-library/user-event';
import {screen} from '@testing-library/angular';

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

  getMatHarnessByLabel<T extends ComponentHarness>(locator: (options?: BaseHarnessFilters) => HarnessPredicate<T>, label: string): Promise<T> {
    return this.matHarness.getHarness(locator({selector: `[aria-label="${label}"]`}));
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

  protected getButtonByLabel(label: string) {
    return screen.getByRole('button', {name: label});
  }

  protected isButtonDisabledByLabel(label: string) {
    return this.getButtonByLabel(label)
      .hasAttribute('disabled');
  }

  protected clickButtonByLabel(label: string) {
    userEvent.click(this.getButtonByLabel(label));
  }

  protected async getInputByLabel(label: string) {
    return await this.getMatHarnessByLabel(MatInputHarness.with, label);
  }

  protected async getInputValueByLabel(label: string) {
    const input = await this.getInputByLabel(label);
    return await input.getValue();
  }

  protected async changeInputByLabel(value: string, label: string) {
    const input = await this.getInputByLabel(label);
    await input.setValue(value);
  }

  protected async isInputDisabledByLabel(label: string) {
    const input = await this.getInputByLabel(label);
    return await input.isDisabled();
  }

  protected async getSelectByLabel(label: string) {
    return await this.getMatHarnessByLabel(MatSelectHarness.with, label);
  }

  protected async getSliderByLabel(label: string) {
    return this.getMatHarnessByLabel(MatSliderHarness.with, label);
  }

  protected async changeSelectedOptionByLabel(text: string, label: string) {
    const select = await this.getSelectByLabel(label);
    await select.open();
    await select.clickOptions({text});
  }

  protected async getSelectOptionsByLabel(label: string) {
    const select = await this.getSelectByLabel(label);
    await select.open();
    return await select.getOptions();
  }

  protected getSlideToggleByLabel(label: string) {
    return screen.getByRole('switch', {name: label});
  }

  protected toggleSlideByLabel(label: string) {
    userEvent.click(this.getSlideToggleByLabel(label));
  }

  protected isSlideToggleCheckedByLabel(label: string) {
    return Boolean(this.getSlideToggleByLabel(label).getAttribute('aria-checked'));
  }

  protected isSlideToggleDisabledByLabel(label: string) {
    return this.getSlideToggleByLabel(label).hasAttribute('disabled');
  }

  protected async isSliderDisabledByLabel(label: string) {
    const slider = await this.getSliderByLabel(label);
    return await slider.isDisabled();
  }

  protected async getSliderValueByLabel(label: string) {
    const slider = await this.getSliderByLabel(label);
    return await slider.getValue();
  }

  protected async getSliderMinByLabel(label: string) {
    const slider = await this.getSliderByLabel(label);
    return await slider.getMinValue();
  }

  protected async getSliderMaxByLabel(label: string) {
    const slider = await this.getSliderByLabel(label);
    return await slider.getMaxValue();
  }

  protected async changeSliderValueByLabel(value: number, label: string) {
    this.fixture.debugElement.query(By.css(`[aria-label="${label}"]`))
      .triggerEventHandler('input', {value});

    await this.whenRenderingDone();
  }

  protected async getExpansionPanelByLabel(label: string) {
    return await this.getMatHarnessByLabel(MatExpansionPanelHarness.with, label);
  }

  protected async expandExpansionPanelByLabel(label: string) {
    const harness = await this.getExpansionPanelByLabel(label);
    await harness.expand();
  }
}
