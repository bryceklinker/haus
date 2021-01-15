import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {HealthRootComponent} from "./health-root.component";
import {Action} from "@ngrx/store";
import {HealthModule} from "../../health.module";
import {HealthDashboardHarness} from "../health-dashboard/health-dashboard.harness";
import {RecentEventsHarness} from "../recent-events/recent-events.harness";

export class HealthRootHarness extends HausComponentHarness<HealthRootComponent> {
  private readonly _dashboardHarness: HealthDashboardHarness;
  private readonly _eventsHarness: RecentEventsHarness;

  get hasDashboard() {
    return this._dashboardHarness.isVisible;
  }

  get isWaitingForReport() {
    return this._dashboardHarness.isWaiting;
  }

  get recentEvents() {
    return this._eventsHarness.events;
  }

  private constructor(result: RenderComponentResult<HealthRootComponent>) {
    super(result);

    this._dashboardHarness = HealthDashboardHarness.fromResult(result);
    this._eventsHarness = RecentEventsHarness.fromResult(result);
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(HealthRootComponent, {
      imports: [HealthModule],
      actions: actions
    });

    return new HealthRootHarness(result);
  }
}
