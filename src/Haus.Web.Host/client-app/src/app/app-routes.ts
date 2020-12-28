import {AuthGuard} from "@auth0/auth0-angular";
import {MainContentComponent} from "./shell/components/main-content/main-content.component";
import {Route} from "@angular/router";

export const MAIN_ROUTE: Route = {
  path: '',
  canActivate: [AuthGuard],
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
    }
  ]
}

export const APP_ROUTES: Array<Route> = [
  MAIN_ROUTE
]
