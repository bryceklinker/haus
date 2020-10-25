import {Model, DataTypes} from 'sequelize';

export class DeviceModel extends Model {
    static db_schema = {
        id: {
            type: DataTypes.BIGINT,
            primaryKey: true,
        },
        external_id: {
            type: DataTypes.STRING,
            allowNull: true
        },
        device_config: {
            type: DataTypes.JSON,
            allowNull: true
        }
    }

    static tableName = 'Devices';

    static async createFromNewDevice(payload) {
        await DeviceModel.create({
            external_id: payload.friendly_name,
            device_config: payload
        })
    }
}
