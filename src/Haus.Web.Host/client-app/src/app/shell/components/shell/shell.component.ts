import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";

import {ThemeService} from "../../../shared/theming/theme.service";
import {AppState} from "../../../app.state";
import {SharedActions} from "../../../shared/actions";
import {AuthActions, UserModel} from "../../../shared/auth";
import {selectUser} from "../../../shared/auth/state";
import {map} from "rxjs/operators";

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent implements OnInit{
  isSidenavOpen: boolean = false;
  user$: Observable<UserModel | null>;

  get isLoading$(): Observable<boolean> {
    return this.user$.pipe(
      map(user => user === null || user === undefined)
    )
  }

  get themeClass$(): Observable<string> {
    return this.themeService.themeClass$;
  }

  constructor(private readonly themeService: ThemeService,
              private readonly store: Store<AppState>) {
    this.user$ = store.select(selectUser);
  }

  ngOnInit(): void {
    this.store.dispatch(SharedActions.initApp());
  }

  onToggleMenu() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  handleDrawerClosed() {
    this.isSidenavOpen = false;
  }

  onLogout() {
    this.store.dispatch(AuthActions.logout());
  }
}
