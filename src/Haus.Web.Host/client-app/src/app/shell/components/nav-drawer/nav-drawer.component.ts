import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from "@angular/core";
import {appRoutes} from "../../../app-routing.module";
import {Observable} from "rxjs";
import {FeatureName, FeaturesService} from "../../../shared/features";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {map} from "rxjs/operators";

type RouteLink = {name: string, path: string};

@Component({
  selector: 'shell-nav-drawer',
  templateUrl: './nav-drawer.component.html',
  styleUrls: ['./nav-drawer.component.scss']
})
export class NavDrawerComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  @Input() isOpen: boolean = false;

  @Output() drawerClosed = new EventEmitter();

  get links$(): Observable<Array<RouteLink>> {
    return this.destroyable.register(this.featuresService.enabledFeatures$.pipe(
      map(features => this.getAvailableLinks(features))
    ));
  }

  constructor(private readonly featuresService: FeaturesService) {
  }



  handleClosed(isOpen: boolean) {
    if (isOpen)
      return;

    this.drawerClosed.emit();
  }

  ngOnInit(): void {
    this.destroyable.register(this.featuresService.load()).subscribe();
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }

  private getAvailableLinks(features: Array<FeatureName>): Array<RouteLink> {
    return appRoutes[0].children
      .filter(route => route.featureName === undefined || features.includes(route.featureName))
      .map(route => ({name: route.name, path: route.path}));
  }
}
