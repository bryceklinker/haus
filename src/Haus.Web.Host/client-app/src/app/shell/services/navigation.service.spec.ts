import {eventually, createAppTestingService} from "../../../testing";
import {NavigationService} from "./navigation.service";
import {NavigationLinkModel} from "../models";

describe('NavigationService', () => {
  let service: NavigationService;

  beforeEach(() => {
    const result = createAppTestingService(NavigationService);

    service = result.service;
  })

  it('should return route links for main route', async () => {
    let links: Array<NavigationLinkModel>;
    service.links$.subscribe(l => links = l);

    await eventually(() => {
      expect(links).toContainEqual({name: 'Diagnostics', path: 'diagnostics'})
      expect(links).toContainEqual({name: 'Devices', path: 'devices'})
      expect(links).toContainEqual({name: 'Rooms', path: 'rooms'})
    })
  })
})
