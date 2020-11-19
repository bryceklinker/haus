import * as path from 'path';
import * as webpack from 'webpack';
import HtmlWebpackPlugin from 'html-webpack-plugin';
import {CleanWebpackPlugin} from 'clean-webpack-plugin';

export function make(env: {production: boolean}): webpack.Configuration {
    return {
        mode: env.production ? 'production' : 'development',
        devtool: env.production ? 'source-map' : 'eval',
        entry: {
            index: path.resolve(__dirname, '..', 'client', 'index.tsx')
        },
        output: {
            path: path.resolve(__dirname, '..', 'build', 'client'),
            filename: env.production ? '[name].[chunkhash].js' : '[name].js',
            sourceMapFilename: '[file].map'
        },
        resolve: {
            extensions: ['.tsx', '.ts', '.js', '.json', '.html', '.css', '.scss']
        },
        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    use: [
                        {loader: 'ts-loader'}
                    ]
                },
                {
                    test: /\.(scss|css)$/,
                    use: [
                        {loader: 'style-loader'},
                        {loader: 'css-loader'},
                        {loader: 'sass-loader'},
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
                        {loader: 'svg-url-loader'}
                    ]
                }
            ]
        },
        plugins: [
            new CleanWebpackPlugin(),
            new HtmlWebpackPlugin({
                template: path.resolve(__dirname, '..', 'client', 'index.html'),
                filename: 'index.html',
                inject: 'body'
            })
        ]
    }
}


export const devConfig = make({production: false});
export default make;
