const path = require('path');

module.exports = {
    entry: {
        users: path.resolve(path.join(__dirname, 'users', 'index.js'))
    },
    output: {
        filename: '[name].js',
        path: path.resolve(path.join(__dirname, 'dist')),
        sourceMapFilename: '[file].map'
    },
    module: {
        rules: [
            {
                test: /\.jsx?$/i,
                use: [
                    {
                        loader: 'babel-loader'
                    }
                ]
            },
            {
                test: /\s[ac]ss.$/i,
                use: [
                    'style-loader',
                    'css-loader',
                    'sass-loader'
                ]
            },
            {
                test: /\.(png|jpg|gif|svg)$/i,
                use: [
                    {
                        loader: 'url-loader',
                        options: {
                            limit: 8192
                        }
                    }
                ]
            },
            {
                test: /\.(png|jpg|gif|svg)$/i,
                use: [
                    {
                        loader: 'file-loader'
                    }
                ]
            }
        ]
    },
    devServer: {
        port: 3000,
        contentBase: path.resolve(path.join(__dirname, 'dist')),
        historyApiFallback: true
    }
}