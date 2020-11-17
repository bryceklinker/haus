import {Component, EventEmitter, Input, Output} from "@angular/core";

@Component({
  selector: 'shell-nav-drawer',
  templateUrl: './nav-drawer.component.html',
  styleUrls: ['./nav-drawer.component.scss']
})
export class NavDrawerComponent {
  @Input() isOpen: boolean = false;

  @Output() drawerClosed = new EventEmitter();

  handleClosed(isOpen: boolean) {
    if (isOpen)
      return;

    this.drawerClosed.emit();
  }
}
