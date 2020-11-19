import {Type} from "@angular/core";
import {createTestingModule, TestModuleOptions} from "./create-testing-module";
import {TestBed} from "@angular/core/testing";

export interface TestServiceResult<T> {
  service: T
}
export function createTestingService<T>(service: Type<T>, options: TestModuleOptions): TestServiceResult<T> {
  TestBed.configureTestingModule(createTestingModule(options))
  return {
    service: TestBed.inject(service)
  }
}
