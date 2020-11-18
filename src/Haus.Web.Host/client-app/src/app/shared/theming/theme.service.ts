import {Injectable} from "@angular/core";
import {BehaviorSubject, Observable} from "rxjs";
import {map} from "rxjs/operators";
import {DARK_THEME_CLASS_NAME, LIGHT_THEME_CLASS_NAME} from "./theme-palettes";

@Injectable()
export class ThemeService {
  private _themeClass$: BehaviorSubject<string>;

  private get themeClass(): string {
    return this._themeClass$.value;
  }

  get themeClass$(): Observable<string> {
    return this._themeClass$.asObservable();
  }

  get isDarkTheme$(): Observable<boolean> {
    return this._themeClass$.pipe(
      map(t => t === DARK_THEME_CLASS_NAME)
    );
  }

  constructor() {
    this._themeClass$ = new BehaviorSubject<string>(DARK_THEME_CLASS_NAME);
  }

  toggleTheme() {
    if (this.themeClass === DARK_THEME_CLASS_NAME) {
      this._themeClass$.next(LIGHT_THEME_CLASS_NAME);
    } else {
      this._themeClass$.next(DARK_THEME_CLASS_NAME);
    }
  }
}
