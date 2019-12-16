 

var dictionaryUrl = "/Admin/SYS_DICTIONARY/GetDictionaryData";

$(function () {
     
      
    //默认面板展开状态
    $(".layui-colla-content").addClass("layui-show");
    
    var time = new Date();
     
    ////年月选择
    //lay(".YMonth").each(function () {
    //    laydate.render({
    //        elem: this,
    //        value: new Date(),
    //        type: "month",  
    //    })
    //})

    ////日期选择
    //lay(".Day").each(function () {
    //    laydate.render({
    //        elem: this,
    //        value: new Date(),
    //    })
    //})
    
    ////日期时间选择
    //lay(".Date").each(function () {
    //    laydate.render({
    //        elem: this,
    //        type: "datetime",
    //        value:new Date(),
    //        change: function (value, date) {
    //            $(".laydate-btns-time").click();
    //            //$(".layui-laydate-list li:eq(0) ol li").removeClass("layui-this");
    //            //$(".layui-laydate-list li:eq(0) ol li:eq(" + time.getHours() + ")").addClass("layui-this"); 
    //        }
    //    })
    //})
     
    layui.use(['element', 'form', 'table','upload'], function () {

        var table = layui.table;
        var form = layui.form;
        var upload = layui.upload;
        var $ = layui.$; 
         

        //获取字典数据--单选 value=itemid
        var Dictionary = $(".Dictionary");
        for (var i = 0; i < Dictionary.size() ; i++) {
            var _this = Dictionary[i];
            $.ajax({
                url: dictionaryUrl+"?itemTypeID=" + _this.id,
                async: false,
                success: function (data) {
                    for (var j = 0; j < data.length; j++) {
                        var _self = data[j];
                        $("#" + _this.id + "").append("<option value=" + _self.ItemID + ">" + _self.ItemText + "</option>")
                    }
                    $("#" + _this.id + "").prepend("<option value='' selected>请选择</option>")

                }
            })
        }

        //获取字典数据--单选 value=itemvalue
        var Dictionary = $(".DictionaryValue");
        for (var i = 0; i < Dictionary.size() ; i++) {
            var _this = Dictionary[i];
            $.ajax({
                url: dictionaryUrl+"?itemTypeID=" + _this.id,
                async: false,
                success: function (data) {
                    for (var j = 0; j < data.length; j++) {
                        var _self = data[j];
                        $("#" + _this.id + "").append("<option value=" + _self.ItemValue + ">" + _self.ItemText + "</option>")
                    }
                     $("#" +_this.id + "").prepend("<option value='0' selected>请选择</option>")
                }
            })
        }
        var Dictionary = $(".DictionaryNullValue");
        for (var i = 0; i < Dictionary.size() ; i++) {
            var _this = Dictionary[i];
            $.ajax({
                url: dictionaryUrl + "?itemTypeID=" + _this.id,
                async: false,
                success: function (data) {
                    for (var j = 0; j < data.length; j++) {
                        var _self = data[j];
                        $("#" + _this.id + "").append("<option value=" + _self.ItemValue + ">" + _self.ItemText + "</option>")
                    }
                    $("#" + _this.id + "").prepend("<option value='' selected>请选择</option>")
                }
            })
        }
        form.render();
         
        //编辑页面下拉框默认选中
        var selectsize = $("select");
        for (var i = 0; i < selectsize.size() ; i++) {
            var _this = selectsize[i];
            var text = $(_this).attr("lay-value");
            if (text != "" && _this.id != "") {
                var select = 'dd[lay-value=' + text + ']';
                $("#" + _this.id).siblings("div.layui-form-select").find('dl').find(select).click();
            }
        }
        
        

        var demoListView = $('#demoList');
         
        //编辑页面获取相关的附件
        var id = getUrlParam("id");
        var HeadType = getUrlParam("HeadType");
          if (id != null && HeadType==null) {
                var imgExt = ".png .jpg .jpeg .bmp .gif";
                $("#UpListAction").hide();
                $.ajax({
                    url: "/Admin/Attach/GetFiles?id=" + id,
                    async:false,
                    success:function(data){
                        for(var i=0;i<data.length;i++){
                            var _self = data[i]; 
                            var ext = _self.TITLE.split('.')[1];

                            //判断附件类型是否为图片 
                            if (imgExt.indexOf(ext) >= 0) {
                                var tr = $(['<tr id=' + _self.ID + '>',
                                    '<td style="width:20%!important;"><i class="fa fa-check-square-o" aria-hidden="true"></i></td>',
                                  '<td style="width:30%!important;">' + _self.TITLE + '</td>',
                                  '<td style="width:40%!important;"><a href=' + _self.FILE_PATH + ' download=' + _self.TITLE + '><img src=' + _self.FILE_PATH + '></a></td>',
                                  
                                  '<td style="width:10%!important;">',
                                  '<i class="fa fa-trash-o DeleteFiles" onclick="Delete(this)" style="cursor:pointer;font-size:20px;"></i>',
                                  '</td>',
                                  '</tr>'].join(''));
                            }
                            else {
                                var tr = $(['<tr id=' + _self.ID + '>',
                                    '<td><i class="fa fa-check-square-o" aria-hidden="true"></i></td>',
                                '<td>' + _self.TITLE + '</td>',
                                '<td><a href=' + _self.FILE_PATH + ' style="color:blue">' + _self.TITLE + '</td>',
                                
                                '<td>',
                                '<i class="fa fa-trash-o DeleteFiles" onclick="Delete(this)" style="cursor:pointer;font-size:20px;"></i>',
                                //'<input class="layui-btn layui-btn-mini layui-btn-danger demo-delete" value="删除" style="width:120px!important;margin-left:10px;" readonly onclick="Delete(this)"/>',
                                '</td>',
                                '</tr>'].join(''));
                          }
                       
                            demoListView.append(tr);
                             
                        }
                    }
                })
          }

        //不可对附件进行操作
        if (getUrlParam("ACTIVITY_ID") != null || getUrlParam("View") == 1) {
            $(".DeleteFiles").attr("style", "display:none");
            $(".layui-btn").attr('disabled', true);
            $(".layui-btn").addClass("layui-btn-disabled");
        }

        //没有附件时，隐藏上传按钮 。
        if (demoListView.find("tr").size() == 0) {
            $(".layui-upload-list").hide();
            $("#UpListAction").hide();
        }

        //附件上传
        uploadListIns = upload.render({
            elem: '#testList',
            url: '/Admin/Attach/UpLoad',
            accept: 'file',
            multiple: true,
            auto: false,
            bindAction: '#UpListAction', //开始上传按钮
            choose: function (obj) { //选择文件
                $(".layui-upload-list").show();
                $("#UpListAction").show();
                var files = this.files = obj.pushFile(); //将每次选择的文件追加到文件队列
                //读取本地文件
                obj.preview(function (index, file, result) {
                    var tr = $(['<tr id="upload-' + index + '">',
                        '<td><i class="fa fa-upload" aria-hidden="true"></i></td>',
                        '<td>' + file.name + '</td>',
                        '<td>' + (file.size / 1014).toFixed(1) + 'kb</td>',
                       
                        '<td>',
                        '<i class="fa fa-repeat  demo-operation layui-hide"  style="cursor:pointer;font-size:20px;width:50px;" aria-hidden="true"></i>',
                        '<i class="fa fa-trash-o demo-operation layui-hide"  style="cursor:pointer;font-size:20px;width:50px;"></i>',
                        '</td>',
                        '</tr>'].join(''));

                    //单个重传
                    tr.find('.fa-repeat').on('click', function () {
                        obj.upload(index, file);
                    });

                    //删除
                    tr.find('.fa-trash-o').on('click', function () {
                        delete files[index]; //删除对应的文件
                        tr.remove(); 
                        uploadListIns.config.elem.next()[0].value = ''; //清空 input file 值，以免删除后出现同名文件不可选
                        if ($("#demoList tr").size() == 0) {
                            $(".layui-upload-list").hide();
                            $("#UpListAction").hide();
                        }
                    });

                    demoListView.append(tr);
                });
            },
            done: function (res, index, upload) {  //上传文件
                if (res.code == 0) { //上传成功

                    //上传成功后记录文件名
                    var files = res.srcName + "," + $("#Files").val();
                    $("#Files").val(files);
                    $("#UploadTime").val(res.uploadtime);

                    var tr = demoListView.find('tr#upload-' + index),
                    tds = tr.children();
                    tds.eq(0).html('<i class="fa fa-check-square-o" aria-hidden="true"></i>');
                    tds.eq(3).html(''); //清空操作
                    return delete this.files[index]; //删除文件队列已经上传成功的文件
                }
                this.error(index, upload);
            },
            error: function (index, upload) {
                var tr = demoListView.find('tr#upload-' + index),
                tds = tr.children();
                tds.eq(0).html('<span style="color: #FF5722;">上传失败</span>');
                tds.eq(3).find('.demo-operation').removeClass('layui-hide'); //显示重传
            }
        });

    })
})

  
 
//删除附件
function Delete(obj) {
    var _self = obj.closest("tr");
    $.ajax({
        url: "/Admin/Attach/DeleteFile?key=" + _self.id,
        success: function (data) {
            if (data) {
                _self.remove();
                if ($("#demoList tr").size() == 0) {
                    $(".layui-upload-list").hide();
                    $("#UpListAction").hide();
                }
            }
        }
    }) 
}

//提交数据前的验证方法
function beforeSubmit()
{
    return true;
}

//获取url中参数
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}
