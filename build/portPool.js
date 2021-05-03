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
export default PortPool;
//# sourceMappingURL=portPool.js.map