import 'jest-preset-angular';
import '@testing-library/jest-dom';
import {TestingServer} from "./testing";

beforeAll(() => {
  TestingServer.start();
});

beforeEach(() => {
  TestingServer.reset();
  TestingServer.setupDefaultApiEndpoints();
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
