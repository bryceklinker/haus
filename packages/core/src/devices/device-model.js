import {Model, DataTypes} from 'sequelize';

export class DeviceModel extends Model {
    static schema = {
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
}
