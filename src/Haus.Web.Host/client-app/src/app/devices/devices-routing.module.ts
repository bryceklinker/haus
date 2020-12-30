import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {DevicesRootComponent} from "./components/devices-root/devices-root.component";
import {DeviceDetailRootComponent} from "./components/device-detail-root/device-detail-root.component";

const routes: Routes = [
  {
    path: '',
    component: DevicesRootComponent,
    children: [
      {path: ':deviceId', component: DeviceDetailRootComponent}
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
