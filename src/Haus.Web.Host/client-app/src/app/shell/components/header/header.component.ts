import {Component, EventEmitter, Input, Output} from "@angular/core";
import {ThemeService} from "../../../shared/theming/theme.service";
import {UserModel} from "../../../shared/auth";

@Component({
  selector: 'shell-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Input() user: UserModel | null = null;
  @Output() toggleMenu = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();

  get profilePicture(): string | null {
    return this.user ? this.user.picture : null;
  }

  constructor(private themeService: ThemeService) {
  }

  onToggleMenu() {
    this.toggleMenu.emit();
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  onLogout() {
    this.logout.emit();
  }
}
