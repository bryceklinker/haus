import {getWords} from "./get-words";

export function toTitleCase(value: string): string {
  return getWords(value)
    .map(capitalizeFirstLetter)
    .join(' ');
}

function capitalizeFirstLetter(value: string): string {
  return `${value[0].toUpperCase()}${value.substr(1)}`
}
