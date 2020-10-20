import settings from './settings';
import {Sequelize} from 'sequelize';
import {DeviceModel} from '../devices/device-model';
import logger from './logger';

let hasInitialized = false;
export function createDbFactory({connectionString} = settings) {
    return () => new Database(connectionString);
}

class Database {
    constructor(connectionString) {
        this.connectionString = connectionString;
        this.sequelize = new Sequelize(this.connectionString, {
            logging: (...args) => logger.info(...args)
        });
    }

    findAll = async (ModelType) => {
        await this.ensureInitialized();
        return await ModelType.findAll();
    };

    ensureInitialized = async () => {
        if (hasInitialized) {
            return;
        }

        hasInitialized = true;
        logger.info('Creating database with connection string', {connectionString: this.connectionString});
        DeviceModel.init(DeviceModel.schema, {
            sequelize: this.sequelize,
            modelName: DeviceModel.tableName
        });
        await this.sequelize.sync({alter: true});
        logger.info('Finished creating database');
    };
}
