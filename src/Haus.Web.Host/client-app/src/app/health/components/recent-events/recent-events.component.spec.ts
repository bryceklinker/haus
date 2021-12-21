import {RecentEventsHarness} from "./recent-events.harness";
import {ModelFactory} from "../../../../testing";

describe('RecentEventsComponent', () => {
  test('should show each event', async () => {
    const events = [
      ModelFactory.createHausEvent(),
      ModelFactory.createHausEvent(),
      ModelFactory.createHausEvent()
    ]
    const harness = await RecentEventsHarness.render({events});

    expect(harness.events).toHaveLength(3);
  })

  test('should show event data', async () => {
    const events = [
      ModelFactory.createHausEvent({
        payload: 'something',
        timestamp: 'time',
        type: 'what?'
      })
    ];

    const harness = await RecentEventsHarness.render({events});

    expect(harness.container).toHaveTextContent('something');
    expect(harness.container).toHaveTextContent('time');
    expect(harness.container).toHaveTextContent('what?');
  })
})
