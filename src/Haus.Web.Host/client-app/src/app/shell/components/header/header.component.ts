import {Component, EventEmitter, Output} from "@angular/core";
import {ThemeService} from "../../../shared/theming/theme.service";

@Component({
  selector: 'shell-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Output() menuClick = new EventEmitter<void>();

  constructor(private themeService: ThemeService) {
  }

  handleMenuClick() {
    this.menuClick.emit();
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
