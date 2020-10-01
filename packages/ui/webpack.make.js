const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = function make(env = 'dev') {
    const isProd = env === 'prod';
    return {
        entry: {
            main: './src/main.js'
        },
        output: {
            path: path.resolve(__dirname, 'dist'),
            filename: isProd ? '[name].[hash].js' : '[name].js',
            sourceMapFilename: '[file].map'
        },
        module: {
            rules: [
                {
                    test: /\.jsx?$/,
                    exclude: /node_modules/,
                    use: [
                        'babel-loader'
                    ]
                },
                {
                    test: /\.scss$/,
                    use: [
                        'style-loader',
                        'css-loader',
                        'sass-loader'
                    ]
                },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
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
                    test: /\.(png|jpg|jpeg|gif)$/,
                    use: [
                        'file-loader'
                    ]
                },
                {
                    test: /\.svg$/,
                    use: [
                        'svg-inline-loader'
                    ]
                }
            ]
        },
        plugins: [
            new HtmlWebpackPlugin({
                filename: 'index.html',
                template: path.resolve(__dirname, 'src', 'index.html'),
                inject: 'body'
            }),
            new CopyWebpackPlugin({
                patterns: [
                    {from: path.resolve(__dirname, 'src', 'assets'), to: path.resolve(__dirname, 'dist', 'assets')},
                    {from: path.resolve(__dirname, 'src', 'settings.json'), to: path.resolve(__dirname, 'dist')}
                ]
            })
        ]
    };
};
