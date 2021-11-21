import {MatDialogRef} from "@angular/material/dialog";
import {Observable, Subject} from "rxjs";
import {Injectable} from "@angular/core";
import {v4 as uuid} from 'uuid';
import {TestingContainerInstance} from "./testing-container-instance";
import {TestingOverlayRef} from "./testing-overlay-ref";

@Injectable()
export class TestingMatDialogRef<T = any, R = any> extends MatDialogRef<T, R> {
  private readonly afterClosed$: Subject<any>;

  constructor() {
    super(new TestingOverlayRef(), new TestingContainerInstance(), uuid());
    jest.spyOn(this as TestingMatDialogRef, 'close');

    this.afterClosed$ = new Subject<void>();
  }

  afterClosed(): Observable<any | undefined> {
    return this.afterClosed$.asObservable();
  }

  close(dialogResult?: R) {
    this.afterClosed$.next(dialogResult);
  }

  triggerAfterClosed(result: any) {
    this.afterClosed$.next(result);
  }
}
