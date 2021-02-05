import {createFeatureTestingService, TestingMatDialog} from "../../../../testing";
import {LoadingDialogService} from "./loading-dialog.service";
import {SharedModule} from "../../shared.module";
import {LoadingDialogComponent} from "./loading-dialog.component";

describe('LoadingDialogService', () => {
  let service: LoadingDialogService;
  let matDialog: TestingMatDialog;

  beforeEach(() => {
    const result = createFeatureTestingService(LoadingDialogService,{imports: [SharedModule]});
    service = result.service;
    matDialog = result.matDialog;
  })

  it('should open loading dialog when opened', () => {
    const options = {text: 'hello'};

    service.open(options);

    expect(matDialog.open).toHaveBeenCalledWith(LoadingDialogComponent, {data: options});
  })
})
