import {Component, EventEmitter, Input, Output} from "@angular/core";
import {getAvailableRoutes} from "../../../app-routes";

@Component({
  selector: 'shell-nav-drawer',
  templateUrl: './nav-drawer.component.html',
  styleUrls: ['./nav-drawer.component.scss']
})
export class NavDrawerComponent {
  readonly availableRoutes: Array<{name: string, path: string}>;
  @Input() isOpen: boolean = false;

  @Output() drawerClosed = new EventEmitter();

  constructor() {
    this.availableRoutes = getAvailableRoutes()[0]
      .children
      .map(r => ({name: r.name, path: r.path}))
  }

  handleClosed(isOpen: boolean) {
    if (isOpen)
      return;

    this.drawerClosed.emit();
  }
}
