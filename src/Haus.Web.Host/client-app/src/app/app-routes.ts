import {AuthGuard} from "@auth0/auth0-angular";
import {MainContentComponent} from "./shell/components/main-content/main-content.component";
import {SettingsService} from "./shared/settings";

export function getAvailableRoutes() {
  const routes = [
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
          loadChildren: () => import('./device-simulator/device-simulator.module').then(m => m.DeviceSimulatorModule)
        }
      ]
    }
  ];
  return routes;
}
