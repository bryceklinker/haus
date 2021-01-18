export function convertLevelToIcon(level: string): string {
  switch (level.toLowerCase()) {
    case 'debug':
      return 'bug_report';
    case 'warning':
      return 'warning';
    case 'error':
      return 'error';
    case 'critical':
      return 'sick';
    default:
      return 'info';
  }
}
