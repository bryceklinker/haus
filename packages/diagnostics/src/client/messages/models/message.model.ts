export interface MessageModel {
    topic: string;
    payload: any;
}

export function createMessageModel({topic, payload}: MessageModel) {
    return {topic, payload};
}
