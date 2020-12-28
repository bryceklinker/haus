const SPACE_EQUIVALENT_REGEX = /(_|-|\s)/g;

export function getWords(value: string): Array<string> {
  return value.replace(SPACE_EQUIVALENT_REGEX, ' ')
    .split(' ');
}
