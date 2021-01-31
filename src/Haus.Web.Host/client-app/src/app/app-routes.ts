import {MainContentComponent} from "./shell/components/main-content/main-content.component";
import {Route} from "@angular/router";
import {LatestVersionDetailsRootComponent} from "./shell/components/latest-version-details-root/latest-version-details-root.component";
import {UserRequiredGuard} from "./shared/auth";

export const MAIN_ROUTE: Route = {
  path: '',
  canActivate: [UserRequiredGuard],
  component: MainContentComponent,
  children: [
    {
      path: 'diagnostics',
      loadChildren: () => import('./diagnostics/diagnostics.module').then(m => m.DiagnosticsModule)
    },
    {
      path: 'devices',
      loadChildren: () => import('./devices/devices.module').then(m => m.DevicesModule)
    },
    {
      path: 'rooms',
      loadChildren: () => import('./rooms/rooms.module').then(m => m.RoomsModule)
    },
    {
      path: 'device-simulator',
      loadChildren: () => import('./device-simulator/device-simulator.module').then(m => m.DeviceSimulatorModule)
    },
    {
      path: 'health',
      loadChildren: () => import('./health/health.module').then(m => m.HealthModule)
    },
    {
      path: 'latest-version',
      component: LatestVersionDetailsRootComponent
    }
  ]
}

export const APP_ROUTES: Array<Route> = [
  MAIN_ROUTE
]
