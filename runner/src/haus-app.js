import {ALL} from "./web-app-options";
import {WebApp} from "./web-app";
import {Database} from "./database";

export class HausApp {
    constructor() {
        this.services = ALL.map(a => new WebApp(a));
        this.db = new Database();
    }
    
    start = () => {
        this.db.start();
        this.services.forEach(s => s.start());
    }
    
    waitToForAppToBeReady = async () => {
        const promises = this.services.map(s => s.wait());
        await Promise.all(promises);
    }
    
    stop = async () => {
        const promises = this.services.map(s => s.stop());
        await Promise.all(promises);
        this.db.stop();
    }
}