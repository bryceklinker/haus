import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {RoomsRootComponent} from "./components/rooms-root/rooms-root.component";
import {RoomDetailRootComponent} from "./components/room-detail-root/room-detail-root.component";

const routes: Routes = [
  {
    path: '',
    component: RoomsRootComponent,
    children: [
      {path: ':roomId', component: RoomDetailRootComponent}
    ]
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
