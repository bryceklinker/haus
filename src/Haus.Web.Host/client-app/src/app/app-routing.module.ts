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
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
