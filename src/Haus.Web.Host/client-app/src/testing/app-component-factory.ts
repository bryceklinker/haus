import {Type} from "@angular/core";
import {createSpectatorOptions} from "./create-spectator-options";
import {SHELL_COMPONENTS} from "../app/shell/components";
import {createComponentFactory} from "@ngneat/spectator";

export function appComponentFactory<T>(component: Type<T>) {
  const opts = createSpectatorOptions({
    declarations: SHELL_COMPONENTS,
    component
  });

  return createComponentFactory(opts);
}
