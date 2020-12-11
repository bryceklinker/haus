import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {DevicesContainerComponent} from "./components/devices-container/devices-container.component";
import {DeviceDetailComponent} from "./components/device-detail/device-detail.component";

const routes: Routes = [
  {
    path: '',
    component: DevicesContainerComponent,
    pathMatch: 'full',
    children: [
      {path: ':deviceId', component: DeviceDetailComponent}
    ]
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
