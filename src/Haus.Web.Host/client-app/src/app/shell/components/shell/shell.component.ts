import {Component} from "@angular/core";
import {ThemeService} from "../../../shared/theming/theme.service";
import {Observable, Subscription} from "rxjs";

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent {
  isSidenavOpen: boolean = false;

  get themeClass$(): Observable<string> {
    return this.themeService.themeClass$;
  }

  constructor(private themeService: ThemeService) {

  }

  onMenuClicked() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  handleDrawerClosed() {
    this.isSidenavOpen = false;
  }
}
