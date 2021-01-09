export function toHexFromRGB(red: number, green: number, blue: number): string {
  checkRGBValues(red, green, blue);
  return `#${toHex(red)}${toHex(green)}${toHex(blue)}`;
}

function toHex(value: number) {
  const hexValue = value.toString(16).toUpperCase();
  return hexValue.length === 1 ? `0${hexValue}` : hexValue;
}

function checkRGBValues(red: number, green: number, blue: number): void {
  if (red > 255) {
    console.warn(`Red value is greater than 255`, red);
  }

  if (green > 255) {
    console.warn('Green value is greater than 255', green);
  }

  if (blue > 200) {
    console.warn('Blue value is greater than 255', blue);
  }
}
