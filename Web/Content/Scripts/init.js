/*!
 * jQuery Tools v1.2.5 - The missing UI library for the Web
 * 
 * dateinput/dateinput.js
 * tabs/tabs.js
 * tooltip/tooltip.js
 * 
 * NO COPYRIGHTS OR LICENSES. DO WHAT YOU LIKE.
 * 
 * http://flowplayer.org/tools/
 * 
 */
(function(a){a.tools=a.tools||{version:"v1.2.5"};var b=[],c,d=[75,76,38,39,74,72,40,37],e={};c=a.tools.dateinput={conf:{format:"mm/dd/yy",selectors:!1,yearRange:[-5,5],lang:"en",offset:[0,0],speed:0,firstDay:0,min:undefined,max:undefined,trigger:!1,css:{prefix:"cal",input:"date",root:0,head:0,title:0,prev:0,next:0,month:0,year:0,days:0,body:0,weeks:0,today:0,current:0,week:0,off:0,sunday:0,focus:0,disabled:0,trigger:0}},localize:function(b,c){a.each(c,function(a,b){c[a]=b.split(",")}),e[b]=c}},c.localize("en",{months:"January,February,March,April,May,June,July,August,September,October,November,December",shortMonths:"Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec",days:"Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday",shortDays:"Sun,Mon,Tue,Wed,Thu,Fri,Sat"});function f(a,b){return 32-(new Date(a,b,32)).getDate()}function g(a,b){a=""+a,b=b||2;while(a.length<b)a="0"+a;return a}var h=/d{1,4}|m{1,4}|yy(?:yy)?|"[^"]*"|'[^']*'/g,i=a("<a/>");function j(a,b,c){var d=a.getDate(),f=a.getDay(),j=a.getMonth(),k=a.getFullYear(),l={d:d,dd:g(d),ddd:e[c].shortDays[f],dddd:e[c].days[f],m:j+1,mm:g(j+1),mmm:e[c].shortMonths[j],mmmm:e[c].months[j],yy:String(k).slice(2),yyyy:k},m=b.replace(h,function(a){return a in l?l[a]:a.slice(1,a.length-1)});return i.html(m).html()}function k(a){return parseInt(a,10)}function l(a,b){return a.getFullYear()===b.getFullYear()&&a.getMonth()==b.getMonth()&&a.getDate()==b.getDate()}function m(a){if(a){if(a.constructor==Date)return a;if(typeof a=="string"){var b=a.split("-");if(b.length==3)return new Date(k(b[0]),k(b[1])-1,k(b[2]));if(!/^-?\d+$/.test(a))return;a=k(a)}var c=new Date;c.setDate(c.getDate()+a);return c}}function n(c,g){var h=this,i=new Date,n=g.css,o=e[g.lang],p=a("#"+n.root),q=p.find("#"+n.title),r,s,t,u,v,w,x=c.attr("data-value")||g.value||c.val(),y=c.attr("min")||g.min,z=c.attr("max")||g.max,A;y===0&&(y="0"),x=m(x)||i,y=m(y||g.yearRange[0]*365),z=m(z||g.yearRange[1]*365);if(!o)throw"Dateinput: invalid language: "+g.lang;if(c.attr("type")=="date"){var B=a("<input/>");a.each("class,disabled,id,maxlength,name,readonly,required,size,style,tabindex,title,value".split(","),function(a,b){B.attr(b,c.attr(b))}),c.replaceWith(B),c=B}c.addClass(n.input);var C=c.add(h);if(!p.length){p=a("<div><div><a/><div/><a/></div><div><div/><div/></div></div>").hide().css({position:"absolute"}).attr("id",n.root),p.children().eq(0).attr("id",n.head).end().eq(1).attr("id",n.body).children().eq(0).attr("id",n.days).end().eq(1).attr("id",n.weeks).end().end().end().find("a").eq(0).attr("id",n.prev).end().eq(1).attr("id",n.next),q=p.find("#"+n.head).find("div").attr("id",n.title);if(g.selectors){var D=a("<select/>").attr("id",n.month),E=a("<select/>").attr("id",n.year);q.html(D.add(E))}var F=p.find("#"+n.days);for(var G=0;G<7;G++)F.append(a("<span/>").text(o.shortDays[(G+g.firstDay)%7]));a("body").append(p)}g.trigger&&(r=a("<a/>").attr("href","#").addClass(n.trigger).click(function(a){h.show();return a.preventDefault()}).insertAfter(c));var H=p.find("#"+n.weeks);E=p.find("#"+n.year),D=p.find("#"+n.month);function I(b,d,e){x=b,u=b.getFullYear(),v=b.getMonth(),w=b.getDate(),e=e||a.Event("api"),e.type="change",C.trigger(e,[b]);e.isDefaultPrevented()||(c.val(j(b,d.format,d.lang)),c.data("date",b),h.hide(e))}function J(b){b.type="onShow",C.trigger(b),a(document).bind("keydown.d",function(b){if(b.ctrlKey)return!0;var e=b.keyCode;if(e==8){c.val("");return h.hide(b)}if(e==27)return h.hide(b);if(a(d).index(e)>=0){if(!A){h.show(b);return b.preventDefault()}var f=a("#"+n.weeks+" a"),g=a("."+n.focus),i=f.index(g);g.removeClass(n.focus);if(e==74||e==40)i+=7;else if(e==75||e==38)i-=7;else if(e==76||e==39)i+=1;else if(e==72||e==37)i-=1;i>41?(h.addMonth(),g=a("#"+n.weeks+" a:eq("+(i-42)+")")):i<0?(h.addMonth(-1),g=a("#"+n.weeks+" a:eq("+(i+42)+")")):g=f.eq(i),g.addClass(n.focus);return b.preventDefault()}if(e==34)return h.addMonth();if(e==33)return h.addMonth(-1);if(e==36)return h.today();e==13&&(a(b.target).is("select")||a("."+n.focus).click());return a([16,17,18,9]).index(e)>=0}),a(document).bind("click.d",function(b){var d=b.target;!a(d).parents("#"+n.root).length&&d!=c[0]&&(!r||d!=r[0])&&h.hide(b)})}a.extend(h,{show:function(d){if(!(c.attr("readonly")||c.attr("disabled")||A)){d=d||a.Event(),d.type="onBeforeShow",C.trigger(d);if(d.isDefaultPrevented())return;a.each(b,function(){this.hide()}),A=!0,D.unbind("change").change(function(){h.setValue(E.val(),a(this).val())}),E.unbind("change").change(function(){h.setValue(a(this).val(),D.val())}),s=p.find("#"+n.prev).unbind("click").click(function(a){s.hasClass(n.disabled)||h.addMonth(-1);return!1}),t=p.find("#"+n.next).unbind("click").click(function(a){t.hasClass(n.disabled)||h.addMonth();return!1}),h.setValue(x);var e=c.offset();/iPad/i.test(navigator.userAgent)&&(e.top-=a(window).scrollTop()),p.css({top:e.top+c.outerHeight({margins:!0})+g.offset[0],left:e.left+g.offset[1]}),g.speed?p.show(g.speed,function(){J(d)}):(p.show(),J(d));return h}},setValue:function(b,c,d){var e=k(c)>=-1?new Date(k(b),k(c),k(d||1)):b||x;e<y?e=y:e>z&&(e=z),b=e.getFullYear(),c=e.getMonth(),d=e.getDate(),c==-1?(c=11,b--):c==12&&(c=0,b++);if(!A){I(e,g);return h}v=c,u=b;var j=new Date(b,c,1-g.firstDay),m=j.getDay(),p=f(b,c),r=f(b,c-1),w;if(g.selectors){D.empty(),a.each(o.months,function(c,d){y<new Date(b,c+1,-1)&&z>new Date(b,c,0)&&D.append(a("<option/>").html(d).attr("value",c))}),E.empty();var B=i.getFullYear();for(var C=B+g.yearRange[0];C<B+g.yearRange[1];C++)y<=new Date(C+1,-1,1)&&z>new Date(C,0,0)&&E.append(a("<option/>").text(C));D.val(c),E.val(b)}else q.html(o.months[c]+" "+b);H.empty(),s.add(t).removeClass(n.disabled);for(var F=m?0:-7,G,J;F<(m?42:35);F++)G=a("<a/>"),F%7===0&&(w=a("<div/>").addClass(n.week),H.append(w)),F<m?(G.addClass(n.off),J=r-m+F+1,e=new Date(b,c-1,J)):F<m+p?(J=F-m+1,e=new Date(b,c,J),l(x,e)?G.attr("id",n.current).addClass(n.focus):l(i,e)&&G.attr("id",n.today)):(G.addClass(n.off),J=F-p-m+1,e=new Date(b,c+1,J)),y&&e<y&&G.add(s).addClass(n.disabled),z&&e>z&&G.add(t).addClass(n.disabled),G.attr("href","#"+J).text(J).data("date",e),w.append(G);H.find("a").click(function(b){var c=a(this);c.hasClass(n.disabled)||(a("#"+n.current).removeAttr("id"),c.attr("id",n.current),I(c.data("date"),g,b));return!1}),n.sunday&&H.find(n.week).each(function(){var b=g.firstDay?7-g.firstDay:0;a(this).children().slice(b,b+1).addClass(n.sunday)});return h},setMin:function(a,b){y=m(a),b&&x<y&&h.setValue(y);return h},setMax:function(a,b){z=m(a),b&&x>z&&h.setValue(z);return h},today:function(){return h.setValue(i)},addDay:function(a){return this.setValue(u,v,w+(a||1))},addMonth:function(a){return this.setValue(u,v+(a||1),w)},addYear:function(a){return this.setValue(u+(a||1),v,w)},hide:function(b){if(A){b=a.Event(),b.type="onHide",C.trigger(b),a(document).unbind("click.d").unbind("keydown.d");if(b.isDefaultPrevented())return;p.hide(),A=!1}return h},getConf:function(){return g},getInput:function(){return c},getCalendar:function(){return p},getValue:function(a){return a?j(x,a,g.lang):x},isOpen:function(){return A}}),a.each(["onBeforeShow","onShow","change","onHide"],function(b,c){a.isFunction(g[c])&&a(h).bind(c,g[c]),h[c]=function(b){b&&a(h).bind(c,b);return h}}),c.bind("focus click",h.show).keydown(function(b){var c=b.keyCode;if(!A&&a(d).index(c)>=0){h.show(b);return b.preventDefault()}return b.shiftKey||b.ctrlKey||b.altKey||c==9?!0:b.preventDefault()}),m(c.val())&&I(x,g)}a.expr[":"].date=function(b){var c=b.getAttribute("type");return c&&c=="date"||a(b).data("dateinput")},a.fn.dateinput=function(d){if(this.data("dateinput"))return this;d=a.extend(!0,{},c.conf,d),a.each(d.css,function(a,b){!b&&a!="prefix"&&(d.css[a]=(d.css.prefix||"")+(b||a))});var e;this.each(function(){var c=new n(a(this),d);b.push(c);var f=c.getInput().data("dateinput",c);e=e?e.add(f):f});return e?e:this}})(jQuery);
(function(a){a.tools=a.tools||{version:"v1.2.5"},a.tools.tabs={conf:{tabs:"a",current:"current",onBeforeClick:null,onClick:null,effect:"default",initialIndex:0,event:"click",rotate:!1,history:!1},addEffect:function(a,c){b[a]=c}};var b={"default":function(a,b){this.getPanes().hide().eq(a).show(),b.call()},fade:function(a,b){var c=this.getConf(),d=c.fadeOutSpeed,e=this.getPanes();d?e.fadeOut(d):e.hide(),e.eq(a).fadeIn(c.fadeInSpeed,b)},slide:function(a,b){this.getPanes().slideUp(200),this.getPanes().eq(a).slideDown(400,b)},ajax:function(a,b){this.getPanes().eq(0).load(this.getTabs().eq(a).attr("href"),b)}},c;a.tools.tabs.addEffect("horizontal",function(b,d){c||(c=this.getPanes().eq(0).width()),this.getCurrentPane().animate({width:0},function(){a(this).hide()}),this.getPanes().eq(b).animate({width:c},function(){a(this).show(),d.call()})});function d(c,d,e){var f=this,g=c.add(this),h=c.find(e.tabs),i=d.jquery?d:c.children(d),j;h.length||(h=c.children()),i.length||(i=c.parent().find(d)),i.length||(i=a(d)),a.extend(this,{click:function(c,d){var i=h.eq(c);typeof c=="string"&&c.replace("#","")&&(i=h.filter("[href*="+c.replace("#","")+"]"),c=Math.max(h.index(i),0));if(e.rotate){var k=h.length-1;if(c<0)return f.click(k,d);if(c>k)return f.click(0,d)}if(!i.length){if(j>=0)return f;c=e.initialIndex,i=h.eq(c)}if(c===j)return f;d=d||a.Event(),d.type="onBeforeClick",g.trigger(d,[c]);if(!d.isDefaultPrevented()){b[e.effect].call(f,c,function(){d.type="onClick",g.trigger(d,[c])}),j=c,h.removeClass(e.current),i.addClass(e.current);return f}},getConf:function(){return e},getTabs:function(){return h},getPanes:function(){return i},getCurrentPane:function(){return i.eq(j)},getCurrentTab:function(){return h.eq(j)},getIndex:function(){return j},next:function(){return f.click(j+1)},prev:function(){return f.click(j-1)},destroy:function(){h.unbind(e.event).removeClass(e.current),i.find("a[href^=#]").unbind("click.T");return f}}),a.each("onBeforeClick,onClick".split(","),function(b,c){a.isFunction(e[c])&&a(f).bind(c,e[c]),f[c]=function(b){b&&a(f).bind(c,b);return f}}),e.history&&a.fn.history&&(a.tools.history.init(h),e.event="history"),h.each(function(b){a(this).bind(e.event,function(a){f.click(b,a);return a.preventDefault()})}),i.find("a[href^=#]").bind("click.T",function(b){f.click(a(this).attr("href"),b)}),location.hash&&e.tabs=="a"&&c.find("[href="+location.hash+"]").length?f.click(location.hash):(e.initialIndex===0||e.initialIndex>0)&&f.click(e.initialIndex)}a.fn.tabs=function(b,c){var e=this.data("tabs");e&&(e.destroy(),this.removeData("tabs")),a.isFunction(c)&&(c={onBeforeClick:c}),c=a.extend({},a.tools.tabs.conf,c),this.each(function(){e=new d(a(this),b,c),a(this).data("tabs",e)});return c.api?e:this}})(jQuery);
(function(a){a.tools=a.tools||{version:"v1.2.5"},a.tools.tooltip={conf:{effect:"toggle",fadeOutSpeed:"fast",predelay:0,delay:30,opacity:1,tip:0,position:["top","center"],offset:[0,0],relative:!1,cancelDefault:!0,events:{def:"mouseenter,mouseleave",input:"focus,blur",widget:"focus mouseenter,blur mouseleave",tooltip:"mouseenter,mouseleave"},layout:"<div/>",tipClass:"tooltip"},addEffect:function(a,c,d){b[a]=[c,d]}};var b={toggle:[function(a){var b=this.getConf(),c=this.getTip(),d=b.opacity;d<1&&c.css({opacity:d}),c.show(),a.call()},function(a){this.getTip().hide(),a.call()}],fade:[function(a){var b=this.getConf();this.getTip().fadeTo(b.fadeInSpeed,b.opacity,a)},function(a){this.getTip().fadeOut(this.getConf().fadeOutSpeed,a)}]};function c(b,c,d){var e=d.relative?b.position().top:b.offset().top,f=d.relative?b.position().left:b.offset().left,g=d.position[0];e-=c.outerHeight()-d.offset[0],f+=b.outerWidth()+d.offset[1],/iPad/i.test(navigator.userAgent)&&(e-=a(window).scrollTop());var h=c.outerHeight()+b.outerHeight();g=="center"&&(e+=h/2),g=="bottom"&&(e+=h),g=d.position[1];var i=c.outerWidth()+b.outerWidth();g=="center"&&(f-=i/2),g=="left"&&(f-=i);return{top:e,left:f}}function d(d,e){var f=this,g=d.add(f),h,i=0,j=0,k=d.attr("title"),l=d.attr("data-tooltip"),m=b[e.effect],n,o=d.is(":input"),p=o&&d.is(":checkbox, :radio, select, :button, :submit"),q=d.attr("type"),r=e.events[q]||e.events[o?p?"widget":"input":"def"];if(!m)throw"Nonexistent effect \""+e.effect+"\"";r=r.split(/,\s*/);if(r.length!=2)throw"Tooltip: bad events configuration for "+q;d.bind(r[0],function(a){clearTimeout(i),e.predelay?j=setTimeout(function(){f.show(a)},e.predelay):f.show(a)}).bind(r[1],function(a){clearTimeout(j),e.delay?i=setTimeout(function(){f.hide(a)},e.delay):f.hide(a)}),k&&e.cancelDefault&&(d.removeAttr("title"),d.data("title",k)),a.extend(f,{show:function(b){if(!h){l?h=a(l):e.tip?h=a(e.tip).eq(0):k?h=a(e.layout).addClass(e.tipClass).appendTo(document.body).hide().append(k):(h=d.next(),h.length||(h=d.parent().next()));if(!h.length)throw"Cannot find tooltip for "+d}if(f.isShown())return f;h.stop(!0,!0);var o=c(d,h,e);e.tip&&h.html(d.data("title")),b=b||a.Event(),b.type="onBeforeShow",g.trigger(b,[o]);if(b.isDefaultPrevented())return f;o=c(d,h,e),h.css({position:"absolute",top:o.top,left:o.left}),n=!0,m[0].call(f,function(){b.type="onShow",n="full",g.trigger(b)});var p=e.events.tooltip.split(/,\s*/);h.data("__set")||(h.bind(p[0],function(){clearTimeout(i),clearTimeout(j)}),p[1]&&!d.is("input:not(:checkbox, :radio), textarea")&&h.bind(p[1],function(a){a.relatedTarget!=d[0]&&d.trigger(r[1].split(" ")[0])}),h.data("__set",!0));return f},hide:function(c){if(!h||!f.isShown())return f;c=c||a.Event(),c.type="onBeforeHide",g.trigger(c);if(!c.isDefaultPrevented()){n=!1,b[e.effect][1].call(f,function(){c.type="onHide",g.trigger(c)});return f}},isShown:function(a){return a?n=="full":n},getConf:function(){return e},getTip:function(){return h},getTrigger:function(){return d}}),a.each("onHide,onBeforeShow,onShow,onBeforeHide".split(","),function(b,c){a.isFunction(e[c])&&a(f).bind(c,e[c]),f[c]=function(b){b&&a(f).bind(c,b);return f}})}a.fn.tooltip=function(b){var c=this.data("tooltip");if(c)return c;b=a.extend(!0,{},a.tools.tooltip.conf,b),typeof b.position=="string"&&(b.position=b.position.split(/,?\s/)),this.each(function(){c=new d(a(this),b),a(this).data("tooltip",c)});return b.api?c:this}})(jQuery);

/* Formalize by Nathan Smith - http://formalize.me */

var FORMALIZE=function(a,b,c,d){var e="placeholder"in c.createElement("input"),f="autofocus"in c.createElement("input"),g=!!a.browser.msie&&parseInt(a.browser.version,10)===6,h=!!a.browser.msie&&parseInt(a.browser.version,10)===7;return{go:function(){for(var a in FORMALIZE.init)FORMALIZE.init[a]()},init:{full_input_size:function(){!!h&&!!a("textarea, input.input_full").length&&a("textarea, input.input_full").wrap('<span class="input_full_wrap"></span>')},ie6_skin_inputs:function(){if(!!g&&!!a("input, select, textarea").length){var b=/button|submit|reset/,c=/date|datetime|datetime-local|email|month|number|password|range|search|tel|text|time|url|week/;a("input").each(function(){var d=a(this);this.getAttribute("type").match(b)?(d.addClass("ie6_button"),this.disabled&&d.addClass("ie6_button_disabled")):this.getAttribute("type").match(c)&&(d.addClass("ie6_input"),this.disabled&&d.addClass("ie6_input_disabled"))}),a("textarea, select").each(function(){this.disabled&&a(this).addClass("ie6_input_disabled")})}},autofocus:function(){!f&&!!a(":input[autofocus]").length&&a(":input[autofocus]:visible:first").focus()},placeholder:function(){!e&&!!a(":input[placeholder]").length&&(FORMALIZE.misc.add_placeholder(),a(":input[placeholder]").each(function(){var b=a(this),c=b.attr("placeholder");b.focus(function(){b.val()===c&&b.val("").removeClass("placeholder_text")}).blur(function(){FORMALIZE.misc.add_placeholder()}),b.closest("form").submit(function(){b.val()===c&&b.val("").removeClass("placeholder_text")}).bind("reset",function(){setTimeout(FORMALIZE.misc.add_placeholder,50)})}))}},misc:{add_placeholder:function(){!e&&!!a(":input[placeholder]").length&&a(":input[placeholder]").each(function(){var b=a(this),c=b.attr("placeholder");(!b.val()||b.val()===c)&&b.val(c).addClass("placeholder_text")})}}}}(jQuery,this,this.document);

/* Custom plugin for checkbox toggling. */

(function($) {

    $.fn.checkToggle = function() {
    
        // Loop each selected element, IE $('element').checkToggle();
        return this.each(function() {
        
            var trigger = $('.trigger', $(this)),
                toggle = $('.toggle', $(this));
                
            runToggle(trigger, toggle);
            
            trigger.click(function() {
                
                runToggle(trigger, toggle);
                
            })
        
        });
        
        function runToggle(trigger, toggle) {
        
            var elements = 'input, select, textarea';
        
            if (trigger.is(':checked')) {
                toggle.removeClass('disabled').css({ opacity: 1 });
                $(elements, toggle).removeAttr('disabled');
            } else {
                toggle.addClass('disabled').css({ opacity: 0.3 });
                $(elements, toggle).attr('disabled', 'disabled');
            }
        
        }
    
    }

})(jQuery);


$(document).ready(function () {

    FORMALIZE.go();

    // ---------------------------------------------------------------------------------
    // Nav drop down.
    $('nav > ul > li').hover(function () {
        $(this).addClass('hover').find('ul').show();
    }, function () {
        $(this).removeClass('hover').find('ul').hide();
    });

    // ---------------------------------------------------------------------------------
    // Tabs, Datepicker
    $('ul.boxtabs').tabs('.panes > div');

    $('input[type="datepicker"]:not(".birthDate")').datepicker({
        changeYear: true,
        gotoCurrent: true
    });

    $('input[type=text].birthDate').mask("99/99/9999");

    // ---------------------------------------------------------------------------------
    // Checkbox toggle stuff
    $('.checktoggle').checkToggle();

    // ---------------------------------------------------------------------------------
    // Login input focus
    $('input:first', $('.login')).focus();

    // ---------------------------------------------------------------------------------
    // jqGrid

    // Buttons
    $('input.link').click(function (e) {
        smartClick(e.target);
    });

    $('button.link').click(function (e) {
        smartClick(e.target);
    });

    $('a.link').click(function (e) {
        smartClick($(e.target).closest('a'));
    });

    // Change enter key to a tab
    trapEnter($('input'));

    $.ajaxSetup({
        cache: false
    });
});

function scanFormSmartClick(src) {

    $(src).find('input.link').click(function (e) {
        smartClick(e.target);
    });

    $(src).find('button.link').click(function (e) {
        smartClick(e.target);
    });

    $(src).find('a.link').click(function (e) {
        smartClick($(e.target).closest('a'));
    });
}

// ********** HELPER FUNCTIONS *************** //

function smartClick(src) {
    
    var dest = $(src).attr("data-url");
    var prompt = $(src).attr("data-prompt");
    var submit = $(src).attr("data-submit");

    if (dest == '#') {
        return;
    }

    if (typeof (prompt) != "undefined") {

        $('#confirm-dialog-content').html('<img src="/Content/Images/prompt.jpg" style="float:left;">' + prompt);
        $('#confirm-dialog-content').data('target', src);

        $("#confirm-dialog").dialog({
            resizable: false,
            height: 180,
            width: 450,
            modal: true,
            buttons: {
                "Yes": function () {
                    var target = $('#confirm-dialog-content').data('target');
                    $(target).attr("data-prompt",null);
                    smartClick($(target));
                },
                Cancel: function () {
                    $('#confirm-dialog-content').data('target',null);
                    $(this).dialog("close");
                }
            }
        });

        $(".ui-dialog-titlebar").hide();

        return;

    }

    if (typeof (submit) != "undefined") {
        showLoading();
        $(src).closest('form').submit();
        return;
    }

    smoothLoad(dest);

}


function showLoading() {
    
    $('#main').hide();
    $('#loading-dialog').dialog({
        height: 180,
        modal: true,
        resizable: false
    });

    $(".ui-dialog-titlebar").hide();
}


function smoothLoad(dest) {
    showLoading();
    window.location.href = dest;
}

function trapEnter(elements) {
    $(elements).keypress(function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;

        if ($(evt.target).hasClass('do-submit') && charCode == 13) {
            showLoading();
            $(evt.target).closest('form').submit();
            return false;
        }

        if ($(evt.target).hasClass('do-tab') && charCode == 13) {
            /* FOCUS ELEMENT */
            var inputs = $(evt.target).parents("form").eq(0).find(":input");
            var idx = inputs.index(evt.target);

            if (idx == inputs.length - 1) {
                inputs[0].select()
            } else {
                inputs[idx + 1].focus(); //  handles submit buttons
                inputs[idx + 1].select();
            }
            return false;
        }

        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    });
}

function criteriaResource(name) {

        var d = $('<div style="width:800px;height:500px;overflow:auto;"><img src="/Content/Images/CriteriaResources/' + name + '.jpg"></div>');
        d.dialog({
                modal: true,
                title: "Criteria Resource",
                width:810,
                height:510
            });

        return false;
}

function showAudit(path) {

    $('#main').hide();
    $('#audit-dialog').html('');

    $('#audit-dialog').dialog({
        modal: true,
        title: "Changes",
        width: 810,
        height: 510,
        close: function () { $('#main').show(); }
    });

    loadAudit(path);
}

function loadAudit(path) {
    $('#audit-dialog').load(path);
}

// ********** ANNOTATION FUNCTIONS *************** //


function initAnnotations() {

    var ttContainer = $('<div id="annotationToolTip"/>');
    $('body').append(ttContainer);

    $('#annotationToolTip').qtip({
        content: {
            text: '-',
            title:
            {
                text: ' ',
                button: true
            }
        },
        position: {
            target: 'mouse',
            viewport: $(window),
            adjust: {
                mouse: true  // Can be omitted (e.g. default behaviour)
            }
        },
        style: {
            classes: 'qtip-shadow qtip-jtools'
        }
    });
}




function clickAnnotations(target, url) {

    if ($('#annotationToolTip').length < 1) {
        initAnnotations();
    }

    $('#annotationToolTip').qtip('api').set('content.title.text', 'Loading:');
    $('#annotationToolTip').qtip('api').set('content.text', 'Please wait..');
    $('#annotationToolTip').qtip('api').set('position.target', target);
    $('#annotationToolTip').qtip('show');

    $.ajaxSetup({ cache: false });
    $.getJSON(url, function (data) {

        var container = $('<div/>');

        for (var i = 0; i < data.length; i++) {
            var d = $('<div/>', { html: data[i] });
            d.css('padding', '2px');
            d.css('background-color', '#1c2428');
            d.css('border', 'solid 1px #6d777d');

            var o = $('<div/>');
            o.css('padding', '2px');
            o.appendTo(container);
            d.appendTo(o);
        }

        $('<br/>').appendTo(container);


        $('#annotationToolTip').qtip('api').set('content.title.text', 'Details');

        $('#annotationToolTip').qtip('api').set('content.text', container);

    });

    return false;
}