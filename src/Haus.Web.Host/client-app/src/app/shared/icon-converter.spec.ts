import {convertLevelToIcon} from "./icon-converter";

describe('iconConverter', () => {
  it('should convert information to info icon', () => {
    expect(convertLevelToIcon('Information')).toEqual('info');
  })

  it('should convert debug to bug-report', () => {
    expect(convertLevelToIcon('Debug')).toEqual('bug_report');
  })

  it('should convert warning to warning', () => {
    expect(convertLevelToIcon('Warning')).toEqual('warning');
  })

  it('should convert error to error', () => {
    expect(convertLevelToIcon('Error')).toEqual('error');
  })

  it('should convert critical to sick', () => {
    expect(convertLevelToIcon('Critical')).toEqual('sick');
  })

  it('should convert trace to info', () => {
    expect(convertLevelToIcon('Trace')).toEqual('info');
  })

  it('should convert none to info', () => {
    expect(convertLevelToIcon('None')).toEqual('info');
  })
})
