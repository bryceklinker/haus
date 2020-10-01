import '@testing-library/jest-dom';
import {TestingServer} from './testing-server';

beforeAll(() => {
    TestingServer.start();
})

afterEach(() => {
    TestingServer.reset();
})

afterAll(() => {
    TestingServer.stop();
})
