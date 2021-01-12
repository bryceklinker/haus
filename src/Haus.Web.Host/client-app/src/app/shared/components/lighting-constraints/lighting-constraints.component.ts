import {Component, Input, OnInit, EventEmitter, Output} from "@angular/core";
import {AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators} from "@angular/forms";
import {LevelLightingModel, LightingConstraintsModel, TemperatureLightingModel} from "../../models";
import {max} from "rxjs/operators";

@Component({
  selector: 'lighting-constraints',
  templateUrl: './lighting-constraints.component.html',
  styleUrls: ['./lighting-constraints.component.scss']
})
export class LightingConstraintsComponent implements OnInit {
  @Input() level: LevelLightingModel | undefined | null = null;
  @Input() temperature: TemperatureLightingModel | undefined | null = null;
  @Input() readonly: boolean = false;
  @Input() hideCancel: boolean = false;
  @Output() save = new EventEmitter<LightingConstraintsModel>();
  @Output() cancel = new EventEmitter<void>();

  constraintsForm = new FormGroup({});

  get showCancel(): boolean {
    return !this.hideCancel;
  }

  get minLevel(): number {
    return this.level ? this.level.min : 0;
  }

  get maxLevel(): number {
    return this.level ? this.level.max : 0;
  }

  get minTemperature(): number | null {
    return this.temperature ? this.temperature.min : null;
  }

  get maxTemperature(): number | null {
    return this.temperature ? this.temperature.max : null;
  }

  constructor(private readonly formBuilder: FormBuilder) {
  }

  ngOnInit(): void {
    this.constraintsForm = this.formBuilder.group({
      minLevel: [{value: this.minLevel, disabled: this.readonly}, [Validators.required]],
      maxLevel: [{value: this.maxLevel, disabled: this.readonly}, [Validators.required]],

      minTemperature: this.temperature ? [{value: this.minTemperature, disabled: this.readonly}, [Validators.required]] : undefined,
      maxTemperature: this.temperature ? [{value: this.maxTemperature, disabled: this.readonly}, [Validators.required]] : undefined,
    }, {
      validators: [this.validateForm()]
    })
  }

  onSave() {
    const constraints: LightingConstraintsModel = {
      minLevel: Number(this.constraintsForm.get('minLevel')?.value),
      maxLevel: Number(this.constraintsForm.get('maxLevel')?.value),
      minTemperature: this.temperature ? Number(this.constraintsForm.get('minTemperature')?.value) : undefined,
      maxTemperature: this.temperature ? Number(this.constraintsForm.get('maxTemperature')?.value) : undefined,
    }
    this.save.emit(constraints);
  }

  onCancel() {
    this.cancel.emit();
  }

  validateForm(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const levelResult = this.validateLevelRange(control);
      const temperatureResult = this.validateTemperatureRange(control);
      if (!levelResult && !temperatureResult) {
        return null;
      }

      return {
        ...(levelResult || {}),
        ...(temperatureResult || {})
      }
    }
  }

  validateLevelRange(control: AbstractControl): ValidationErrors | null {
    const minLevel = Number(control.get('minLevel')?.value);
    const maxLevel = Number(control.get('maxLevel')?.value);
    if (minLevel < maxLevel) {
      return null;
    }

    return {
      minLevel: {value: minLevel},
      maxLevel: {value: maxLevel}
    }
  }

  validateTemperatureRange(control: AbstractControl): ValidationErrors | null {
    if (!this.temperature) {
      return null;
    }

    const minTemperature = Number(control.get('minTemperature')?.value);
    const maxTemperature = Number(control.get('maxTemperature')?.value);
    if (minTemperature < maxTemperature) {
      return null;
    }

    return {
      minTemperature: {value: minTemperature},
      maxTemperature: {value: maxTemperature}
    }
  }
}
