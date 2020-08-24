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

        this.webApps.forEach(s => s.startPublished());
    }
    
    start = async () => {
        this.containerApps.forEach(s => s.start());
        const promises = this.containerApps.map(c => c.wait());
        await Promise.all(promises);
        
        this.webApps.forEach(s => s.start());
    }
    
    waitToForAppToBeReady = async () => {
        const promises = this.webApps.map(s => s.wait());
        await Promise.all(promises);
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