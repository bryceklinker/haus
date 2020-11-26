import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {DevicesContainerComponent} from "./components/devices-container/devices-container.component";

const routes: Routes = [
  {
    path: '',
    component: DevicesContainerComponent,
    pathMatch: 'full'
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
export class DevicesRoutingModule {

}
