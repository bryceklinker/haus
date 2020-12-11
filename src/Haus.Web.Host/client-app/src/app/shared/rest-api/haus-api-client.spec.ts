import {HausApiClient} from "./haus-api-client";
import {HttpTestingController} from "@angular/common/http/testing";
import {createTestingService} from "../../../testing";
import {SharedModule} from "../shared.module";
import {TestBed} from "@angular/core/testing";
import {ModelFactory} from "../../../testing/model-factory";

describe('HausApiClient', () => {
  let hausApiClient: HausApiClient;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    const {service} = createTestingService(HausApiClient, {imports: [SharedModule]});
    httpTestingController = TestBed.get(HttpTestingController);
    hausApiClient = service;
  })

  it('should return devices with device type arrays', done => {
    hausApiClient.getDevices().subscribe(result => {
      expect(result.items[0].deviceType).toEqual(['Light']);
      done();
    })

    httpTestingController.expectOne('/api/devices').flush(ModelFactory.createListResult(
      {...ModelFactory.createDeviceModel(), deviceType: 'Light'}
    ))
  })
})
