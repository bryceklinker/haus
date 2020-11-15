import {EventEmitter} from "@angular/core";

export class TestingEventEmitter extends EventEmitter {
  constructor() {
    super();

    this.emit = jasmine.createSpy();
  }
}
