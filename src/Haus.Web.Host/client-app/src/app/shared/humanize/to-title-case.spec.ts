import {toTitleCase} from "./index";

describe('toTitleCase', () => {
  it('should capitalize first letter of word', () => {
    expect(toTitleCase('devices')).toEqual('Devices');
  })

  it('should capitalize the letter of each word', () => {
    expect(toTitleCase('device-simulator')).toEqual('Device Simulator');
  })
})
