import {
  createAppTestingService,
  createTestingRouterStateSnapshot,
  eventually,
  TestingActivatedRouteSnapshot
} from '../../../testing';
import {UserRequiredGuard} from './user-required.guard';
import {RouterStateSnapshot} from '@angular/router';

describe('UserRequiredGuard', () => {
  let activatedRoute: TestingActivatedRouteSnapshot;
  let routerSnapshot: RouterStateSnapshot;

  beforeEach(() => {
    activatedRoute = new TestingActivatedRouteSnapshot();
    routerSnapshot = createTestingRouterStateSnapshot();
  });

  test('when user is unauthenticated then redirects user to login', async () => {
    const {service, authService} = createAppTestingService(UserRequiredGuard);
    authService.setIsAuthenticated(false);

    let canActivate: boolean | null = null;
    service.canActivate(activatedRoute, routerSnapshot).subscribe(value => canActivate = value);

    await eventually(() => expect(authService.loginWithRedirect).toHaveBeenCalled());
    await eventually(() => expect(canActivate).toEqual(false));
  });

  test('when user is authenticated then route activation is allowed', async () => {
    const {service, authService} = createAppTestingService(UserRequiredGuard);
    authService.setIsAuthenticated(true);

    let canActivate: boolean | null = null;
    service.canActivate(activatedRoute, routerSnapshot).subscribe(value => canActivate = value);

    await eventually(() => expect(authService.loginWithRedirect).not.toHaveBeenCalled());
    await eventually(() => expect(canActivate).toEqual(true));
  });
})

