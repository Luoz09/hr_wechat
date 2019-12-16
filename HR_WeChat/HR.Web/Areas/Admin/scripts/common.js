//调用服务器端的Action
function callAction(url, data, callback) {
    var type = "post";
    if (!data) {
        type = "get";
        data = {};
    }
    $.ajax({
        url: url,
        type: type,
        dataType: "json",
        data: data,
        success: function (data) {
            if (!callback || callback(data)) {
                if (!data.Success) {
                    $.messager.alert(data.Title || "错误", data.Message, "error")
                } else {
                    $.messager.alert("操作成功", "操作成功");
                }
            }
        },
        error: function () {
            $.messager.alert("操作失败", "系统错误", "error")
        }
    })
}

//设置cookies
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

//读取cookies
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg)) {
        return (unescape(arr[2]));
    }
    else
        return null;
}

//删除cookies
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

//去空格
function trim(str) {
    return str.replace(/^\s+|\s+$/g, "");
}

//从数组中返回某个元素的索引,找不到返回-1;
//arr : 数组  elem : 元素
function getIndexInArr(arr, elem) {
    var result = -1;
    $.each(arr, function (i, o) {
        if (o == elem) {
            result = i;
            return false;
        }
    });
    return result;
}

//获取QueryString的数组

function getQueryStringArr() {

    var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));

    if (result == null) {
        return "";
    }

    for (var i = 0; i < result.length; i++) {
        result[i] = result[i].substring(1);
    }
    return result;

}

//根据QueryString参数名称获取值

function getQueryString(name) {

    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));

    if (result == null || result.length < 1) {
        return "";
    }

    return result[1];

}

//根据QueryString参数索引获取值

function getQueryStringByIndex(index) {
    if (index == null) {
        return "";
    }

    var queryStringList = getQueryStringArr();

    if (index >= queryStringList.length) {
        return "";
    }

    var result = queryStringList[index];

    var startIndex = result.indexOf("=") + 1;

    result = result.substring(startIndex);

    return result;

}

//获取页面名
function getPageName() {
    return location.href/*.replace(/.*(?:\/|\\)/g, "")*/.replace(/\?.*/, "");
}

function urlDecode(str) {
    var ret = str.replace(/\%3a/g, ":").replace(/\%2f/g,"/");
    
    return ret;
}

function formatterSort(value, row, index) {
    value = "00" + value
    return value.substr(value.length - 3, 3);
}

function formatterDate(value) {
    if (!value) return "";
    return value.replace(/T.*/g, "");
}

function formatterDateTime(value) {
    if (!value) return "";
    return value.replace("T", " ").replace(/\..*/g, "");
}


function formatterJWDateTime(value) {
    if (!value) return "";
   
    return value.substr(4, 2) + "-" + value.substr(6, 2) + " " + value.substr(8, 2) + ":" + value.substr(10, 2);
}

function formatterLightBoxImage(value) {
    return "<a class='lightbox' href='" + value + "' target='_blank'>查看图片</a>";
}

function formatterCheckBox(value) {
    var checkedStr = value ? " checked='checked'" : "";
    return "<input type='checkbox'" + checkedStr + " value='True'  />"
}

function formatterBoolG(value) {
    if (value) {
        return "<span style='color:#008000'>是</span>"
    }
    return "<span style='color:#F00'>否</span>"
}

function formatterBoolR(value) {
    if (value) {
        return "<span style='color:#F00'>是</span>"
    }
    return "<span style='color:#008000'>否</span>"
}