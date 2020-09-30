const path = require("path");
const TerserPlugin = require('terser-webpack-plugin');

module.exports = {
  optimization: {
    minimize: true,
    minimizer: [new TerserPlugin()],
  },
  performance: {
    hints: 'warning'
  },
  context: path.resolve(__dirname, "src"),
  entry: {
    main: "./js/app.js",
    registration: "./js/registration.js",
    auth: "./js/authenticate.js",
    requesthelp: "./js/requesthelp/requesthelp.js",
    feedback: "./js/feedback/feedback.js",
    profile: "./js/profile/profile.js",
    coveragemap: "./js/coveragemap.js",
    stickynav: "./js/stickynav.js",
    community: "./js/community/community.js",
    testimonials: "./js/testimonialSliders.js" 
    helpertabs: "./js/helperTabs.js"
  },
  output: {
    path: path.resolve(__dirname, "wwwroot"),
  },
  module: {
    rules: [
      {
        test: /\.s[ac]ss$/i,
        use: [
          // Creates `style` nodes from JS strings
          "style-loader",
          // Translates CSS into CommonJS
          "css-loader",
          // Compiles Sass to CSS
          "sass-loader",
        ],
      },
      {
        test: /\.js$/, //using regex to tell babel exactly what files to transcompile
        exclude: /node_modules/, // files to be ignored
        use: {
          loader: "babel-loader", // specify the loader
          options: { plugins: ["@babel/plugin-proposal-class-properties"] },
        },
      },
      {
        test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
        use: {
          loader: "file-loader",
            options: {
              name: '[name].[ext]',
              outputPath: 'fonts/'
            },
        },
      },
    ],
  },
};
