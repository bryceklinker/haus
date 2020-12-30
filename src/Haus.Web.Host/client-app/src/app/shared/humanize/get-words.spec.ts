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

  it('should split camel case words', () => {
    const original = 'thisIsACamelCaseWord';

    expect(getWords(original)).toEqual(['this', 'Is', 'A', 'Camel', 'Case', 'Word']);
  })

  it('should remove extra spaces from words', () => {
    const original = ' this is my StringHere There MightBE Spaces ';

    expect(getWords(original)).toEqual(['this', 'is', 'my', 'String', 'Here', 'There', 'Might', 'BE', 'Spaces'])
  })
})
