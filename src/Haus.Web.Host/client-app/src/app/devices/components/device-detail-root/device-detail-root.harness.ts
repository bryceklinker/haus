import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";
import {MatSelectHarness} from "@angular/material/select/testing";
import {MatButtonHarness} from "@angular/material/button/testing";

import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {DeviceDetailRootComponent} from "./device-detail-root.component";
import {DeviceModel} from "../../../shared/models";
import {DevicesModule} from "../../devices.module";

export class DeviceDetailRootHarness extends HausComponentHarness<DeviceDetailRootComponent> {
    get dispatchedActions(): Array<Action> {
        return this.store.dispatchedActions;
    }

    get deviceDetail() {
        return screen.getByTestId('device-detail');
    }

    async getLightTypes() {
        const lightTypesSelect = await this.getMatHarnessByTestId(MatSelectHarness.with, 'light-type-select');
        await lightTypesSelect.open();
        return await lightTypesSelect.getOptions();
    }

    async saveDevice() {
        const saveButton = await this.getMatHarnessByTestId(MatButtonHarness.with, 'save-device-btn');
        await saveButton.click();
    }

    async saveConstraints() {
        const saveButton = await this.getMatHarnessByTestId(MatButtonHarness.with, 'save-constraints-btn');
        await saveButton.click();
    }

    static async render(device: DeviceModel, ...actions: Action[]) {
        const result = await DeviceDetailRootHarness.renderRoot(...actions);
        result.activatedRoute.triggerParamsChange({deviceId: `${device.id}`});

        result.detectChanges();
        await result.fixture.whenRenderingDone();

        return new DeviceDetailRootHarness(result);
    }

    private static renderRoot(...actions: Action[]) {
        return renderFeatureComponent(DeviceDetailRootComponent, {
            imports: [DevicesModule],
            actions: actions
        })
    }
}
