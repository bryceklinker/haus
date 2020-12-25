import {BehaviorSubject, Observable} from "rxjs";
import {SignalrService, SignalrServiceFactory} from "../../shared/signalr";
import {SettingsService} from "../../shared/settings";
import {Injectable, OnDestroy} from "@angular/core";
import {DestroyableSubject} from "../../shared/destroyable-subject";
import {DeviceSimulatorStateModel} from "../models";
import {HttpClient} from "@angular/common/http";
import {ListResult} from "../../shared/models";
import {tap} from "rxjs/operators";

@Injectable()
export class DeviceSimulatorService implements OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  private readonly stateSubject = new BehaviorSubject<DeviceSimulatorStateModel>({devices: [], devicesById: {}});
  private readonly deviceTypesSubject = new BehaviorSubject<string[]>([]);
  private readonly signalRService: SignalrService;

  get simulatorUrl(): string {
    return this.settingService.settings.deviceSimulator.url;
  }

  get hubUrl(): string {
    return `${this.simulatorUrl}/hubs/devices`;
  }

  get state$(): Observable<DeviceSimulatorStateModel> {
    return this.destroyable.register(this.stateSubject.asObservable());
  }

  get deviceTypes$(): Observable<string[]> {
    return this.destroyable.register(this.deviceTypesSubject.asObservable());
  }

  constructor(private readonly settingService: SettingsService,
              private readonly signalrServiceFactory: SignalrServiceFactory,
              private readonly http: HttpClient) {
    this.signalRService = signalrServiceFactory.createFromUrl(this.hubUrl);
  }

  start(): Observable<void> {
    this.signalRService.on<DeviceSimulatorStateModel>('OnStateChange', state => {
      this.stateSubject.next(state);
    });
    return this.destroyable.register(this.signalRService.start());
  }

  stop(): Observable<void> {
    return this.destroyable.register(this.signalRService.stop());
  }

  addDevice(model: { deviceType: string }): Observable<any> {
    return this.destroyable.register(this.http.post(`${this.simulatorUrl}/api/devices`, model));
  }

  getDeviceTypes() {
    return this.destroyable.register(this.http.get<ListResult<string>>(`${this.simulatorUrl}/api/deviceTypes`).pipe(
      tap(result => this.deviceTypesSubject.next(result.items))
    ));
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }
}
