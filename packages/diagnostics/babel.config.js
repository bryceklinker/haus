module.exports = {
    presets: [
        '@babel/preset-react',
        [
            '@babel/preset-env',
            {
                useBuiltIns: 'usage',
                targets: '> 0.25%, not dead',
                corejs: 3
            }
        ]
    ],
    plugins: [
        '@babel/plugin-syntax-class-properties'
    ]
}
