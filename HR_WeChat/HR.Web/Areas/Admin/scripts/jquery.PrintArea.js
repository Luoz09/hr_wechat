// JavaScript Document
(function ($) {
    var printAreaCount = 0;
    var cssLoadCount = 0;
    $.fn.printArea = function () {
        cssLoadCount = 0;
        var ele = $(this);
        var idPrefix = "printArea_";
        removePrintArea(idPrefix + printAreaCount);
        printAreaCount++;
        var iframeId = idPrefix + printAreaCount;
        var iframeStyle = 'position:absolute;width:0;height:0;left:-50000px;top:-50000px';
        iframe = document.createElement('IFRAME');
        $(iframe).attr({
            style: iframeStyle,
            id: iframeId
        });
        document.body.appendChild(iframe);
        var doc = iframe.contentWindow.document;
        $(document).find("link")
.filter(function () {
    return $(this).attr("rel").toLowerCase() == "stylesheet";
})
.each(function () {
    cssLoadCount++;
    $(doc.documentElement).data("cssLoadCount", cssLoadCount);
    doc.write('<link type="text/css" rel="stylesheet" href="' +
$(this).attr("href") + '" onload="var k=\'data-count\'; var dom=document.getElementById(\'printAreaContainer\'); var count=parseInt(dom.attributes[k].value)-1; dom.setAttribute(k,count);if(count==0){close();focus();print();} " >');
    //$(doc.documentElement).find("head").append(link);
});
        doc.write('<div class="' + $(ele).attr("class") + '" id="printAreaContainer" data-count="' + cssLoadCount + '">' + $(ele).html() + '</div>');
        doc.close();
        /*var frameWindow = iframe.contentWindow;
        frameWindow.close();
        frameWindow.focus();
        alert(2)
        setTimeout(function () {
        frameWindow.print();
        }, 3000);*/
    }
    var removePrintArea = function (id) {
        $("iframe#" + id).remove();
    };

    function cssOnLoad() {
        cssLoadCount--;
        if (cssLoadCount == 0) {
            frameWindow.print();
        }
    }
})(jQuery);