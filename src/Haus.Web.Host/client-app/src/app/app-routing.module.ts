import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AuthGuard} from "@auth0/auth0-angular";
import {MainContentComponent} from "./shell/components/main-content/main-content.component";

const routes: Routes = [
  {
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
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
