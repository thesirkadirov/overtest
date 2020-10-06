"use strict";

let webpack = require("webpack");
let path = require("path");
let miniCssExtractPlugin = require('mini-css-extract-plugin');
let copyPlugin = require('copy-webpack-plugin');

const assetsFolder = "./frontend_assets/";
const bundleFolder = "./wwwroot/bundle/";

module.exports = {
    mode: "production",
    entry: {
        webapp: path.resolve(assetsFolder, 'webapp.js')
    },
    output: {
        path: path.resolve(bundleFolder),
        filename: '[name].bundle.js'
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    {
                        loader: miniCssExtractPlugin.loader
                    },
                    'css-loader',
                ],
            },
            {
                test: /\.svg$/,
                use: [
                    {
                        loader: 'file-loader',
                        options: {
                            name: '[name].[ext]',
                            outputPath: 'assets/svg/'
                        }
                    }
                ]
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                use: [
                    {
                        loader: 'file-loader',
                        options: {
                            name: '[name].[ext]',
                            outputPath: 'assets/fonts/'
                        }
                    }
                ]
            }
        ]
    },
    plugins: [
        new miniCssExtractPlugin({
            filename: '[name].bundle.css',
            chunkFilename: '[id].bundle.css',
            ignoreOrder: false
        }),
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            "window.jQuery": "jquery",
        }),
        new copyPlugin({
            patterns: [
                {
                    from: path.resolve('node_modules', 'trumbowyg', 'dist'),
                    to: path.resolve(bundleFolder, 'external', 'trumbowyg')
                },
                {
                    from: path.resolve('node_modules', 'ace-builds', 'src-min-noconflict'),
                    to: path.resolve(bundleFolder, 'external', 'acejs')
                }
            ],
            options: {
                concurrency: 100,
            },
        })
    ]
};