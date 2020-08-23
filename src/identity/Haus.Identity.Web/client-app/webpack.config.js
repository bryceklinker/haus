const path = require('path');

const DIST_PATH = path.resolve(path.join(__dirname, 'dist'));
module.exports = {
    entry: {
        users: path.resolve(path.join(__dirname, 'users', 'index.js'))
    },
    output: {
        path: DIST_PATH,
        filename: '[name].js',
        sourceMapFilename: '[file].map'
    },
    resolve: {
      extensions: ['.js', '.css', '.scss']  
    },
    module: {
        rules: [
            {
                test: /\.jsx?$/i,
                use: [
                    'babel-loader'
                ]
            },
            {
                test: /\.(s[ac]ss)$/i,
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
                    'file-loader'
                ]
            }
        ]
    },
    devServer: {
        contentBase: DIST_PATH,
        disableHostCheck: true,
        publicPath: '/dist/',
        compress: true,
        port: process.env.PORT || 3000,
        hot: true,
        injectClient: false
    }
}