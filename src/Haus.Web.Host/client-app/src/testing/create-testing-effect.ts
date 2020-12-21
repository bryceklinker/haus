import {Subject} from "rxjs";
import {Action, ActionsSubject} from "@ngrx/store";
import {Type} from "@angular/core";
import {createTestingModule, TestModuleOptions} from "./create-testing-module";
import {TestBed} from "@angular/core/testing";

export interface TestEffectResult<T> {
    effects: T;
    actions$: Subject<Action>;
}

export function createTestingEffect<T>(effects: Type<T>, options: TestModuleOptions): TestEffectResult<T> {
    const effectsOptions = createTestingModule(options);
    TestBed.configureTestingModule(effectsOptions);
    return {
        effects: TestBed.inject(effects),
        actions$: TestBed.inject(ActionsSubject)
    }
}
