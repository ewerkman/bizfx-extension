# Sitecore Commerce Business Tools SDK

## Setup

Before start developing make sure you have the Sitecore feed in your user's npm config.

Go to `C:\Users\[your user]\.npmrc` and add the following lines to the file:

`@speak:registry=https://sitecore.myget.org/F/sc-npm-packages/npm/`
`@sitecore:registry=https://sitecore.myget.org/F/sc-npm-packages/npm/`

If the file doesn't exists, run

```npm
npm config set @speak:registry=https://sitecore.myget.org/F/sc-npm-packages/npm/
npm config set @sitecore:registry=https://sitecore.myget.org/F/sc-npm-packages/npm/
```

Install the SPEAK and BizFx packages, run

**You can find the SPEAK tarball files at the root of the Sitecore.Commerce.[version].zip file.**

**When installing the `@sitecore/bizfx` package, make sure you are installing the version that shipped with `Sitecore Experience Commerce` that you are using.**

```npm
​​​​​​​npm install speak-ng-bcl-0.8.0.tgz
npm install speak-styling-0.9.0-r00078.tgz
npm install @sitecore/bizfx
```

Run `npm install`

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `-prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).