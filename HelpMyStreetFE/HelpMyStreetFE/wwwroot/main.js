/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./js/app.js");
/******/ })
/************************************************************************/
/******/ ({

/***/ "../node_modules/css-loader/dist/cjs.js!../node_modules/sass-loader/dist/cjs.js!./sass/main.scss":
/*!*******************************************************************************************************!*\
  !*** ../node_modules/css-loader/dist/cjs.js!../node_modules/sass-loader/dist/cjs.js!./sass/main.scss ***!
  \*******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

eval("// Imports\nvar ___CSS_LOADER_API_IMPORT___ = __webpack_require__(/*! ../../node_modules/css-loader/dist/runtime/api.js */ \"../node_modules/css-loader/dist/runtime/api.js\");\nexports = ___CSS_LOADER_API_IMPORT___(false);\n// Module\nexports.push([module.i, \".container {\\n  max-width: 1248px;\\n  margin: auto; }\\n  .container .row {\\n    display: flex;\\n    flex-direction: row;\\n    flex-wrap: wrap; }\\n    .container .row .sm1 {\\n      flex-basis: 8.33333%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm2 {\\n      flex-basis: 16.66667%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm3 {\\n      flex-basis: 25%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm4 {\\n      flex-basis: 33.33333%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm5 {\\n      flex-basis: 41.66667%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm6 {\\n      flex-basis: 50%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm7 {\\n      flex-basis: 58.33333%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm8 {\\n      flex-basis: 66.66667%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm9 {\\n      flex-basis: 75%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm10 {\\n      flex-basis: 83.33333%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm11 {\\n      flex-basis: 91.66667%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row .sm12 {\\n      flex-basis: 100%;\\n      padding-right: 12px;\\n      padding-left: 12px; }\\n    .container .row.justify-center {\\n      justify-content: center; }\\n\\nheader {\\n  padding: 50px 12px;\\n  margin-bottom: 32px; }\\n  header > div {\\n    margin: 0 4px; }\\n  header .logo {\\n    margin-right: 68px; }\\n  header .btn {\\n    margin: 0 4px; }\\n  header .spacer {\\n    height: 59px;\\n    width: 1px;\\n    background-color: #ccc; }\\n\\nfooter {\\n  background-color: #fff;\\n  border-top: 1px solid #ddd;\\n  margin-top: 88px;\\n  padding-top: 28px; }\\n  footer p, footer .list > span {\\n    font-size: 16px;\\n    line-height: 24px;\\n    margin: 8px 0 32px; }\\n    footer p span, footer .list > span span {\\n      margin-right: 24px; }\\n    footer p a, footer .list > span a {\\n      color: #9D9D9D; }\\n  footer .partner-logos ul {\\n    list-style-type: none;\\n    padding: 0px;\\n    display: flex; }\\n    footer .partner-logos ul li {\\n      flex-grow: 1;\\n      display: inline;\\n      position: relative; }\\n      footer .partner-logos ul li img {\\n        height: 50px; }\\n\\n.white, .cta {\\n  color: white; }\\n\\n.green {\\n  color: #25AC10; }\\n\\n.dark-blue {\\n  color: #001489; }\\n\\n.orange {\\n  color: #FF4E00; }\\n\\n.black {\\n  color: black; }\\n\\n.blue {\\n  color: #1E91D6; }\\n\\n.bold {\\n  font-weight: bold; }\\n\\n.bg-green, .cta {\\n  background-color: #25AC10; }\\n\\n.bg-white {\\n  background-color: white; }\\n\\n.transparent {\\n  background-color: transparent; }\\n\\n.mb0 {\\n  margin-bottom: 0px; }\\n\\n.mb12 {\\n  margin-bottom: 12px; }\\n\\n.mb15 {\\n  margin-bottom: 15px; }\\n\\n.mb40 {\\n  margin-bottom: 40px; }\\n\\n.mb56 {\\n  margin-bottom: 56px; }\\n\\n.mt16 {\\n  margin-top: 16px; }\\n\\n.mt68 {\\n  margin-top: 56px; }\\n\\n.mt72 {\\n  margin-top: 72px; }\\n\\n.ml32 {\\n  margin-left: 32px; }\\n\\n.ma0 {\\n  margin: 0; }\\n\\n.pt32 {\\n  padding-top: 32px !important; }\\n\\n.input label {\\n  position: absolute;\\n  top: -27px; }\\n\\n.input input {\\n  width: 240px;\\n  border: 1px solid lightgrey;\\n  padding: 11px 10px 10px;\\n  border-radius: 5px;\\n  background-color: #fff; }\\n\\n.input a.input__link {\\n  display: block;\\n  line-height: 22px;\\n  color: var(--p-color);\\n  position: absolute;\\n  left: 0;\\n  bottom: -22px; }\\n\\n.stepper {\\n  display: flex;\\n  width: 100%;\\n  align-items: stretch; }\\n  .stepper__step {\\n    text-align: center;\\n    flex-basis: 20%;\\n    opacity: 0.3; }\\n    .stepper__step--active {\\n      opacity: 1; }\\n    .stepper__step__number {\\n      margin: 8px auto;\\n      display: block;\\n      background-color: #001489;\\n      color: white;\\n      font-size: 32px;\\n      line-height: 39px;\\n      font-weight: bold;\\n      height: 64px;\\n      width: 64px;\\n      padding: 12.8px 0 12.2px;\\n      border-radius: 40px;\\n      z-index: 1; }\\n    .stepper__step__text {\\n      color: #001489;\\n      font-size: 18px;\\n      line-height: 22px;\\n      letter-spacing: 0.38px;\\n      font-weight: bold; }\\n  .stepper__link {\\n    content: \\\"\\\";\\n    border-top: 1px dashed #979797;\\n    height: 0px;\\n    z-index: 0;\\n    margin: 40px -40px;\\n    flex: 1 1 0px; }\\n\\nform a {\\n  color: #51535B; }\\n\\nform .input {\\n  width: 100%;\\n  margin: 64px 0 32px; }\\n  form .input label {\\n    font-weight: bold;\\n    font-size: 18px;\\n    line-height: 27px; }\\n  form .input .error {\\n    color: red;\\n    position: absolute;\\n    left: 0;\\n    bottom: -25px; }\\n  form .input__hint {\\n    font-weight: normal;\\n    color: #51535B; }\\n  form .input input {\\n    width: 100%; }\\n  form .input--checkbox {\\n    margin: 32px 0; }\\n    form .input--checkbox label {\\n      position: relative;\\n      margin: 0;\\n      padding-left: 40px;\\n      font-weight: normal;\\n      color: #51535B;\\n      top: 0; }\\n      form .input--checkbox label:hover {\\n        cursor: pointer; }\\n    form .input--checkbox input:checked ~ .input--checkbox__checkbox span {\\n      display: block; }\\n    form .input--checkbox__checkbox {\\n      background-color: #fff;\\n      border: 1px solid lightgrey;\\n      background-color: white;\\n      width: 25px;\\n      height: 25px;\\n      left: 0px;\\n      top: 0px;\\n      border-radius: 2px;\\n      position: absolute; }\\n      form .input--checkbox__checkbox span {\\n        display: none;\\n        text-align: center; }\\n\\nform .controls {\\n  text-align: right; }\\n\\n* {\\n  font-family: 'Montserrat', sans-serif;\\n  font-size: 18px;\\n  line-height: 27px;\\n  position: relative;\\n  box-sizing: border-box; }\\n\\nhtml, body {\\n  background-color: #F7F6F4;\\n  margin: 0; }\\n\\np, .list > span {\\n  font-size: 18px;\\n  line-height: 27px;\\n  color: #51535B; }\\n  p.subtitle, .list > span.subtitle {\\n    font-size: 24px;\\n    line-height: 36px; }\\n\\nh1, h2, h4, h6 {\\n  color: #001489;\\n  font-size: 56px;\\n  line-height: 72px;\\n  letter-spacing: 1px;\\n  font-weight: bold; }\\n\\nh2, h4, h6 {\\n  font-size: 44px;\\n  line-height: 56px;\\n  letter-spacing: 0.5px;\\n  margin: 0 0 22px; }\\n\\nh4, h6 {\\n  color: black;\\n  font-size: 32px;\\n  line-height: 44px;\\n  margin: 15px 0; }\\n\\nh6 {\\n  font-size: 18px;\\n  line-height: 27px; }\\n\\n.flex, .inline-flex {\\n  display: flex; }\\n  .flex.align-center, .align-center.inline-flex {\\n    align-items: center; }\\n\\n.inline-flex {\\n  display: inline-flex; }\\n\\n.dnone {\\n  display: none; }\\n\\n.logo {\\n  width: 280px; }\\n\\n.text-right {\\n  text-align: right; }\\n\\n.text-center {\\n  text-align: center; }\\n\\n.btn {\\n  border-radius: 24px;\\n  border: none;\\n  font-size: 18px;\\n  font-weight: bold;\\n  line-height: 22px;\\n  letter-spacing: 0.5px;\\n  padding: 13px 40px;\\n  text-decoration: none; }\\n  .btn.border-blue {\\n    border: 2px solid #1E91D6; }\\n  .btn.large {\\n    border-radius: 32px;\\n    line-height: 29px;\\n    padding: 17px 112px 18px; }\\n  .btn:hover {\\n    cursor: pointer; }\\n  .btn .loader {\\n    display: none; }\\n\\n.stats h2, .stats h4, .stats h6 {\\n  margin: 0; }\\n\\n.stats p, .stats .list > span {\\n  font-size: 24px;\\n  line-height: 29px;\\n  margin: 8px 0 4px; }\\n\\n.street-check {\\n  padding-left: 273px !important; }\\n  .street-check h3 {\\n    margin-top: 40px;\\n    font-size: 36px;\\n    line-height: 48px; }\\n  .street-check .progress {\\n    display: inline-block;\\n    width: 227px;\\n    height: 3px;\\n    background-color: #EEE;\\n    margin: 0 10px;\\n    border-radius: 5px; }\\n    .street-check .progress .indicator {\\n      border-radius: 5px;\\n      display: inline-block;\\n      width: 15%;\\n      height: 3px;\\n      background-color: #48A415; }\\n  .street-check input {\\n    width: 392px;\\n    font-size: 18px;\\n    line-height: 22px;\\n    padding: 20px 32px; }\\n  .street-check button {\\n    font-size: 24px;\\n    line-height: 29px;\\n    border-radius: 32px;\\n    padding: 17px 29px;\\n    margin-left: 24px; }\\n\\n.card {\\n  box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.1);\\n  border-radius: 5px;\\n  border: 1px solid #DDDDDD;\\n  background-color: white;\\n  padding: 24px; }\\n\\n.list-item {\\n  margin: 16px 0; }\\n\\n.fab {\\n  display: block;\\n  text-align: center;\\n  background-color: #001489;\\n  color: white;\\n  font-size: 40px;\\n  line-height: 49px;\\n  font-weight: bold;\\n  padding: 16px 0 15px;\\n  border-radius: 40px; }\\n\\n.fill {\\n  width: 100%; }\\n\\n.mdi-check {\\n  color: #1E91D6;\\n  font-weight: bold; }\\n\\n.list > span {\\n  display: block;\\n  margin: 16px 0; }\\n\", \"\"]);\n// Exports\nmodule.exports = exports;\n\n\n//# sourceURL=webpack:///./sass/main.scss?../node_modules/css-loader/dist/cjs.js!../node_modules/sass-loader/dist/cjs.js");

/***/ }),

/***/ "../node_modules/css-loader/dist/runtime/api.js":
/*!******************************************************!*\
  !*** ../node_modules/css-loader/dist/runtime/api.js ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
eval("\n\n/*\n  MIT License http://www.opensource.org/licenses/mit-license.php\n  Author Tobias Koppers @sokra\n*/\n// css base code, injected by the css-loader\n// eslint-disable-next-line func-names\nmodule.exports = function (useSourceMap) {\n  var list = []; // return the list of modules as css string\n\n  list.toString = function toString() {\n    return this.map(function (item) {\n      var content = cssWithMappingToString(item, useSourceMap);\n\n      if (item[2]) {\n        return \"@media \".concat(item[2], \" {\").concat(content, \"}\");\n      }\n\n      return content;\n    }).join('');\n  }; // import a list of modules into the list\n  // eslint-disable-next-line func-names\n\n\n  list.i = function (modules, mediaQuery, dedupe) {\n    if (typeof modules === 'string') {\n      // eslint-disable-next-line no-param-reassign\n      modules = [[null, modules, '']];\n    }\n\n    var alreadyImportedModules = {};\n\n    if (dedupe) {\n      for (var i = 0; i < this.length; i++) {\n        // eslint-disable-next-line prefer-destructuring\n        var id = this[i][0];\n\n        if (id != null) {\n          alreadyImportedModules[id] = true;\n        }\n      }\n    }\n\n    for (var _i = 0; _i < modules.length; _i++) {\n      var item = [].concat(modules[_i]);\n\n      if (dedupe && alreadyImportedModules[item[0]]) {\n        // eslint-disable-next-line no-continue\n        continue;\n      }\n\n      if (mediaQuery) {\n        if (!item[2]) {\n          item[2] = mediaQuery;\n        } else {\n          item[2] = \"\".concat(mediaQuery, \" and \").concat(item[2]);\n        }\n      }\n\n      list.push(item);\n    }\n  };\n\n  return list;\n};\n\nfunction cssWithMappingToString(item, useSourceMap) {\n  var content = item[1] || ''; // eslint-disable-next-line prefer-destructuring\n\n  var cssMapping = item[3];\n\n  if (!cssMapping) {\n    return content;\n  }\n\n  if (useSourceMap && typeof btoa === 'function') {\n    var sourceMapping = toComment(cssMapping);\n    var sourceURLs = cssMapping.sources.map(function (source) {\n      return \"/*# sourceURL=\".concat(cssMapping.sourceRoot || '').concat(source, \" */\");\n    });\n    return [content].concat(sourceURLs).concat([sourceMapping]).join('\\n');\n  }\n\n  return [content].join('\\n');\n} // Adapted from convert-source-map (MIT)\n\n\nfunction toComment(sourceMap) {\n  // eslint-disable-next-line no-undef\n  var base64 = btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap))));\n  var data = \"sourceMappingURL=data:application/json;charset=utf-8;base64,\".concat(base64);\n  return \"/*# \".concat(data, \" */\");\n}\n\n//# sourceURL=webpack:///../node_modules/css-loader/dist/runtime/api.js?");

/***/ }),

/***/ "../node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js":
/*!*****************************************************************************!*\
  !*** ../node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
eval("\n\nvar isOldIE = function isOldIE() {\n  var memo;\n  return function memorize() {\n    if (typeof memo === 'undefined') {\n      // Test for IE <= 9 as proposed by Browserhacks\n      // @see http://browserhacks.com/#hack-e71d8692f65334173fee715c222cb805\n      // Tests for existence of standard globals is to allow style-loader\n      // to operate correctly into non-standard environments\n      // @see https://github.com/webpack-contrib/style-loader/issues/177\n      memo = Boolean(window && document && document.all && !window.atob);\n    }\n\n    return memo;\n  };\n}();\n\nvar getTarget = function getTarget() {\n  var memo = {};\n  return function memorize(target) {\n    if (typeof memo[target] === 'undefined') {\n      var styleTarget = document.querySelector(target); // Special case to return head of iframe instead of iframe itself\n\n      if (window.HTMLIFrameElement && styleTarget instanceof window.HTMLIFrameElement) {\n        try {\n          // This will throw an exception if access to iframe is blocked\n          // due to cross-origin restrictions\n          styleTarget = styleTarget.contentDocument.head;\n        } catch (e) {\n          // istanbul ignore next\n          styleTarget = null;\n        }\n      }\n\n      memo[target] = styleTarget;\n    }\n\n    return memo[target];\n  };\n}();\n\nvar stylesInDom = [];\n\nfunction getIndexByIdentifier(identifier) {\n  var result = -1;\n\n  for (var i = 0; i < stylesInDom.length; i++) {\n    if (stylesInDom[i].identifier === identifier) {\n      result = i;\n      break;\n    }\n  }\n\n  return result;\n}\n\nfunction modulesToDom(list, options) {\n  var idCountMap = {};\n  var identifiers = [];\n\n  for (var i = 0; i < list.length; i++) {\n    var item = list[i];\n    var id = options.base ? item[0] + options.base : item[0];\n    var count = idCountMap[id] || 0;\n    var identifier = \"\".concat(id, \" \").concat(count);\n    idCountMap[id] = count + 1;\n    var index = getIndexByIdentifier(identifier);\n    var obj = {\n      css: item[1],\n      media: item[2],\n      sourceMap: item[3]\n    };\n\n    if (index !== -1) {\n      stylesInDom[index].references++;\n      stylesInDom[index].updater(obj);\n    } else {\n      stylesInDom.push({\n        identifier: identifier,\n        updater: addStyle(obj, options),\n        references: 1\n      });\n    }\n\n    identifiers.push(identifier);\n  }\n\n  return identifiers;\n}\n\nfunction insertStyleElement(options) {\n  var style = document.createElement('style');\n  var attributes = options.attributes || {};\n\n  if (typeof attributes.nonce === 'undefined') {\n    var nonce =  true ? __webpack_require__.nc : undefined;\n\n    if (nonce) {\n      attributes.nonce = nonce;\n    }\n  }\n\n  Object.keys(attributes).forEach(function (key) {\n    style.setAttribute(key, attributes[key]);\n  });\n\n  if (typeof options.insert === 'function') {\n    options.insert(style);\n  } else {\n    var target = getTarget(options.insert || 'head');\n\n    if (!target) {\n      throw new Error(\"Couldn't find a style target. This probably means that the value for the 'insert' parameter is invalid.\");\n    }\n\n    target.appendChild(style);\n  }\n\n  return style;\n}\n\nfunction removeStyleElement(style) {\n  // istanbul ignore if\n  if (style.parentNode === null) {\n    return false;\n  }\n\n  style.parentNode.removeChild(style);\n}\n/* istanbul ignore next  */\n\n\nvar replaceText = function replaceText() {\n  var textStore = [];\n  return function replace(index, replacement) {\n    textStore[index] = replacement;\n    return textStore.filter(Boolean).join('\\n');\n  };\n}();\n\nfunction applyToSingletonTag(style, index, remove, obj) {\n  var css = remove ? '' : obj.media ? \"@media \".concat(obj.media, \" {\").concat(obj.css, \"}\") : obj.css; // For old IE\n\n  /* istanbul ignore if  */\n\n  if (style.styleSheet) {\n    style.styleSheet.cssText = replaceText(index, css);\n  } else {\n    var cssNode = document.createTextNode(css);\n    var childNodes = style.childNodes;\n\n    if (childNodes[index]) {\n      style.removeChild(childNodes[index]);\n    }\n\n    if (childNodes.length) {\n      style.insertBefore(cssNode, childNodes[index]);\n    } else {\n      style.appendChild(cssNode);\n    }\n  }\n}\n\nfunction applyToTag(style, options, obj) {\n  var css = obj.css;\n  var media = obj.media;\n  var sourceMap = obj.sourceMap;\n\n  if (media) {\n    style.setAttribute('media', media);\n  } else {\n    style.removeAttribute('media');\n  }\n\n  if (sourceMap && btoa) {\n    css += \"\\n/*# sourceMappingURL=data:application/json;base64,\".concat(btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap)))), \" */\");\n  } // For old IE\n\n  /* istanbul ignore if  */\n\n\n  if (style.styleSheet) {\n    style.styleSheet.cssText = css;\n  } else {\n    while (style.firstChild) {\n      style.removeChild(style.firstChild);\n    }\n\n    style.appendChild(document.createTextNode(css));\n  }\n}\n\nvar singleton = null;\nvar singletonCounter = 0;\n\nfunction addStyle(obj, options) {\n  var style;\n  var update;\n  var remove;\n\n  if (options.singleton) {\n    var styleIndex = singletonCounter++;\n    style = singleton || (singleton = insertStyleElement(options));\n    update = applyToSingletonTag.bind(null, style, styleIndex, false);\n    remove = applyToSingletonTag.bind(null, style, styleIndex, true);\n  } else {\n    style = insertStyleElement(options);\n    update = applyToTag.bind(null, style, options);\n\n    remove = function remove() {\n      removeStyleElement(style);\n    };\n  }\n\n  update(obj);\n  return function updateStyle(newObj) {\n    if (newObj) {\n      if (newObj.css === obj.css && newObj.media === obj.media && newObj.sourceMap === obj.sourceMap) {\n        return;\n      }\n\n      update(obj = newObj);\n    } else {\n      remove();\n    }\n  };\n}\n\nmodule.exports = function (list, options) {\n  options = options || {}; // Force single-tag solution on IE6-9, which has a hard limit on the # of <style>\n  // tags it will allow on a page\n\n  if (!options.singleton && typeof options.singleton !== 'boolean') {\n    options.singleton = isOldIE();\n  }\n\n  list = list || [];\n  var lastIdentifiers = modulesToDom(list, options);\n  return function update(newList) {\n    newList = newList || [];\n\n    if (Object.prototype.toString.call(newList) !== '[object Array]') {\n      return;\n    }\n\n    for (var i = 0; i < lastIdentifiers.length; i++) {\n      var identifier = lastIdentifiers[i];\n      var index = getIndexByIdentifier(identifier);\n      stylesInDom[index].references--;\n    }\n\n    var newLastIdentifiers = modulesToDom(newList, options);\n\n    for (var _i = 0; _i < lastIdentifiers.length; _i++) {\n      var _identifier = lastIdentifiers[_i];\n\n      var _index = getIndexByIdentifier(_identifier);\n\n      if (stylesInDom[_index].references === 0) {\n        stylesInDom[_index].updater();\n\n        stylesInDom.splice(_index, 1);\n      }\n    }\n\n    lastIdentifiers = newLastIdentifiers;\n  };\n};\n\n//# sourceURL=webpack:///../node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js?");

/***/ }),

/***/ "./js/app.js":
/*!*******************!*\
  !*** ./js/app.js ***!
  \*******************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _sass_main_scss__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../sass/main.scss */ \"./sass/main.scss\");\n/* harmony import */ var _sass_main_scss__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(_sass_main_scss__WEBPACK_IMPORTED_MODULE_0__);\n﻿\r\n\r\n$(function () {\r\n    $(\"#postcode_button\").click(function (evt) {\r\n\r\n        const postCode = $(\"#postcode\").val();\r\n\r\n        if (postCode) {\r\n            $(this).width($(this).width());\r\n            $(this).height($(this).height());\r\n\r\n            $('#postcode_notcovered').hide();\r\n            $('#postcode_covered').hide();\r\n            $('#postcode_error').hide();\r\n            $(this).find('.text').hide();\r\n            $(this).find('.loader').show();\r\n\r\n            fetch(`/api/postcode/${postCode}`)\r\n                .then(resp => resp.json())\r\n                .then(data => {\r\n                    if (data.addresses && data.addresses.length) $('#postcode_covered').show();\r\n                    else $('#postcode_notcovered').show();\r\n                })\r\n                .catch(err => {\r\n                    $('#postcode_error').show();\r\n                })\r\n                .finally(() => {\r\n                    $(this).width(null);\r\n                    $(this).height(null);\r\n                    $(this).find('.text').show();\r\n                    $(this).find('.loader').hide();\r\n                });\r\n        }\r\n    });\r\n});\n\n//# sourceURL=webpack:///./js/app.js?");

/***/ }),

/***/ "./sass/main.scss":
/*!************************!*\
  !*** ./sass/main.scss ***!
  \************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

eval("var api = __webpack_require__(/*! ../../node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js */ \"../node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js\");\n            var content = __webpack_require__(/*! !../../node_modules/css-loader/dist/cjs.js!../../node_modules/sass-loader/dist/cjs.js!./main.scss */ \"../node_modules/css-loader/dist/cjs.js!../node_modules/sass-loader/dist/cjs.js!./sass/main.scss\");\n\n            content = content.__esModule ? content.default : content;\n\n            if (typeof content === 'string') {\n              content = [[module.i, content, '']];\n            }\n\nvar options = {};\n\noptions.insert = \"head\";\noptions.singleton = false;\n\nvar update = api(content, options);\n\nvar exported = content.locals ? content.locals : {};\n\n\n\nmodule.exports = exported;\n\n//# sourceURL=webpack:///./sass/main.scss?");

/***/ })

/******/ });
