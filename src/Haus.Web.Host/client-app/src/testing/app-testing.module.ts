import {NgModule} from "@angular/core";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {SharedModule} from "../app/shared/shared.module";
import {RouterTestingModule} from "@angular/router/testing";
import {SHELL_COMPONENTS} from "../app/shell/components";

@NgModule({
  declarations: [
    ...SHELL_COMPONENTS
  ],
  imports: [
    SharedModule,
    NoopAnimationsModule,
    HttpClientTestingModule,
    RouterTestingModule.withRoutes([]),
  ]
})
export class AppTestingModule {

}
