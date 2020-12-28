import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from "@angular/core";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {NavigationService} from "../../services";
import {NavigationLinkModel} from "../../models";
import {Observable} from "rxjs";

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

  constructor(private service: NavigationService) {
    this.links$ = service.links$;
  }

  handleClosed(isOpen: boolean) {
    if (isOpen)
      return;

    this.drawerClosed.emit();
  }
}
