import {ALL_WEB_APPS} from "./web-app-options";
import {WebApp} from "./web-app";
import {ALL_CONTAINER_APPS} from "./container-app-options";
import {ContainerApp} from "./container-app";

export class HausApp {
    constructor(repositoryRoot) {
        this.repositoryRoot = repositoryRoot;
        this.webApps = ALL_WEB_APPS.map(a => new WebApp(this.repositoryRoot, a));
        this.containerApps = ALL_CONTAINER_APPS.map(a => new ContainerApp(a));
    }
    
    publish = () => {
        this.webApps.forEach(w => w.publish());
    }
    
    startPublishedApp = async () => {
        this.containerApps.forEach(s => s.start());
        const promises = this.containerApps.map(c => c.wait());
        await Promise.all(promises);

        for (let i = 0; i < this.webApps.length; i++) {
            await this.webApps[i].startPublished();
            await this.webApps[i].wait();
        }
    }
    
    start = async () => {
        this.containerApps.forEach(s => s.start());
        const promises = this.containerApps.map(c => c.wait());
        await Promise.all(promises);

        for (let i = 0; i < this.webApps.length; i++) {
            await this.webApps[i].start();
            await this.webApps[i].wait();
        }
    }
    
    stop = async () => {
        const promises = this.webApps.map(s => s.stop());
        await Promise.all(promises);
        this.containerApps.forEach(c => c.stop());
    }
    
    clean = async () => {
        const promises = this.webApps.map(w => w.clean());
        await Promise.all(promises);
    }
}