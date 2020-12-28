import {Type} from "@angular/core";
import {createAppTestingModule, createTestingModule, TestModuleOptions} from "./create-testing-module";
import {TestBed} from "@angular/core/testing";
import {SHELL_PROVIDERS} from "../app/shell/services";
import {SHELL_COMPONENTS} from "../app/shell/components";
import {SharedModule} from "../app/shared/shared.module";
import {CommonModule} from "@angular/common";

export interface TestServiceResult<T> {
  service: T
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
  return {
    service: TestBed.inject(service)
  }
}
