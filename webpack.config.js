const path = require('path');

module.exports = {
  entry: './Content/portPool',
  mode: 'development',
  devtool: 'inline-source-map',
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: [ '.tsx', '.ts', '.js' ],
  },
  output: {
    filename: 'index.js',
    path: path.resolve(__dirname, 'wwwroot/scripts/portPool'),
      libraryTarget: 'global',
      libraryExport: 'default',
    library: ["PortPool"],
  },
};