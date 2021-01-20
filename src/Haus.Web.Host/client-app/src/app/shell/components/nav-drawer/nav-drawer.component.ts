import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from "@angular/core";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {NavigationService} from "../../services";
import {NavigationLinkModel} from "../../models";
import {Observable} from "rxjs";
import {SettingsService} from "../../../shared/settings";
import {map} from "rxjs/operators";

type RouteLink = {name: string, path: string};

@Component({
  selector: 'shell-nav-drawer',
  templateUrl: './nav-drawer.component.html',
  styleUrls: ['./nav-drawer.component.scss']
})
export class NavDrawerComponent {
  @Input() isOpen: boolean = false;

  @Output() drawerClosed = new EventEmitter();

  links$: Observable<Array<NavigationLinkModel>>
  version$: Observable<string>;

  constructor(private readonly navigationService: NavigationService,
              private readonly settingsService: SettingsService) {
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
}
