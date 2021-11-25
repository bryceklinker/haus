import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {HausAuthService} from './haus-auth.service';
import {switchMap} from 'rxjs/operators';

@Injectable()
export class HausAuthInterceptor implements HttpInterceptor {
  constructor(private readonly authService: HausAuthService) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!req.url.startsWith('/api')) {
      return next.handle(req);
    }

    return this.authService.token$.pipe(
      switchMap(token => {
          return next.handle(req.clone({
            headers: req.headers.set('Authorization', `Bearer ${token}`)
          }))
      })
    )
  }
}
