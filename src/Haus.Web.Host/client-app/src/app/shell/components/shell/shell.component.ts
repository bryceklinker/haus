import {Component, HostBinding, OnDestroy, OnInit} from "@angular/core";
import {ThemeService} from "../../../shared/theming/theme.service";
import {Observable, Subscription} from "rxjs";

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent implements OnInit, OnDestroy {
  private _isDarkTheme: boolean = true;
  private subscription: Subscription | null = null;
  isSidenavOpen: boolean = false;

  @HostBinding("class.dark-theme")
  get isDarkTheme(): boolean {
    return this._isDarkTheme;
  }
  @HostBinding("class.light-theme")
  get isLightTheme(): boolean {
    return !this._isDarkTheme;
  }

  constructor(private themeService: ThemeService) {

  }

  onMenuClicked() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  handleDrawerClosed() {
    this.isSidenavOpen = false;
  }

  ngOnDestroy(): void {
    this.subscription = this.themeService.isDarkTheme$.subscribe(isDarkTheme => {
      this._isDarkTheme = isDarkTheme;
    })
  }

  ngOnInit(): void {
    this.subscription?.unsubscribe();
  }
}
