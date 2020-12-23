import {ParamMap} from "@angular/router";

export class TestingParamMap implements ParamMap {
    get keys(): string[] {
        return Object.keys(this.set)
            .filter(key => this.set.hasOwnProperty(key));
    }

    constructor(private readonly set: { [name: string]: string }) {

    }

    get(name: string): string | null {
        return this.set[name];
    }

    getAll(name: string): string[] {
        return [this.set[name]];
    }

    has(name: string): boolean {
        return this.keys.includes(name);
    }
}
