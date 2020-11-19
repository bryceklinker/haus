import {ThemeService} from "./theme.service";
import {SharedModule} from "../shared.module";
import {createTestingService} from "../../../testing";

describe('ThemeService', () => {
  let service: ThemeService;

  beforeEach(() => {
    service = createTestingService(ThemeService, {imports: [SharedModule]}).service;
  })

  it('should default to dark theme', done => {
    service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(true);
      done();
    })
  })

  it('should be in light mode when toggled', done => {
    service.toggleTheme();
    service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(false);
      done();
    })
  })

  it('should toggle back to dark mode', done => {
    service.toggleTheme();
    service.toggleTheme();
    service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(true);
      done();
    })
  })
})
