import {Component, EventEmitter, Output} from "@angular/core";

@Component({
  selector: 'shell-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Output() menuClick = new EventEmitter<void>();

  handleMenuClick() {
    this.menuClick.emit();
  }
}
