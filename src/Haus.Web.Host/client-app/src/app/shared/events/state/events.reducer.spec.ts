import {generateStateFromActions} from "../../../../testing/app-state-generator";
import {eventsReducer} from "./events.reducer";
import {EventsActions} from "./actions";
import {EventsState} from "./events.state";
import {ModelFactory} from "../../../../testing";

describe('eventsReducer', () => {
  it('should have empty events when initialized', () => {
    const state = generateStateFromActions(eventsReducer);

    expect(state.events).toHaveLength(0);
  })

  it('should have event when event is received', () => {
    const event = {
      type: 'idk',
      timestamp: '2020-01-09',
      payload: null
    };
    const state = generateStateFromActions(eventsReducer,
      EventsActions.fromHausEvent(event)
    );

    expect(state.events).toContainEqual({...event, isEvent: true});
  })

  it('should only keep the last 100 events in state', () => {
    const state = generateStateWithEvents(200);

    expect(state.events).toHaveLength(100);
  })

  it('should order events by timestamp descending', () => {
    const firstDate = '2020-09-23T05:33:01.123Z';
    const secondDate = '2020-09-23T05:33:02.123Z';
    const thirdDate = '2020-09-23T05:33:03.123Z';
    const state = generateStateFromActions(eventsReducer,
      EventsActions.fromHausEvent({type: '', payload: null, timestamp: secondDate}),
      EventsActions.fromHausEvent({type: '', payload: null, timestamp: firstDate}),
      EventsActions.fromHausEvent({type: '', payload: null, timestamp: thirdDate}),
    );

    expect(state.events[0].timestamp).toEqual(thirdDate);
    expect(state.events[1].timestamp).toEqual(secondDate);
    expect(state.events[2].timestamp).toEqual(firstDate);
  })

  it('should maintain state when action is not an event', () => {
    const state = generateStateFromActions(eventsReducer,
      EventsActions.fromHausEvent(ModelFactory.createHausEvent()),
      {type: 'ignore'}
    );

    expect(state.events).toHaveLength(1);
  })

  function generateStateWithEvents(count: number): EventsState {
    const actions = [];
    for (let i = 0; i < count; i++) {
      actions.push(EventsActions.fromHausEvent({type: '', timestamp: new Date().toISOString(), payload: null}));
    }
    return generateStateFromActions(eventsReducer, ...actions);
  }
})
