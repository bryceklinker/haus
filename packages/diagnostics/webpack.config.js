const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const {CleanWebpackPlugin} = require('clean-webpack-plugin');

module.exports = function (env, options) {
    const isProd = options.mode === 'production';
    return {
        mode: options.mode,
        devtool: isProd ? 'cheap-module-source-map' : 'inline-source-map',
        entry: {
            main: path.resolve(__dirname, 'src', 'client', 'main.js'),
        },
        output: {
            path: path.resolve(__dirname, 'build'),
            filename: isProd ? '[name].[chunkhash].js' : '[name].js',
            sourceMapFilename: '[file].map'
        },
        module: {
            rules: [
                {
                    test: /\.jsx?$/,
                    exclude: /node_modules/,
                    use: [
                        {loader: 'babel-loader'}
                    ]
                },
                {
                    test: /\.(css|scss)$/,
                    use: [
                        {loader: 'style-loader'},
                        {loader: 'css-loader'},
                        {loader: 'postcss-loader'},
                        {loader: 'sass-loader'}
                    ]
                },
                {
                    test: /\.(png|jpe?g|gif)$/i,
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
                    test: /\.(png|jpe?g|gif)$/i,
                    use: [
                        {loader: 'file-loader'}
                    ]
                },
                {
                    test: /\.svg$/,
                    use: [
                        {loader: 'svg-inline-loader'}
                    ]
                }
            ]
        },
        plugins: [
            new CleanWebpackPlugin(),
            new HtmlWebpackPlugin({
                template: path.resolve(__dirname, 'src', 'client', 'index.html'),
                filename: 'index.html',
                inject: 'body'
            })
        ],
        devServer: {
            contentBase: path.resolve(__dirname, 'build'),
            compress: true,
            port: 8080,
            historyApiFallback: true,
            hot: true
        }
    }
}
