const settings = {
    logLevel: process.env.LOG_LEVEL || 'info',
    connectionString: process.env.CONNECTION_STRING,
    dialect: process.env.DB_DIALECT
}

export default settings;
