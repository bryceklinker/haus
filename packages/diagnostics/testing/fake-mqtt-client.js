import EventEmitter from 'events';

export class FakeMqttClient extends EventEmitter {
    emitMessage(data) {
        this.emit('message', data);
    }
}
