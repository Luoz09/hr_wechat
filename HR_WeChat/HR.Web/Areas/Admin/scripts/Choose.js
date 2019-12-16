//打开选择页面 3列/多选
//title  标题 
//page   选择页面
//back   是否回调
function OpenChooseW7(title, page, back) {
    var width = 700;
    var height = 500; 
    $('#Choose').window({
        title: title,
        width: width,
        height: height,
        content: '<iframe scrolling="yes"  frameborder="0"  src="/Admin/Choose/'+page+'?Back='+back+'" style="width:100%;height:98%;"></iframe> ',
        modal: true,
        minimizable: false,
        maximizable: false,
        shadow: false,
        cache: false,
        closed: false,
        collapsible: false, //是否显示可折叠按钮
        resizable: true, //窗口调整大小
        draggable: true, 
    });
}

//打开选择页面 2列/单选
function OpenChooseW5(title, page, back) { 
    $('#Choose').window({
        title: title,
        width: 530,
        height: 550,
        content: '<iframe scrolling="yes"  frameborder="0"  src="/Admin/Choose/' + page + '?Back='+back+'" style="width:100%;height:98%;"></iframe> ',
        modal: true,
        minimizable: false,
        maximizable: false,
        shadow: false,
        cache: false,
        closed: false,
        collapsible: false,
        resizable: true, //窗口调整大小
        draggable: true,
    });
}

function OpenMaps(width,height) { 
    $('#Choose').window({
        title: "地址选择",
        width: width,
        height: height,
        content: '<iframe scrolling="yes"  frameborder="0"  src="../BaiDuMap/Index" style="width:100%;height:98%;"></iframe> ',
        modal: true,
        minimizable: false,
        maximizable: false,
        shadow: false,
        cache: false,
        closed: false,
        collapsible: false,
        resizable: false,
        draggable: false,
    });
}
function OpenSupplierInfo(width, height) {
    $('#Choose').window({
        title: "供应商选择",
        width: width,
        height: height,
        content: '<iframe scrolling="yes"  frameborder="0"  src="../Choose/SupplierInfo" style="width:100%;height:98%;"></iframe> ',
        modal: true,
        minimizable: false,
        maximizable: false,
        shadow: false,
        cache: false,
        closed: false,
        collapsible: false,
        resizable: true,
        draggable: true,
    });
}
  
//确认选择后回调事件
function CallBack() {

}


//要填充数据的元素的id或者class
function Item() {
    return $(".item").val();
}

//根据父节点的ID加载子节点的数据
function Value() {
    return $(".id").val();
}
 
//是否单选
function SelectSingleUser() {
    return $(".selectSingleUser").val();
}

//是否全选
function DefaultSelect() {
    return $(".defaultSelect").val();
}

//关闭选择页面
function close() {
    $("#Choose").window("close");
}

//获取url参数
//name 参数名
function GetUrlParm(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return (r[2]); return null; //返回参数值
}