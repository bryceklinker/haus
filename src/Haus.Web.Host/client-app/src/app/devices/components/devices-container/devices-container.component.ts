import {Component, OnInit} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {DevicesActions} from "../../actions";

@Component({
  selector: 'devices-container',
  templateUrl: './devices-container.component.html',
  styleUrls: ['./devices-container.component.scss']
})
export class DevicesContainerComponent implements OnInit {

  constructor(private store: Store<AppState>) {
  }

  ngOnInit(): void {
    this.store.dispatch(DevicesActions.load.request());
  }

}
