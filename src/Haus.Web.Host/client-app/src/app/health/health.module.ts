import {NgModule} from "@angular/core";
import {AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";

@NgModule({
  imports: [
    AuthModule,
    CommonModule,
    SharedModule
  ]
})
export class HealthModule {

}
