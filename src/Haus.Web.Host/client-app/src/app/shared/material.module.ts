import {MatButtonModule} from "@angular/material/button";
import {MatSidenavModule} from "@angular/material/sidenav";
import {MatSnackBarModule} from "@angular/material/snack-bar";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatListModule} from "@angular/material/list";
import {MatInputModule} from "@angular/material/input";
import {MatRadioModule} from "@angular/material/radio";
import {MatSelectModule} from "@angular/material/select";
import {MatTableModule} from "@angular/material/table";
import {MatMenuModule} from "@angular/material/menu";
import {MatIconModule} from "@angular/material/icon";
import {MatCardModule} from "@angular/material/card";
import {MatDialogModule} from "@angular/material/dialog";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatDividerModule} from "@angular/material/divider";
import {MatCheckboxModule} from "@angular/material/checkbox";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatSlideToggleModule} from "@angular/material/slide-toggle";
import {MatExpansionModule} from "@angular/material/expansion";
import {MatGridListModule} from "@angular/material/grid-list";
import {MatSliderModule} from '@angular/material/slider';
import {MatRippleModule} from "@angular/material/core";
import {NgModule} from "@angular/core";
import {DragDropModule} from "@angular/cdk/drag-drop";
import {ScrollingModule} from "@angular/cdk/scrolling";

const MATERIAL_MODULES = [
  MatButtonModule,
  MatSidenavModule,
  MatSnackBarModule,
  MatProgressSpinnerModule,
  MatToolbarModule,
  MatListModule,
  MatInputModule,
  MatRadioModule,
  MatSelectModule,
  MatTableModule,
  MatMenuModule,
  MatIconModule,
  MatCardModule,
  MatDialogModule,
  MatFormFieldModule,
  MatDividerModule,
  MatCheckboxModule,
  MatDatepickerModule,
  MatSlideToggleModule,
  MatExpansionModule,
  MatGridListModule,
  MatRippleModule,
  MatSliderModule,
  DragDropModule,
  ScrollingModule
]

@NgModule({
  imports: [
    ...MATERIAL_MODULES,
  ],
  exports: [
    ...MATERIAL_MODULES
  ]
})
export class MaterialModule{}
