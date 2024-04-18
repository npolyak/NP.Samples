const path = require('path');

module.exports = {
    mode: 'development',
    entry: './wwwroot/dist/client.js',
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'),
        filename: 'main.js',
    },
};