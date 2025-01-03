import {EventEmitter} from "@angular/core";

export class TestingEventEmitter<T = any> extends EventEmitter<T> {
  private _emittedEvents: Array<T>;

  get emittedEvents(): Array<T> {
    return this._emittedEvents;
  }

  constructor() {
    super();
    this._emittedEvents = [];
    jest.spyOn(this as TestingEventEmitter, 'emit');
  }

  emit(value: T) {
    this._emittedEvents.push(value);
    super.emit(value);
  }
}
