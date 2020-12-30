import {Component, OnDestroy, OnInit} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {DeviceTypesActions, selectAllDeviceTypes} from "../../../devices/state";
import {Observable} from "rxjs";
import {toTitleCase} from "../../../shared/humanize";
import {DeviceSimulatorActions} from "../../state";
import {FormArray, FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {Actions, ofType} from "@ngrx/effects";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {tap} from "rxjs/operators";
import {Router} from "@angular/router";

@Component({
  selector: 'add-simulated-device',
  templateUrl: './add-simulated-device.component.html',
  styleUrls: ['./add-simulated-device.component.scss']
})
export class AddSimulatedDeviceComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  deviceTypes$: Observable<Array<string>>;

  form = new FormBuilder().group({
    deviceType: ['', Validators.required],
    metadata: new FormBuilder().array([])
  });

  get metadata(): FormArray {
    return this.form.get('metadata') as FormArray;
  }

  get metadataGroups(): Array<FormGroup> {
    return this.metadata.controls.map(c => c as FormGroup);
  }

  constructor(private readonly store: Store<AppState>,
              private readonly actions$: Actions,
              private readonly router: Router) {
    this.deviceTypes$ = store.select(selectAllDeviceTypes);
  }

  ngOnInit(): void {
    this.store.dispatch(DeviceTypesActions.loadDeviceTypes.request());
    this.destroyable.register(this.actions$.pipe(
      ofType(DeviceSimulatorActions.addSimulatedDevice.success),
      tap(() => this.navigateToDashboard())
    )).subscribe();
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }

  humanize(deviceType: string): string {
    return toTitleCase(deviceType);
  }

  onAddMetadata(): void {
    this.metadata.push(new FormBuilder().group({
      key: ['', Validators.required],
      value: ['', Validators.required]
    }))
  }

  onSave() {
    const model = this.form.getRawValue();
    this.store.dispatch(DeviceSimulatorActions.addSimulatedDevice.request(model));
  }

  getFormControl(group: FormGroup, key: string): FormControl {
    return group.get(key) as FormControl;
  }

  async onCancel() {
    await this.navigateToDashboard();
  }

  private navigateToDashboard() {
    return this.router.navigateByUrl('/device-simulator');
  }
}
