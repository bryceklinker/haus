import {eventually, createAppTestingService} from '../../../testing';
import {NavigationService} from './navigation.service';
import {NavigationLinkModel} from '../models';

describe('NavigationService', () => {
  let service: NavigationService;

  beforeEach(() => {
    const result = createAppTestingService(NavigationService);

    service = result.service;
  });

  test('when getting links then returns each route', async () => {
    let links: Array<NavigationLinkModel>;
    service.links$.subscribe(l => links = l);

    await eventually(() => {
      expect(links).toContainEqual({name: 'Diagnostics', path: 'diagnostics'});
      expect(links).toContainEqual({name: 'Devices', path: 'devices'});
      expect(links).toContainEqual({name: 'Rooms', path: 'rooms'});
    });
  });

  test('when getting links then does not have redirect link', async () => {
    let links: Array<NavigationLinkModel>;
    service.links$.subscribe(l => links = l);

    await eventually(() => {
      expect(links).not.toContainEqual({path: '**', name: '**'});
    });
  });
});
