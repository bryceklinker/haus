import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import {AuthGuard} from "@auth0/auth0-angular";
import {MainContentComponent} from "./shell/components/main-content/main-content.component";
import {FeatureName} from "./shared/features";
import {FeatureGuard} from "./shared/features/guards/feature.guard";

export const appRoutes = [
  {
    path: '',
    canActivate: [AuthGuard],
    component: MainContentComponent,
    children: [
      {
        name: 'Diagnostics',
        path: 'diagnostics',
        loadChildren: () => import('./diagnostics/diagnostics.module').then(m => m.DiagnosticsModule)
      },
      {
        name: 'Devices',
        path: 'devices',
        loadChildren: () => import('./devices/devices.module').then(m => m.DevicesModule)
      },
      {
        name: 'Rooms',
        path: 'rooms',
        loadChildren: () => import('./rooms/rooms.module').then(m => m.RoomsModule)
      },
      {
        name: 'Device Simulator',
        path: 'device-simulator',
        featureName: FeatureName.DeviceSimulator,
        canActivate: [FeatureGuard],
        loadChildren: () => import('./device-simulator/device-simulator.module').then(m => m.DeviceSimulatorModule)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule]
})
export class AppRoutingModule {

}
