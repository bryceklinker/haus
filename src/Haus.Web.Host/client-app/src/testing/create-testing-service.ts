import {Type} from "@angular/core";
import {createAppTestingModule, createTestingModule, TestModuleOptions} from "./create-testing-module";
import {TestBed} from "@angular/core/testing";
import {SHELL_PROVIDERS} from "../app/shell/services";
import {SHELL_COMPONENTS} from "../app/shell/components";
import {SharedModule} from "../app/shared/shared.module";
import {CommonModule} from "@angular/common";
import {ActionsSubject} from "@ngrx/store";
import {TestingActionsSubject, TestingAuthService} from "./fakes";
import {AuthService} from "@auth0/auth0-angular";

export interface TestServiceResult<T> {
  service: T,
  actionsSubject: TestingActionsSubject,
  authService: TestingAuthService
}
export function createFeatureTestingService<T>(service: Type<T>, options: TestModuleOptions): TestServiceResult<T> {
  TestBed.configureTestingModule(createTestingModule(options))
  return createServiceResult(service);
}

export function createAppTestingService<T>(service: Type<T>): TestServiceResult<T> {
  TestBed.configureTestingModule(createTestingModule(createAppTestingModule()))
  return createServiceResult(service);
}

function createServiceResult<T>(service: Type<T>): TestServiceResult<T> {
  const instance = TestBed.inject(service);
  // @ts-ignore
  if (instance.ngOnInit) {
    // @ts-ignore
    instance.ngOnInit();
  }
  return {
    service: instance,
    actionsSubject: TestBed.inject(ActionsSubject) as TestingActionsSubject,
    authService: TestBed.inject(AuthService) as TestingAuthService
  }
}
