////打开选择页面 3列/多选
////title  标题 
////page   选择页面
////back   是否回调  
//function OpenChooseW7(title,width,height,page,back) {
//    $('#Choose').window({
//        title: title,
//        width: 700,
//        height: 550,
//        content: '<iframe scrolling="yes"  frameborder="0"  src="../Choose/'+page+'?Back='+back+'" style="width:100%;height:98%;"></iframe> ',
//        modal: true,
//        minimizable: false,
//        maximizable: false,
//        shadow: false,
//        cache: false,
//        closed: false,
//        collapsible: false, //是否显示可折叠按钮
//        resizable: false, //窗口调整大小
//        draggable: false, 
//    });
//}


//手机版选择页面 
function OpenChooseMB(title,width,height, page,back) {
    $('#Choose').window({
        title: title,
        width: width,
        height: height,
        content: '<iframe scrolling="yes"  frameborder="0"  src="/Mobile/Choose/' + page + '?Back='+back+'" style="width:100%;height:99%;overflow:hidden"></iframe> ',
        modal: true,
        minimizable: false,
        maximizable: false,
        shadow: false,
        cache: false,
        top:0,
        closed: false,
        collapsible: false,
        resizable: false,
        draggable: false,
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

//关闭选择页面
function close() {
    $("#Choose").window("close");
}