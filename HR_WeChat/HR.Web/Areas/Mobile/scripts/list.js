
var page = 1;
var rows = 15;
var _parent

$(function () {
     
    //列表页面分页 
    $.fn.Page = function () {
        _parent = $(this);
        $(this).scroll(function () {

            //div可显示内容的高度
            divHeight = $(this).height();

            //滚动条与顶部的的高度
            scrollTop = $(this)[0].scrollTop;

            //div内容的总高度 
            scrollHight = $(this)[0].scrollHeight;


            if (scrollTop + divHeight >= scrollHight && parseInt($(".Count").text()) >= rows) {
                page++
                LoadData();
            }
        })
    }

})

function TransferMenu(index) {
    $(".Menus").removeClass("ChooseMenu");
    $(".Menus:eq(" + index + ")").addClass("ChooseMenu");
    location.href = '/Mobile/Home/Index?flag=' + index;
}

function Search() {
    _parent.empty();
    page = 1;
    LoadData();
}

function ShowClear() {
    $(".layui-icon-close-fill").show();
}


function Clear() {
    $(".SearchText").val("");
    _parent.empty();
    page = 1;
    LoadData();
}

 

function HtmlCreate(mark, strName, str) {
    if (typeof (str) == "string") {
        if (str.length > 0) {
            return '' + mark + '<span class="td_label"> ' + strName + '：</span>' + str
        }
        else {
            return "";
        }
    }

    if (typeof (str) == "number") {
        if (str > 0) {
            return '' + mark + '<span class="td_label"> ' + strName + '：</span>' + str
        }
        else {
            return "";
        }
    }
    return "";

}

function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}

 

