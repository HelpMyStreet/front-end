# front-end

Website to be hosted at helpmystreet.org

https://vsrm.dev.azure.com/HelpMyStreet/_apis/public/Release/badge/6a0f70ae-5438-4fbe-a9f2-8a46ef2b14b2/4/13

## Configuration

`appsettings.json` has default local configurations. To override set the following environment variables:

| Env var                       | Description                                              |
| ----------------------------- | -------------------------------------------------------- |
| `Firebase__Credentials`       | JSON serialized version of the Firebase credentials file |
| `Services__Address__Location` | URL of the Address service                               |
| `Services__Address__Key`      | Auth key for the Address service                         |
| `Services__User__Location`    | URL of the User service                                  |
| `Services__User__Key`         | Auth key for the User service                            |

For development you will require:

- The Firebase Credential Key file json-serialized as a string, ask a maintainer
- The Service API Keys, ask a maintainer

These can be configured using the above enviroment variables or by overriding the default configuration using an `appsettings.Development.json` file. This file has been gitignored to prevent keys/secrets ending up in the repository.

## HTML Build

The frontend uses Webpack to build static sources in to minified version and places them in the `wwwroot` folder of the MVC application

`npm run build` will build the sources

`npm run watch` will watch the sources and rebuild on changes.

## ToDo:

- [ ] Run `npm run build` when building the main project for release
- [ ] Split CSS out of Webpack bundle and include as separate file
