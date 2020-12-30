const SPACE_EQUIVALENT_REGEX = /(_|-|\s)/g;
const CAPITAL_LETTER_REGEX = /((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))/g;

export function getWords(value: string): Array<string> {
  return value
    .replace(CAPITAL_LETTER_REGEX, ' $1')
    .replace(SPACE_EQUIVALENT_REGEX, ' ')
    .split(' ')
    .filter(word => word.length > 0)
    .map(word => word.trim());
}
