self.PortPool =
/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./Content/portPool/index.ts":
/*!***********************************!*\
  !*** ./Content/portPool/index.ts ***!
  \***********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => __WEBPACK_DEFAULT_EXPORT__
/* harmony export */ });
/* harmony import */ var _portPool__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./portPool */ "./Content/portPool/portPool.ts");

/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (_portPool__WEBPACK_IMPORTED_MODULE_0__.default);


/***/ }),

/***/ "./Content/portPool/portPool.ts":
/*!**************************************!*\
  !*** ./Content/portPool/portPool.ts ***!
  \**************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => __WEBPACK_DEFAULT_EXPORT__
/* harmony export */ });
var PortPool = /** @class */ (function () {
    function PortPool(serviceUrl) {
        this.template = "<div";
        this.serviceUrl = serviceUrl;
        this.doPoll();
    }
    PortPool.prototype.processConfigInput = function () {
    };
    PortPool.prototype.websProcessKey = function (id) {
    };
    PortPool.prototype.render = function (data) {
        if (data.displayText) {
            var renderText = $('#displayText [data-render=displayText]');
            data.displayText.forEach(function (t, ix) {
                $(renderText[ix]).text(t.text);
            });
        }
        if (data.states) {
            var current = $('#statusKeys .WEBS_KEY');
            var enabled = data.states.split(",").reduce(function (o, s) {
                o[s.trim()] = true;
                return o;
            }, {});
            current.each(function () {
                $(this).toggleClass('WEBS_ON', enabled[$(this).attr('id')] == true);
            });
        }
        if (data.statusValues) {
            var container = $('#statusValues');
            var current = container.find('.statusValue');
            var values = data.statusValues.reduce(function (o, s) {
                o[s.name] = s;
                return o;
            }, {});
            current.each(function () {
                var that = $(this);
                var id = that.attr('id');
                var value = values[id];
                if (!value) {
                    that.hide();
                }
                else {
                    that.find('[data-render=state-text]').text(value.text);
                    that.find('[data-render=state-value]').text(value.value);
                    that.find('[data-render=state-unit]').text(value.unit);
                    that.show();
                    delete values[id];
                }
            });
            Object.values(values).forEach(function (v) {
                container.append("<div id=\"" + v.name + "\" class=\"statusValue\"><div data-render=\"state-text\">" + v.text + "</div><div data-render=\"state-value\">" + v.value + "</div><div data-render=\"state-unit\">" + v.unit + "</div></div>");
            });
        }
    };
    PortPool.prototype.doPoll = function () {
        var _this = this;
        $.get(this.serviceUrl, function (data) {
            _this.render(data);
            setTimeout(function () { return _this.doPoll(); }, 1000);
        });
    };
    return PortPool;
}());
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (PortPool);


/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		if(__webpack_module_cache__[moduleId]) {
/******/ 			return __webpack_module_cache__[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => Object.prototype.hasOwnProperty.call(obj, prop)
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	// module exports must be returned from runtime so entry inlining is disabled
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__("./Content/portPool/index.ts");
/******/ })()
.default;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9Qb3J0UG9vbC8uL0NvbnRlbnQvcG9ydFBvb2wvaW5kZXgudHMiLCJ3ZWJwYWNrOi8vUG9ydFBvb2wvLi9Db250ZW50L3BvcnRQb29sL3BvcnRQb29sLnRzIiwid2VicGFjazovL1BvcnRQb29sL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL1BvcnRQb29sL3dlYnBhY2svcnVudGltZS9kZWZpbmUgcHJvcGVydHkgZ2V0dGVycyIsIndlYnBhY2s6Ly9Qb3J0UG9vbC93ZWJwYWNrL3J1bnRpbWUvaGFzT3duUHJvcGVydHkgc2hvcnRoYW5kIiwid2VicGFjazovL1BvcnRQb29sL3dlYnBhY2svcnVudGltZS9tYWtlIG5hbWVzcGFjZSBvYmplY3QiLCJ3ZWJwYWNrOi8vUG9ydFBvb2wvd2VicGFjay9zdGFydHVwIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7Ozs7Ozs7QUFBaUM7QUFDakMsaUVBQWUsOENBQVE7Ozs7Ozs7Ozs7Ozs7OztBQ0d2QjtJQUVJLGtCQUFZLFVBQWlCO1FBeUQ3QixhQUFRLEdBQVUsTUFBTTtRQXhEcEIsSUFBSSxDQUFDLFVBQVUsR0FBRyxVQUFVLENBQUM7UUFDN0IsSUFBSSxDQUFDLE1BQU0sRUFBRSxDQUFDO0lBQ2xCLENBQUM7SUFFRCxxQ0FBa0IsR0FBbEI7SUFDQSxDQUFDO0lBRUQsaUNBQWMsR0FBZCxVQUFlLEVBQVU7SUFDekIsQ0FBQztJQUVELHlCQUFNLEdBQU4sVUFBTyxJQUFnQjtRQUNuQixJQUFJLElBQUksQ0FBQyxXQUFXLEVBQUU7WUFDbEIsSUFBSSxVQUFVLEdBQUcsQ0FBQyxDQUFDLHdDQUF3QyxDQUFDLENBQUM7WUFDN0QsSUFBSSxDQUFDLFdBQVcsQ0FBQyxPQUFPLENBQUMsVUFBQyxDQUFhLEVBQUUsRUFBVTtnQkFDL0MsQ0FBQyxDQUFDLFVBQVUsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUM7WUFDbkMsQ0FBQyxDQUFDLENBQUM7U0FFTjtRQUNELElBQUksSUFBSSxDQUFDLE1BQU0sRUFBRTtZQUNiLElBQUksT0FBTyxHQUFHLENBQUMsQ0FBQyx1QkFBdUIsQ0FBQyxDQUFDO1lBQ3pDLElBQUksT0FBTyxHQUFHLElBQUksQ0FBQyxNQUFNLENBQUMsS0FBSyxDQUFDLEdBQUcsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxVQUFDLENBQUMsRUFBRSxDQUFDO2dCQUM3QyxDQUFDLENBQUMsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDLEdBQUcsSUFBSSxDQUFDO2dCQUNuQixPQUFPLENBQUMsQ0FBQztZQUNiLENBQUMsRUFBRSxFQUFFLENBQUMsQ0FBQztZQUNQLE9BQU8sQ0FBQyxJQUFJLENBQUM7Z0JBQ1QsQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLFdBQVcsQ0FBQyxTQUFTLEVBQUUsT0FBTyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLENBQUMsSUFBSSxJQUFJLENBQUMsQ0FBQztZQUN4RSxDQUFDLENBQUMsQ0FBQztTQUNOO1FBQ0QsSUFBSSxJQUFJLENBQUMsWUFBWSxFQUFFO1lBQ25CLElBQUksU0FBUyxHQUFHLENBQUMsQ0FBQyxlQUFlLENBQUMsQ0FBQztZQUNuQyxJQUFJLE9BQU8sR0FBRyxTQUFTLENBQUMsSUFBSSxDQUFDLGNBQWMsQ0FBQyxDQUFDO1lBQzdDLElBQUksTUFBTSxHQUFHLElBQUksQ0FBQyxZQUFZLENBQUMsTUFBTSxDQUFDLFVBQUMsQ0FBQyxFQUFFLENBQUM7Z0JBQ3ZDLENBQUMsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFDO2dCQUNkLE9BQU8sQ0FBQyxDQUFDO1lBQ2IsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxDQUFDO1lBQ1AsT0FBTyxDQUFDLElBQUksQ0FBQztnQkFDVCxJQUFJLElBQUksR0FBRyxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUM7Z0JBQ25CLElBQUksRUFBRSxHQUFHLElBQUksQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLENBQUM7Z0JBQ3pCLElBQUksS0FBSyxHQUFjLE1BQU0sQ0FBQyxFQUFFLENBQUMsQ0FBQztnQkFDbEMsSUFBSSxDQUFDLEtBQUssRUFBRTtvQkFDUixJQUFJLENBQUMsSUFBSSxFQUFFLENBQUM7aUJBQ2Y7cUJBQU07b0JBQ0gsSUFBSSxDQUFDLElBQUksQ0FBQywwQkFBMEIsQ0FBQyxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUM7b0JBQ3ZELElBQUksQ0FBQyxJQUFJLENBQUMsMkJBQTJCLENBQUMsQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLEtBQUssQ0FBQyxDQUFDO29CQUN6RCxJQUFJLENBQUMsSUFBSSxDQUFDLDBCQUEwQixDQUFDLENBQUMsSUFBSSxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQztvQkFDdkQsSUFBSSxDQUFDLElBQUksRUFBRSxDQUFDO29CQUNaLE9BQU8sTUFBTSxDQUFDLEVBQUUsQ0FBQyxDQUFDO2lCQUNyQjtZQUNMLENBQUMsQ0FBQyxDQUFDO1lBQ0gsTUFBTSxDQUFDLE1BQU0sQ0FBQyxNQUFNLENBQUMsQ0FBQyxPQUFPLENBQUMsVUFBQyxDQUFXO2dCQUN0QyxTQUFTLENBQUMsTUFBTSxDQUNaLGVBQVksQ0FBQyxDQUFDLElBQUksaUVBQXVELENBQUMsQ0FBQyxJQUFJLCtDQUF3QyxDQUFDLENBQUMsS0FBSyw4Q0FBdUMsQ0FBQyxDQUFDLElBQUksaUJBQWMsQ0FBQyxDQUFDO1lBQ25NLENBQUMsQ0FBQyxDQUFDO1NBQ047SUFDTCxDQUFDO0lBSUQseUJBQU0sR0FBTjtRQUFBLGlCQUtDO1FBSkcsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsVUFBVSxFQUFFLFVBQUMsSUFBZ0I7WUFDcEMsS0FBSSxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMsQ0FBQztZQUNsQixVQUFVLENBQUMsY0FBTSxZQUFJLENBQUMsTUFBTSxFQUFFLEVBQWIsQ0FBYSxFQUFFLElBQUksQ0FBQyxDQUFDO1FBQzFDLENBQUMsQ0FBQyxDQUFDO0lBQ1AsQ0FBQztJQUNMLGVBQUM7QUFBRCxDQUFDOzs7Ozs7OztVQ3ZFRDtVQUNBOztVQUVBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTs7VUFFQTtVQUNBOztVQUVBO1VBQ0E7VUFDQTs7Ozs7V0NyQkE7V0FDQTtXQUNBO1dBQ0E7V0FDQSx3Q0FBd0MseUNBQXlDO1dBQ2pGO1dBQ0E7V0FDQSxFOzs7OztXQ1BBLHNGOzs7OztXQ0FBO1dBQ0E7V0FDQTtXQUNBLHNEQUFzRCxrQkFBa0I7V0FDeEU7V0FDQSwrQ0FBK0MsY0FBYztXQUM3RCxFOzs7O1VDTkE7VUFDQTtVQUNBO1VBQ0EiLCJmaWxlIjoiaW5kZXguanMiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgUG9ydFBvb2wgZnJvbSAnLi9wb3J0UG9vbCdcclxuZXhwb3J0IGRlZmF1bHQgUG9ydFBvb2wiLCJcclxuaW1wb3J0IHsgUG9vbFN0YXR1cywgU3RhdHVzTGluZSB9IGZyb20gJy4vcG9vbFN0YXR1cydcclxuaW1wb3J0IHsgUG9vbFZhbHVlIH0gZnJvbSAnLi9wb29sVmFsdWUnXHJcbiBcclxuZXhwb3J0IGRlZmF1bHQgY2xhc3MgUG9ydFBvb2wgeyAgIFxyXG4gICAgc2VydmljZVVybDpzdHJpbmc7XHJcbiAgICBjb25zdHJ1Y3RvcihzZXJ2aWNlVXJsOnN0cmluZykge1xyXG4gICAgICAgIHRoaXMuc2VydmljZVVybCA9IHNlcnZpY2VVcmw7IFxyXG4gICAgICAgIHRoaXMuZG9Qb2xsKCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJvY2Vzc0NvbmZpZ0lucHV0KCkgeyBcclxuICAgIH1cclxuXHJcbiAgICB3ZWJzUHJvY2Vzc0tleShpZDogc3RyaW5nKSB7XHJcbiAgICB9ICAgXHJcbiAgICAgXHJcbiAgICByZW5kZXIoZGF0YTogUG9vbFN0YXR1cyk6IHZvaWQge1xyXG4gICAgICAgIGlmIChkYXRhLmRpc3BsYXlUZXh0KSB7XHJcbiAgICAgICAgICAgIHZhciByZW5kZXJUZXh0ID0gJCgnI2Rpc3BsYXlUZXh0IFtkYXRhLXJlbmRlcj1kaXNwbGF5VGV4dF0nKTtcclxuICAgICAgICAgICAgZGF0YS5kaXNwbGF5VGV4dC5mb3JFYWNoKCh0OiBTdGF0dXNMaW5lLCBpeDogbnVtYmVyKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAkKHJlbmRlclRleHRbaXhdKS50ZXh0KHQudGV4dCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICBcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKGRhdGEuc3RhdGVzKSB7XHJcbiAgICAgICAgICAgIHZhciBjdXJyZW50ID0gJCgnI3N0YXR1c0tleXMgLldFQlNfS0VZJyk7XHJcbiAgICAgICAgICAgIHZhciBlbmFibGVkID0gZGF0YS5zdGF0ZXMuc3BsaXQoXCIsXCIpLnJlZHVjZSgobywgcykgPT4ge1xyXG4gICAgICAgICAgICAgICAgb1tzLnRyaW0oKV0gPSB0cnVlO1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIG87XHJcbiAgICAgICAgICAgIH0sIHt9KTtcclxuICAgICAgICAgICAgY3VycmVudC5lYWNoKGZ1bmN0aW9uKCkge1xyXG4gICAgICAgICAgICAgICAgJCh0aGlzKS50b2dnbGVDbGFzcygnV0VCU19PTicsIGVuYWJsZWRbJCh0aGlzKS5hdHRyKCdpZCcpXSA9PSB0cnVlKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmIChkYXRhLnN0YXR1c1ZhbHVlcykge1xyXG4gICAgICAgICAgICB2YXIgY29udGFpbmVyID0gJCgnI3N0YXR1c1ZhbHVlcycpO1xyXG4gICAgICAgICAgICB2YXIgY3VycmVudCA9IGNvbnRhaW5lci5maW5kKCcuc3RhdHVzVmFsdWUnKTtcclxuICAgICAgICAgICAgdmFyIHZhbHVlcyA9IGRhdGEuc3RhdHVzVmFsdWVzLnJlZHVjZSgobywgcykgPT4ge1xyXG4gICAgICAgICAgICAgICAgb1tzLm5hbWVdID0gcztcclxuICAgICAgICAgICAgICAgIHJldHVybiBvO1xyXG4gICAgICAgICAgICB9LCB7fSk7XHJcbiAgICAgICAgICAgIGN1cnJlbnQuZWFjaChmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICB2YXIgdGhhdCA9ICQodGhpcyk7XHJcbiAgICAgICAgICAgICAgICB2YXIgaWQgPSB0aGF0LmF0dHIoJ2lkJyk7XHJcbiAgICAgICAgICAgICAgICB2YXIgdmFsdWUgPSA8UG9vbFZhbHVlPnZhbHVlc1tpZF07XHJcbiAgICAgICAgICAgICAgICBpZiAoIXZhbHVlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhhdC5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoYXQuZmluZCgnW2RhdGEtcmVuZGVyPXN0YXRlLXRleHRdJykudGV4dCh2YWx1ZS50ZXh0KTtcclxuICAgICAgICAgICAgICAgICAgICB0aGF0LmZpbmQoJ1tkYXRhLXJlbmRlcj1zdGF0ZS12YWx1ZV0nKS50ZXh0KHZhbHVlLnZhbHVlKTtcclxuICAgICAgICAgICAgICAgICAgICB0aGF0LmZpbmQoJ1tkYXRhLXJlbmRlcj1zdGF0ZS11bml0XScpLnRleHQodmFsdWUudW5pdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhhdC5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgZGVsZXRlIHZhbHVlc1tpZF07XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICBPYmplY3QudmFsdWVzKHZhbHVlcykuZm9yRWFjaCgodjpQb29sVmFsdWUpID0+IHtcclxuICAgICAgICAgICAgICAgIGNvbnRhaW5lci5hcHBlbmQoXHJcbiAgICAgICAgICAgICAgICAgICAgYDxkaXYgaWQ9XCIke3YubmFtZX1cIiBjbGFzcz1cInN0YXR1c1ZhbHVlXCI+PGRpdiBkYXRhLXJlbmRlcj1cInN0YXRlLXRleHRcIj4ke3YudGV4dH08L2Rpdj48ZGl2IGRhdGEtcmVuZGVyPVwic3RhdGUtdmFsdWVcIj4ke3YudmFsdWV9PC9kaXY+PGRpdiBkYXRhLXJlbmRlcj1cInN0YXRlLXVuaXRcIj4ke3YudW5pdH08L2Rpdj48L2Rpdj5gKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHRlbXBsYXRlOnN0cmluZyA9IFwiPGRpdlwiXHJcbiAgICAgXHJcbiAgICBkb1BvbGwoKSB7XHJcbiAgICAgICAgJC5nZXQodGhpcy5zZXJ2aWNlVXJsLCAoZGF0YTogUG9vbFN0YXR1cykgPT4ge1xyXG4gICAgICAgICAgICB0aGlzLnJlbmRlcihkYXRhKTtcclxuICAgICAgICAgICAgc2V0VGltZW91dCgoKSA9PiB0aGlzLmRvUG9sbCgpLCAxMDAwKTtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxufVxyXG5cclxuIiwiLy8gVGhlIG1vZHVsZSBjYWNoZVxudmFyIF9fd2VicGFja19tb2R1bGVfY2FjaGVfXyA9IHt9O1xuXG4vLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXHQvLyBDaGVjayBpZiBtb2R1bGUgaXMgaW4gY2FjaGVcblx0aWYoX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXSkge1xuXHRcdHJldHVybiBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdLmV4cG9ydHM7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0obW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIvLyBkZWZpbmUgZ2V0dGVyIGZ1bmN0aW9ucyBmb3IgaGFybW9ueSBleHBvcnRzXG5fX3dlYnBhY2tfcmVxdWlyZV9fLmQgPSAoZXhwb3J0cywgZGVmaW5pdGlvbikgPT4ge1xuXHRmb3IodmFyIGtleSBpbiBkZWZpbml0aW9uKSB7XG5cdFx0aWYoX193ZWJwYWNrX3JlcXVpcmVfXy5vKGRlZmluaXRpb24sIGtleSkgJiYgIV9fd2VicGFja19yZXF1aXJlX18ubyhleHBvcnRzLCBrZXkpKSB7XG5cdFx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkoZXhwb3J0cywga2V5LCB7IGVudW1lcmFibGU6IHRydWUsIGdldDogZGVmaW5pdGlvbltrZXldIH0pO1xuXHRcdH1cblx0fVxufTsiLCJfX3dlYnBhY2tfcmVxdWlyZV9fLm8gPSAob2JqLCBwcm9wKSA9PiBPYmplY3QucHJvdG90eXBlLmhhc093blByb3BlcnR5LmNhbGwob2JqLCBwcm9wKSIsIi8vIGRlZmluZSBfX2VzTW9kdWxlIG9uIGV4cG9ydHNcbl9fd2VicGFja19yZXF1aXJlX18uciA9IChleHBvcnRzKSA9PiB7XG5cdGlmKHR5cGVvZiBTeW1ib2wgIT09ICd1bmRlZmluZWQnICYmIFN5bWJvbC50b1N0cmluZ1RhZykge1xuXHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBTeW1ib2wudG9TdHJpbmdUYWcsIHsgdmFsdWU6ICdNb2R1bGUnIH0pO1xuXHR9XG5cdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCAnX19lc01vZHVsZScsIHsgdmFsdWU6IHRydWUgfSk7XG59OyIsIi8vIG1vZHVsZSBleHBvcnRzIG11c3QgYmUgcmV0dXJuZWQgZnJvbSBydW50aW1lIHNvIGVudHJ5IGlubGluaW5nIGlzIGRpc2FibGVkXG4vLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbnJldHVybiBfX3dlYnBhY2tfcmVxdWlyZV9fKFwiLi9Db250ZW50L3BvcnRQb29sL2luZGV4LnRzXCIpO1xuIl0sInNvdXJjZVJvb3QiOiIifQ==