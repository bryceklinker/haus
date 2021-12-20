import {ActivatedRouteSnapshot, RouterStateSnapshot} from '@angular/router';
import {TestingActivatedRouteSnapshot} from './testing-activated-route-snapshot';

export function createTestingRouterStateSnapshot(): RouterStateSnapshot {
  return {
    url: '',
    get root(): ActivatedRouteSnapshot {
      return new TestingActivatedRouteSnapshot()
    }
  }
}
