
import { PoolStatus, StatusLine } from './poolStatus'
import { PoolValue } from './poolValue'
 
export default class PortPool {   
    serviceUrl:string;
    constructor(serviceUrl:string) {
        this.serviceUrl = serviceUrl; 
        this.doPoll();
    }

    processConfigInput() { 
    }

    websProcessKey(id: string) {
    }   
     
    render(data: PoolStatus): void {
        if (data.displayText) {
            var renderText = $('#displayText [data-render=displayText]');
            data.displayText.forEach((t: StatusLine, ix: number) => {
                $(renderText[ix]).text(t.text);
            });
            
        }
        if (data.states) {
            var current = $('#statusKeys .WEBS_KEY');
            var enabled = data.states.split(",").reduce((o, s) => {
                o[s.trim()] = true;
                return o;
            }, {});
            current.each(function() {
                $(this).toggleClass('WEBS_ON', enabled[$(this).attr('id')] == true);
            });
        }
        if (data.statusValues) {
            var container = $('#statusValues');
            var current = container.find('.statusValue');
            var values = data.statusValues.reduce((o, s) => {
                o[s.name] = s;
                return o;
            }, {});
            current.each(function () {
                var that = $(this);
                var id = that.attr('id');
                var value = <PoolValue>values[id];
                if (!value) {
                    that.hide();
                } else {
                    that.find('[data-render=state-text]').text(value.text);
                    that.find('[data-render=state-value]').text(value.value);
                    that.find('[data-render=state-unit]').text(value.unit);
                    that.show();
                    delete values[id];
                }
            });
            Object.values(values).forEach((v:PoolValue) => {
                container.append(
                    `<div id="${v.name}" class="statusValue"><div data-render="state-text">${v.text}</div><div data-render="state-value">${v.value}</div><div data-render="state-unit">${v.unit}</div></div>`);
            });
        }
    }

    template:string = "<div"
     
    doPoll() {
        $.get(this.serviceUrl, (data: PoolStatus) => {
            this.render(data);
            setTimeout(() => this.doPoll(), 1000);
        });
    }
}

