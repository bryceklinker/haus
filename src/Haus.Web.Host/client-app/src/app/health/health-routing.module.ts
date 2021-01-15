import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {HealthRootComponent} from "./components";

const routes: Routes = [
  {
    path: '',
    component: HealthRootComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class HealthRoutingModule {

}
