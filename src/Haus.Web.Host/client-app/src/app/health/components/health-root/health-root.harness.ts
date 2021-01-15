import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {HealthRootComponent} from "./health-root.component";
import {Action} from "@ngrx/store";
import {HealthModule} from "../../health.module";
import {HealthDashboardHarness} from "../health-dashboard/health-dashboard.harness";

export class HealthRootHarness extends HausComponentHarness<HealthRootComponent> {
  private readonly _dashboardHarness: HealthDashboardHarness;

  get hasDashboard() {
    return this._dashboardHarness.isVisible;
  }

  get isWaitingForReport() {
    return this._dashboardHarness.isWaiting;
  }

  private constructor(result: RenderComponentResult<HealthRootComponent>) {
    super(result);

    this._dashboardHarness = HealthDashboardHarness.fromResult(result);
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(HealthRootComponent, {
      imports: [HealthModule],
      actions: actions
    });

    return new HealthRootHarness(result);
  }
}
