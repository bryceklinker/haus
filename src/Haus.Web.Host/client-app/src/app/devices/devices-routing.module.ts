import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {DevicesContainerComponent} from "./components/devices-container/devices-container.component";
import {DeviceDetailContainerComponent} from "./components/device-detail-container/device-detail-container.component";

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: DevicesContainerComponent,
    children: [
      {path: ':deviceId', component: DeviceDetailContainerComponent}
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
