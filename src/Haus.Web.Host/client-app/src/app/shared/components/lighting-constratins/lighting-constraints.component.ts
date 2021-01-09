import {Component, Input, OnInit, EventEmitter, Output} from "@angular/core";
import {LightingConstraintsModel} from "../../models";
import {AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators} from "@angular/forms";

const DEFAULT_LIGHTING_CONSTRAINTS: LightingConstraintsModel = {minLevel: 0, maxLevel: 100, minTemperature: 2000, maxTemperature: 6000};
@Component({
  selector: 'lighting-constraints',
  templateUrl: './lighting-constraints.component.html',
  styleUrls: ['./lighting-constraints.component.scss']
})
export class LightingConstraintsComponent implements OnInit {
  @Input() constraints: LightingConstraintsModel | null = null;
  @Input() readonly: boolean = false;
  @Input() hideCancel: boolean = false;
  @Output() save = new EventEmitter<LightingConstraintsModel>();
  @Output() cancel = new EventEmitter<void>();

  constraintsForm = new FormGroup({});

  get showCancel(): boolean {
    return !this.hideCancel;
  }

  get minLevel(): number {
    return this.constraints ? this.constraints.minLevel : DEFAULT_LIGHTING_CONSTRAINTS.minLevel;
  }

  get maxLevel(): number {
    return this.constraints ? this.constraints.maxLevel : DEFAULT_LIGHTING_CONSTRAINTS.maxLevel;
  }

  get minTemperature(): number {
    return this.constraints ? this.constraints.minTemperature : DEFAULT_LIGHTING_CONSTRAINTS.minTemperature;
  }

  get maxTemperature(): number {
    return this.constraints ? this.constraints.maxTemperature : DEFAULT_LIGHTING_CONSTRAINTS.maxTemperature;
  }

  constructor(private readonly formBuilder: FormBuilder) {
  }

  ngOnInit(): void {
    this.constraintsForm = this.formBuilder.group({
      minLevel: [{value: this.minLevel, disabled: this.readonly}, [Validators.required, this.validateMinLevel()]],
      maxLevel: [{value: this.maxLevel, disabled: this.readonly}, [Validators.required, this.validateMaxLevel()]],
      minTemperature: [{value: this.minTemperature, disabled: this.readonly}, [Validators.required, this.validateMinTemperature()]],
      maxTemperature: [{value: this.maxTemperature, disabled: this.readonly}, [Validators.required, this.validateMaxTemperature()]],
    })
  }

  onSave() {
    this.save.emit(this.constraintsForm.getRawValue());
  }

  onCancel() {
    this.cancel.emit();
  }

  validateMinLevel(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return this.validateLessThanValue(control, this.maxLevel)
        ? {minLevel: {value: control.value}}
        : null;
    }
  }

  validateMaxLevel(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return this.validateGreaterThanValue(control, this.minLevel)
        ? {maxLevel: {value: control.value}}
        : null;
    }
  }

  validateMinTemperature(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return this.validateLessThanValue(control, this.maxTemperature)
        ? {minTemperature: {value: control.value}}
        : null;
    }
  }

  validateMaxTemperature(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return this.validateGreaterThanValue(control, this.minTemperature)
        ? {maxTemperature: {value: control.value}}
        : null;
    }
  }

  validateLessThanValue(control: AbstractControl, limit: number): boolean {
    return Number(control.value) <= limit;
  }

  validateGreaterThanValue(control: AbstractControl, limit: number): boolean {
    return Number(control.value) >= limit;
  }
}
