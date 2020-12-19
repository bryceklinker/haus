import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {RoomsContainerComponent} from "./components/rooms-container/rooms-container.component";

const routes: Routes = [
  {
    path: '',
    component: RoomsContainerComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class RoomsRoutingModule {}
