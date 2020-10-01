const path = require('path');
const make = require('./webpack.make');

module.exports = {
    ...make(),
    devServer: {
        contentBase: path.join(__dirname, 'dist'),
        compress: true,
        port: 8080,
        historyApiFallback: true
    }
}
