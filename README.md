# front-end
Website to be hosted at helpmystreet.org

## HTML Build

The frontend uses Webpack to build static sources in to minified version and places them in the `wwwroot` folder of the MVC application

`npm run build` will build the sources

`npm run watch` will watch the sources and rebuild on changes.

## ToDo:

- [ ] Run `npm run build` when building the main project for release
- [ ] Split CSS out of Webpack bundle and include as separate file
