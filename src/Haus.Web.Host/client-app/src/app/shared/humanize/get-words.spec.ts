import {getWords} from "./get-words";

const SYMBOLS_TO_CONVERT_TO_SPACES = [
  '-',
  '_',
  ' '
]

describe('getWords', () => {
  SYMBOLS_TO_CONVERT_TO_SPACES.forEach(symbol => {
    it(`should convert "${symbol}" to space`, () => {
      const original = `device${symbol}simulator`;

      expect(getWords(original)).toEqual(['device', 'simulator']);
    })
  })
})
