import {toTitleCase} from "./index";

describe('toTitleCase', () => {
  test('should capitalize first letter of word', () => {
    expect(toTitleCase('devices')).toEqual('Devices');
  })

  test('should capitalize the letter of each word', () => {
    expect(toTitleCase('device-simulator')).toEqual('Device Simulator');
  })
})
