import {_MatDialogContainerBase} from "@angular/material/dialog";

export class TestingContainerInstance extends _MatDialogContainerBase {
    constructor() {
        super(<any>jest.fn(), <any>jest.fn(), <any>jest.fn(), <any>jest.fn(), {}, <any>jest.fn());
    }

    _startExitAnimation(): void {
    }

}
