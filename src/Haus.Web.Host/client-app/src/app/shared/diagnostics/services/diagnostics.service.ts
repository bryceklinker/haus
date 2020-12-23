import {Injectable} from "@angular/core";
import {BehaviorSubject, Observable, of} from "rxjs";

import {SortDirection} from "../../sort-array-by";
import {HubStatus} from "../../models";
import {KNOWN_HUB_NAMES, SignalrService, SignalrServiceFactory} from "../../signalr";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {DiagnosticsMessageModel} from "../models";
import {map} from "rxjs/operators";
import {subscribeOnce} from "../../observable-extensions";

@Injectable({
  providedIn: 'root'
})
export class DiagnosticsService {
  private readonly signalrService: SignalrService;
  private readonly entityService: SortingEntityService<DiagnosticsMessageModel>;
  private readonly _allowDiscoverySubject = new BehaviorSubject<boolean>(false);

  get status$(): Observable<HubStatus> {
    return this.signalrService.status$;
  }

  get isConnected$(): Observable<boolean> {
    return this.status$.pipe(
      map(status => status === HubStatus.Connected)
    );
  }

  get allowDiscovery$(): Observable<boolean> {
    return this._allowDiscoverySubject.asObservable();
  }

  get messages$(): Observable<DiagnosticsMessageModel[]> {
    return this.entityService.entitiesArray$;
  }

  constructor(private readonly signalrServiceFactory: SignalrServiceFactory,
              private readonly api: HausApiClient) {
    this.signalrService = this.signalrServiceFactory.create(KNOWN_HUB_NAMES.diagnostics);
    this.entityService = new SortingEntityService<DiagnosticsMessageModel>(i => Date.parse(i.timestamp), SortDirection.Descending);
  }

  start(): Observable<void> {
    this.signalrService.on('OnMqttMessage', (msg: DiagnosticsMessageModel) => {
      this.entityService.executeAdd(() => of(msg))
    });
    return this.signalrService.start();
  }

  stop(): Observable<void> {
    return this.signalrService.stop();
  }

  replayMessage(model: DiagnosticsMessageModel): Observable<void> {
    return subscribeOnce(this.api.replayMessage(model));
  }

  startDiscovery() {
    return subscribeOnce(this.api.startDiscovery(), () => this._allowDiscoverySubject.next(true));
  }

  stopDiscovery() {
    return subscribeOnce(this.api.stopDiscovery(), () => this._allowDiscoverySubject.next(false));
  }

  syncDiscovery() {
    return subscribeOnce(this.api.syncDiscovery());
  }
}
