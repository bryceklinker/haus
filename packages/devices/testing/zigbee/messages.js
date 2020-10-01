import {ZIGBEE_LOG_TYPES, ZIGBEE_TOPICS} from '../../src/zigbee/topics';
import {v4 as uuid} from 'uuid';

export const zigbeeMessageFactory = {
    log: {
        deviceConnected: ({friendly_name = uuid()} = {}) => ({
            topic: ZIGBEE_TOPICS.LOG,
            payload: {
                type: ZIGBEE_LOG_TYPES.DEVICE_CONNECTED,
                message: {friendly_name}
            }
        }),
        pairing: {
            interviewStarted: ({friendly_name = uuid(), ...rest} = {}) => ({
                topic: ZIGBEE_TOPICS.LOG,
                payload: {
                    type: ZIGBEE_LOG_TYPES.PAIRING,
                    message: 'interview_started',
                    meta: {friendly_name, ...rest}
                }
            }),
            interviewSuccessful: ({friendly_name = uuid(), ...rest} = {}) => ({
                topic: ZIGBEE_TOPICS.LOG,
                payload: {
                    type: ZIGBEE_LOG_TYPES.PAIRING,
                    message: 'interview_successful',
                    meta: {friendly_name, ...rest}
                }
            })
        }
    }
};
