import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import {getAvailableRoutes} from "./app-routes";

@NgModule({
  imports: [RouterModule.forRoot(getAvailableRoutes())],
  exports: [RouterModule]
})
export class AppRoutingModule { }
