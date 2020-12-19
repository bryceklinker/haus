import {HausHttpUrlGeneratorService} from "./haus-http-url-generator.service";
import {DefaultPluralizer} from "@ngrx/data";
import {ENTITY_NAMES} from "../../entity-metadata";

describe('HausHttpUrlGeneratorService', () => {
  let service: HausHttpUrlGeneratorService;

  beforeEach(() => {
    service = new HausHttpUrlGeneratorService(new DefaultPluralizer([]));
  })

  it('should pluralize entity resource url', () => {
    const entityUrl = service.entityResource(ENTITY_NAMES.Room, '');
    expect(entityUrl).toEqual('/rooms/');
  })

  it('should pluralize collection resource url', () => {
    const entityUrl = service.collectionResource(ENTITY_NAMES.Room, '');
    expect(entityUrl).toEqual('/rooms/');
  })
})
