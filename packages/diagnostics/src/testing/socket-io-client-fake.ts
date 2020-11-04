import {EventEmitter} from 'events';

export class SocketIoClientFake extends EventEmitter {
    triggerDisconnect() {
        this.emit('disconnect');
    }
    triggerConnect() {
        this.emit('connect');
    }

    triggerMessage(message: any) {
        this.emit('message', message);
    }
}
