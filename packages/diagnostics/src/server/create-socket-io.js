import socketIo from 'socket.io';

export function createSocketIo(server) {
    const io = socketIo(server);
    return io;
}
