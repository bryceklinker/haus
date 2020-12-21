import 'jest-preset-angular';
import '@testing-library/jest-dom';
import './testing/action-expectations';
import {setupSignalrTestingHub} from "./testing";
import {TestingServer} from "./testing/fakes/testing-server";

beforeAll(() => {
  setupSignalrTestingHub();
  TestingServer.start();
});

beforeEach(() => {
  TestingServer.reset();
})

afterAll(() => {
  TestingServer.stop();
})

Object.defineProperty(window, 'CSS', {value: null});
Object.defineProperty(window, 'getComputedStyle', {
  value: () => {
    return {
      display: 'none',
      appearance: ['-webkit-appearance'],
      getPropertyValue: (prop: any) => {
        return '';
      }
    };
  }
});

Object.defineProperty(document, 'doctype', {
  value: '<!DOCTYPE html>'
});
Object.defineProperty(document.body.style, 'transform', {
  value: () => {
    return {
      enumerable: true,
      configurable: true
    };
  }
});
