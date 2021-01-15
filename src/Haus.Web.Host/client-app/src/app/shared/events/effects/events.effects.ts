import {Actions} from "@ngrx/effects";
import {ActionsSubject, createAction} from "@ngrx/store";
import {KNOWN_HUB_NAMES, SignalrEffectsFactory, SignalrService} from "../../signalr";
import {Injectable} from "@angular/core";
import {HausEvent} from "../../models";
import {EventsActions} from "../state";
import {SharedActions} from "../../actions";

@Injectable()
export class EventsEffects {
  start$;

  constructor(private readonly actions$: Actions,
              private readonly actionsSubject: ActionsSubject,
              private readonly signalREffectsFactory: SignalrEffectsFactory) {
    const ignoreAction = createAction('ignore');
    const {start$} = this.signalREffectsFactory.createEffects({
      hubName: KNOWN_HUB_NAMES.events,
      disconnectedAction: ignoreAction,
      connectedAction: ignoreAction,
      startAction: SharedActions.initApp,
      stopAction: ignoreAction,
      hubInitializer: hub => this.initializeHub(hub)
    });

    this.start$ = start$;
  }

  private initializeHub(hub: SignalrService) {
    hub.on<HausEvent>('OnEvent', msg => {
      this.actionsSubject.next(EventsActions.fromHausEvent(msg));
    })
  }
}
