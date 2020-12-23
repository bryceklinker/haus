import {Injectable, OnDestroy} from "@angular/core";
import {BehaviorSubject, Observable, of} from "rxjs";
import {map, takeUntil, tap} from "rxjs/operators";

import {SortDirection} from "../../sort-array-by";
import {HubStatus} from "../../models";
import {KNOWN_HUB_NAMES, SignalrService, SignalrServiceFactory} from "../../signalr";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {DiagnosticsMessageModel} from "../models";
import {DestroyableSubject} from "../../destroyable-subject";

@Injectable({
  providedIn: 'root'
})
export class DiagnosticsService implements OnDestroy{
  private readonly signalrService: SignalrService;
  private readonly entityService: SortingEntityService<DiagnosticsMessageModel>;
  private readonly allowDiscoverySubject = new BehaviorSubject<boolean>(false);
  private readonly destroyable = new DestroyableSubject();

  get status$(): Observable<HubStatus> {
    return this.destroyable.register(this.signalrService.status$);
  }

  get isConnected$(): Observable<boolean> {
    return this.destroyable.register(this.status$.pipe(
      map(status => status === HubStatus.Connected)
    ));
  }

  get allowDiscovery$(): Observable<boolean> {
    return this.destroyable.register(this.allowDiscoverySubject);
  }

  get messages$(): Observable<DiagnosticsMessageModel[]> {
    return this.destroyable.register(this.entityService.entitiesArray$);
  }

  constructor(private readonly signalrServiceFactory: SignalrServiceFactory,
              private readonly api: HausApiClient) {
    this.signalrService = this.signalrServiceFactory.create(KNOWN_HUB_NAMES.diagnostics);
    this.entityService = new SortingEntityService<DiagnosticsMessageModel>(i => Date.parse(i.timestamp), SortDirection.Descending);
  }

  start(): Observable<void> {
    this.signalrService.on('OnMqttMessage', (msg: DiagnosticsMessageModel) => {
      this.destroyable.register(this.entityService.executeAdd(() => of(msg))).subscribe();
    });
    return this.destroyable.register(this.signalrService.start());
  }

  stop(): Observable<void> {
    return this.destroyable.register(this.signalrService.stop());
  }

  replayMessage(model: DiagnosticsMessageModel): Observable<void> {
    return this.destroyable.register(this.api.replayMessage(model));
  }

  startDiscovery() {
    return this.destroyable.register(this.api.startDiscovery().pipe(
      tap(() => this.allowDiscoverySubject.next(true)),
    ));
  }

  stopDiscovery() {
    return this.destroyable.register(this.api.stopDiscovery().pipe(
      tap(() => this.allowDiscoverySubject.next(false)),
    ));
  }

  syncDiscovery() {
    return this.destroyable.register(this.api.syncDiscovery());
  }

  ngOnDestroy(): void {
    this.signalrService.destroy();
    this.destroyable.destroy();
  }
}
