import {Component} from '@angular/core';
import {Observable} from 'rxjs';
import {UserModel} from '../../../shared/auth';
import {AppState} from '../../../app.state';
import {Store} from '@ngrx/store';
import {selectUser} from '../../../shared/auth/state';
import {map} from 'rxjs/operators';

@Component({
  selector: 'welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.scss']
})
export class WelcomeComponent {
  get user$(): Observable<UserModel | null> {
    return this.store.select(selectUser)
  }

  get username$(): Observable<string> {
    return this.user$.pipe(
      map(user => user ? user.name : '')
    );
  }

  constructor(private readonly store: Store<AppState>) {
  }
}
