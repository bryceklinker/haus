import {TestBed} from "@angular/core/testing";

import {
  createAppTestingService, eventually, ModelFactory,
  TestingActionsSubject,
  TestingSignalrHubConnection,
  TestingSignalrHubConnectionFactory
} from "../../../../testing";
import {KNOWN_HUB_NAMES, SignalrHubConnectionFactory} from "../../signalr";
import {EventsEffects} from "./events.effects";
import {EventsActions} from "../state";
import {HausEvent, RoomLightingChangedEvent} from "../../models";
import {SharedActions} from "../../actions";

describe('EventsEffects', () => {
  let actions$: TestingActionsSubject;
  let signalrHub: TestingSignalrHubConnection;

  beforeEach(() => {
    const result = createAppTestingService(EventsEffects);
    actions$ = result.actionsSubject;
    signalrHub = (TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrHubConnectionFactory)
      .getTestingHub(KNOWN_HUB_NAMES.events);
  })

  it('should start signalr connection when initialized', async () => {
      actions$.next(SharedActions.initApp());

      await eventually(() => {
        expect(signalrHub.start).toHaveBeenCalled();
      })
  })

  it('should dispatch event action when signalr message triggered', async () => {
    actions$.next(SharedActions.initApp());
    signalrHub.triggerStart();

    const hausEvent: HausEvent<RoomLightingChangedEvent> = ModelFactory.createHausEvent<RoomLightingChangedEvent>({
      type: 'room_lighting_changed',
      payload: {
        room: ModelFactory.createRoomModel(),
        lighting: ModelFactory.createLighting()
      }
    });
    signalrHub.triggerMessage('OnEvent', hausEvent);
    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(EventsActions.fromHausEvent({...hausEvent, isEvent: true}));
    })
  })
})
