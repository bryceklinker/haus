import {NgModule} from "@angular/core";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {SharedModule} from "../app/shared/shared.module";
import {RouterTestingModule} from "@angular/router/testing";
import {RouterModule} from "@angular/router";

@NgModule({
  imports: [
    NoopAnimationsModule,
    SharedModule,
    HttpClientTestingModule,
    RouterTestingModule.withRoutes([])
  ],
  exports: [
    SharedModule,
    RouterModule
  ]
})
export class AppTestingModule {

}
