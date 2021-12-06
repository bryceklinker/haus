import 'zone.js';
import 'zone.js/testing';
import '@angular/compiler';
import 'jest-preset-angular';
import '@testing-library/jest-dom';
import {TestingServer} from "./testing";
import { TestBed } from '@angular/core/testing';
import {BrowserDynamicTestingModule, platformBrowserDynamicTesting} from "@angular/platform-browser-dynamic/testing";
import {configure} from '@testing-library/angular';

beforeAll(() => {
  TestingServer.start();
});

beforeEach(() => {
  configure({
    dom: {
      defaultHidden: true,
    }
  })
  TestBed.resetTestEnvironment();
  TestBed.initTestEnvironment(
    BrowserDynamicTestingModule,
    platformBrowserDynamicTesting(),
    {
      teardown: {
        destroyAfterEach: true
      }
    }
  );
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
