import {convertLevelToIcon} from "./icon-converter";

describe('iconConverter', () => {
  test('should convert information to info icon', () => {
    expect(convertLevelToIcon('Information')).toEqual('info');
  })

  test('should convert debug to bug-report', () => {
    expect(convertLevelToIcon('Debug')).toEqual('bug_report');
  })

  test('should convert warning to warning', () => {
    expect(convertLevelToIcon('Warning')).toEqual('warning');
  })

  test('should convert error to error', () => {
    expect(convertLevelToIcon('Error')).toEqual('error');
  })

  test('should convert critical to sick', () => {
    expect(convertLevelToIcon('Critical')).toEqual('sick');
  })

  test('should convert trace to info', () => {
    expect(convertLevelToIcon('Trace')).toEqual('info');
  })

  test('should convert none to info', () => {
    expect(convertLevelToIcon('None')).toEqual('info');
  })
})
