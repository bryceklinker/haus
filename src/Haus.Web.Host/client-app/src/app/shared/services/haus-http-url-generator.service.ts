import {Injectable} from "@angular/core";
import {DefaultHttpUrlGenerator, HttpResourceUrls, Pluralizer, normalizeRoot} from "@ngrx/data";

@Injectable()
export class HausHttpUrlGeneratorService extends DefaultHttpUrlGenerator {
  constructor(private entityPluralizer: Pluralizer) {
    super(entityPluralizer);
  }


  protected getResourceUrls(entityName: string, root: string): HttpResourceUrls {
    return this.knownHttpResourceUrls[entityName] || this.registerUrlForEntity(entityName, root);
  }

  private registerUrlForEntity(entityName: string, root: string): HttpResourceUrls {
    const normalizedRoot = normalizeRoot(root);
    const pluralName = this.entityPluralizer.pluralize(entityName);
    const url = `${normalizedRoot}/${pluralName}/`.toLowerCase();
    const resourceUrls: HttpResourceUrls = {
      entityResourceUrl: url,
      collectionResourceUrl: url
    };
    this.registerHttpResourceUrls({ [entityName]: resourceUrls})
    return resourceUrls;
  }
}
