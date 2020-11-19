import {createSpyObject, SpyObject} from "@ngneat/spectator/jest";
import {AuthService} from "@auth0/auth0-angular";
import {BehaviorSubject} from "rxjs";

export interface TestingAuthService extends SpyObject<AuthService> {
  setToken(token: string): void;
}

export function createTestingAuthService(): TestingAuthService {
  const token$ = new BehaviorSubject<string>('');
  const service = <TestingAuthService>createSpyObject(AuthService);
  service.setToken = (token) => token$.next(token);
  service.getAccessTokenSilently.andReturn(token$.asObservable());
  return service;
}
