import {Component, OnInit} from "@angular/core";
import {ThemeService} from "../../../shared/theming/theme.service";
import {Observable, Subscription} from "rxjs";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {SharedActions} from "../../../shared/actions";

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent implements OnInit{
  isSidenavOpen: boolean = false;

  get themeClass$(): Observable<string> {
    return this.themeService.themeClass$;
  }

  constructor(private readonly themeService: ThemeService,
              private readonly store: Store<AppState>) {

  }

  ngOnInit(): void {
    this.store.dispatch(SharedActions.initApp());
  }

  onMenuClicked() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  handleDrawerClosed() {
    this.isSidenavOpen = false;
  }
}
