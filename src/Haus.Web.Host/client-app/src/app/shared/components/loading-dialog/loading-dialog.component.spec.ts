import {Subject} from "rxjs";
import {LoadingDialogHarness} from "./loading-dialog.harness";

describe('LoadingDialogComponent', () => {
  let closeSubject: Subject<void>;

  beforeEach(() => {
    closeSubject = new Subject<void>();
  });

  test('should show loading indicator and text', async () => {
    const harness = await LoadingDialogHarness.render({text: 'waiting'});

    expect(harness.container).toHaveTextContent('waiting');
    expect(harness.loadingElement).toBeInTheDocument();
  })

  test('should disable closing dialog', async () => {
    const harness = await LoadingDialogHarness.render({text: ''});

    expect(harness.isClosingDialogDisabled).toEqual(true);
  })
})
