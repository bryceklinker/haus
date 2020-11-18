import {createFeatureServiceFactory} from "../../../testing/create-feature-service-factory";
import {ThemeService} from "./theme.service";
import {SharedModule} from "../shared.module";

describe('ThemeService', () => {
  const createService = createFeatureServiceFactory(ThemeService, SharedModule);


  it('should default to dark theme', done => {
    const spectator = createService();
    spectator.service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(true);
      done();
    })
  })

  it('should be in light mode when toggled', done => {
    const spectator = createService();
    spectator.service.toggleTheme();
    spectator.service.isDarkTheme$.subscribe(isDarkTheme => {
      expect(isDarkTheme).toEqual(false);
      done();
    })
  })
})
