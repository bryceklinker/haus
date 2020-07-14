export class InMemoryStorage {
    constructor() {
        this.map = new Map();
    }

    getItem = (key) => {
        return this.map.get(key) || null;
    }

    setItem = (key, value) => {
        this.map.set(key, value);
    }
    
    clear = () => {
        this.map.clear();
    }
}