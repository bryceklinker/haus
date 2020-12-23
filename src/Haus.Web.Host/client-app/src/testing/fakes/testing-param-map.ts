import {ParamMap} from "@angular/router";

export class TestingParamMap implements ParamMap {
    get keys(): string[] {
        return Object.keys(this.set)
            .filter(key => this.set.hasOwnProperty(key));
    }

    constructor(private readonly set: { [name: string]: Array<string> }) {

    }

    get(name: string): string | null {
        const values = this.getAll(name);
        const firstValue = values[0];
        return firstValue || null;
    }

    getAll(name: string): string[] {
        return this.set[name] || [];
    }

    has(name: string): boolean {
        return this.keys.includes(name);
    }
}
