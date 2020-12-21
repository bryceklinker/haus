import {OverlayRef} from "@angular/cdk/overlay";

export class TestingOverlayRef extends OverlayRef {
    constructor() {
        super(<any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn());
    }
}
