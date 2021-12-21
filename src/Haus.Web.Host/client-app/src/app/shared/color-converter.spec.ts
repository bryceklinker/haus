import {toHexFromRGB} from './color-converter';

describe('colorConverter', () => {
  test('should convert rgb color to hex', () => {
    const hex = toHexFromRGB(9, 8, 15);

    expect(hex).toEqual('#09080F');
  })
})
