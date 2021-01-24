import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from "@angular/core";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {NavigationService} from "../../services";
import {NavigationLinkModel} from "../../models";
import {Observable} from "rxjs";
import {SettingsService} from "../../../shared/settings";
import {map} from "rxjs/operators";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {selectHasLatestVersionError, selectIsUpdateAvailable, ShellActions} from "../../state";

type RouteLink = {name: string, path: string};

@Component({
  selector: 'shell-nav-drawer',
  templateUrl: './nav-drawer.component.html',
  styleUrls: ['./nav-drawer.component.scss']
})
export class NavDrawerComponent implements OnInit {
  @Input() isOpen: boolean = false;

  @Output() drawerClosed = new EventEmitter();

  links$: Observable<Array<NavigationLinkModel>>
  version$: Observable<string>;
  isUpdateAvailable$: Observable<boolean>;
  hasUpdateError$: Observable<boolean>

  constructor(private readonly navigationService: NavigationService,
              private readonly settingsService: SettingsService,
              private readonly store: Store<AppState>) {
    this.isUpdateAvailable$ = store.select(selectIsUpdateAvailable);
    this.hasUpdateError$ = store.select(selectHasLatestVersionError);

    this.links$ = navigationService.links$;
    this.version$ = settingsService.settings$.pipe(
      map(settings => settings.version)
    );
  }

  handleClosed(isOpen: boolean) {
    if (isOpen)
      return;

    this.drawerClosed.emit();
  }

  ngOnInit(): void {
    this.store.dispatch(ShellActions.loadLatestVersion.request());
  }
}
