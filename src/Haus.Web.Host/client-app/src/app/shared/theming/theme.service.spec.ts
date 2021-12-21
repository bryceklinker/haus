import {ThemeService} from "./theme.service";
import {SharedModule} from "../shared.module";
import {createFeatureTestingService} from "../../../testing";

describe('ThemeService', () => {
  let service: ThemeService;

  beforeEach(() => {
    service = createFeatureTestingService(ThemeService, {imports: [SharedModule]}).service;
  })

  test('should default to dark theme', done => {
    service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(true);
      done();
    })
  })

  test('should be in light mode when toggled', done => {
    service.toggleTheme();
    service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(false);
      done();
    })
  })

  test('should toggle back to dark mode', done => {
    service.toggleTheme();
    service.toggleTheme();
    service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(true);
      done();
    })
  })
})
