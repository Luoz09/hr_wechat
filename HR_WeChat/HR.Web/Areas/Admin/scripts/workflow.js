var wfParms =
    {
        ProcessDinfineKey: "",
        AppName: "",
        WorkItemID: "",
        ActivityName: "",
        TaskTitle: "",
        NextActivity: "",
        TaskSubject: "",
        ActivityDCode: "N1",
        NextStepActivity: "",
        ActivityInstanceID: "",
        ProcessInstanceID: "",
        IsExitNode: "false",
        IsWithdraw: "false",
        IsProcessInstanceEnd: "false",
        selectSingleUser: "",
        defaultSelect: "true",
        NextUsersGUID: "",
        NextUsersFullPath: "",

        parentID: "",
    }

var UserData;
var wfFunction = {
    //添加工作流控件
    AddWorkFlowDom:function(dom){
        var html = [
                 '<div class="layui-colla-item WFlow">',
                 '<h2 class="layui-colla-title">审批流程 &emsp;<span class="WFActivityName"></span></h2>',
                 '<div class="layui-colla-content">',
                 '<div class="layui-form-item Opinion">',
                 '<label class="layui-form-label">审批意见</label>',
                 '<div class="layui-input-block">',
                 '<input type="radio" name="Opinion" value="同意" title="同意" lay-filter="Opinion" checked>',
                 '<input type="radio" name="Opinion" value="退回" title="退回" lay-filter="Opinion">',
                 '<input type="radio" name="Opinion" value="不同意" title="不同意" lay-filter="Opinion">', 
                 '</div>',
                 '</div>',
                 '<div class="layui-form-item layui-form-text Opinion">',
                 '<label class="layui-form-label" style="margin-bottom:0px!important">审批说明</label>',
                 '<div class="layui-input-block">',
                 '<textarea class="layui-textarea" cols="20" name="AppendOpinion" rows="2"></textarea>',
                 '</div>',
                 '</div>',
                 '<div class="layui-form-item WorkFlow">',
                 '<label class="layui-form-label"><span style="color:red;padding-left:-10px;">*</span>下一节点</label>',
                 '<div class="layui-input-block">',
                 '<select class="layui-input NextStep" name="NextStep" lay-filter="NextStep" onchange="wfFunction.ChangeUsers(this)"></select>',
                 '</div>',
                 '</div>',
                 '<div class="layui-form-item WorkFlow">',
                 '<label class="layui-form-label"><span style="color:red;padding-left:-10px;">*</span>节点人员</label>',
                 '<div class="layui-input-block">',
                 '<input class="layui-input NextUsers" readonly id="NextUsers" name="NextUsers" placeholder="请选择审批人员" onclick="wfFunction.ChooseNextUser()" />',
                 '</div>',
                 '</div>',
                 '<div class="layui-form-item" style="margin-bottom:1px!important;">',
                 '<label class="layui-form-label Bottom" style="margin-bottom:0px!important;width:100%!important;text-align:left!important; cursor:pointer;" onclick="wfFunction.ShowRecord()"><i class="layui-icon layui-colla-icon" style="position:absolute;top:9px;" id="IconShow"></i><i class="layui-icon layui-colla-icon" id="IconHide" style="display:none;position:absolute;top:8px;"></i><span style="margin-left:20px;">审批记录</span></label>',
                 '</div>',
                 '<div class="WFLog">',
                 '</div>',
                 '</div>',
                 '</div>',
        ].join("");
        dom.append(html);
    },

    //获取工作流当前状态
    GetCurrentActivity: function () {
        $.ajax({
            url: "/WebApi/WorkFlow/GetCurrentActivity?activityid=" + GetUrlParm("ACTIVITY_ID"),
            async: false,
            success: function (data) {
                if (data.IsCurrentActivity == 0) {
                    $("#top-bar").hide();
                    $(".WorkFlow").hide();
                    $("#Save").hide();
                    $(".Opinion").hide();
                    $("#main").css({ "height": $("#main").height() + 58 });
                }
                else {
                    if (data.IsExitNode) {
                        $(".WorkFlow").hide();
                        $("#NextUsers").removeAttr("lay-verify");
                        $(".WFActivityName").css({ color: "red" })
                    }
                    $(".ActivityDCode").val(data.ActivityDCode);
                    $(".CurrentActivity").text(data.ActivityName);
                    $(".ActivityName").val(data.ActivityName);
                    $(".WorkItemID").val(GetUrlParm("TaskID"));
                    $(".WFActivityName").text("（" + data.ActivityName + "）");
                    $(".ActivityInstanceID").val(data.ActivityInstanceID);
                    $(".ProcessInstanceID").val(data.ProcessInstanceID)
                    $(".IsExitNode").val(data.IsExitNode);

                }
            }
        })
    },

    //获取下一节点
    GetNextStep: function () {
        layui.use('form', function () {
            var form = layui.form;
            //判断是否为新增数据
            if (GetUrlParm("ACTIVITY_ID") != null) {
                $.ajax({
                    url: "/WebApi/WorkFlow/GetNextActivitiesList?activityID=" + GetUrlParm("ACTIVITY_ID") + "",
                    async:false,
                    success: function (data) {
                        if (data.length > 0 ) {
                            for (var i = 0; i < data.length; i++) {
                                $(".NextStep").append("<option value=" + i + " id=" + JSON.stringify(data[i]) + ">" + data[i].nodeName + "</option>")
                            }
                            form.render();
                            if (data.length > 0) {
                                //默认选择第一个选项;
                                wfFunction.ChangeUsers($(".NextStep option:eq(0)")[0])
                            }
                        }
                    },
                })
            }
            else {
                $.ajax({
                    url: "/WebApi/WorkFlow/GetNextActivitiesList?processDKey=" + $(".ProcessDinfineKey").val(),
                    async:false,
                    success: function (data) {
                        if (data.length > 0) {
                            for (var i = 0; i < data.length; i++) {
                                $(".NextStep").append("<option value=" + i + " id=" + JSON.stringify(data[i]) + ">" + data[i].nodeName + "</option>")
                            }
                            form.render();
                            if (data.length > 0) {
                                //默认选择第一个选项;
                                wfFunction.ChangeUsers($(".NextStep option:eq(0)")[0])
                            }
                        }
                    },
                })
            }
        })
    },

    //选择下一节点改变节点人员
    ChangeUsers: function (obj) {

        $("#NextUsers").val("");

        
        var activity = getUrlParam("ACTIVITY_ID");
        if (activity != null) {
            $.ajax({
                url: "/WebApi/WorkFlow/GetNextActivityResource?activityID=" + activity + "&nextActivityKey=" + JSON.parse(obj.id).activityKey + "&AppName=" + $(".AppName").val() + "",
                async: false,
                success: function (data) {
                    UserData = data;
                }
            })
        }
        else {
            $.ajax({
                url: "/WebApi/WorkFlow/GetNextActivityResource?processDKey=" + $(".ProcessDinfineKey").val() + "&nextActivityKey=" + JSON.parse(obj.id).activityKey + "&AppName=" + $(".AppName").val() + "",
                async: false,
                success: function (data) {
                    UserData = data;
                }
            })
        }

        if (UserData.length == 1) {
            $(".NextUsers").removeAttr("onclick");
            $(".NextUsers").val(UserData[0].DisplayName);
            $(".NextUsersFullPath").val(UserData[0].AllPathName);
            $(".NextUsersGUID").val(UserData[0].GUID);
        }

        if (obj.id != "") {
            $(".selectSingleUser").val("" + JSON.parse(obj.id).selectSingleUser + ""); 
            $(".defaultSelect").val("" + JSON.parse(obj.id).defaultSelect + "");
            $(".NextActivity").val("" + JSON.parse(obj.id).activityKey + "");
            $(".NextStepActivity").val("" + JSON.parse(obj.id).nodeName + "");
            if (GetUrlParm("ACTIVITY_ID") == null) {
                $(".parentID").val("" + $(".ProcessDinfineKey").val() + "," + JSON.parse(obj.id).activityKey + "," + $(".AppName").val() + ",true")
            }
            else {
                $(".parentID").val("" + GetUrlParm('ACTIVITY_ID') + "," + JSON.parse(obj.id).activityKey + "," + $(".AppName").val() + ",false")
            }
            if (getUrlParam("ACTIVITY_ID") != null) {
                $(".NextUsers").attr("lay-verify", "required");
            }
        }
        else {
            $(".parentID").val("")
        }
    },

    //选择下一节点人员
    ChooseNextUser: function () {
        $(".id").val($(".parentID").val());
        $(".item").val("#NextUsers/DisplayName, .NextUsersFullPath/AllPathName, .NextUsersGUID/GUID");
        if (navigator.userAgent.match(/mobile/i) || $(window).width() < 530) {
            wfFunction.OpenChooseMB("人员选择", $(window).width(), $(window).height(), "Users", false)
        }
        else {
            wfFunction.OpenChooseW7("人员选择", "Users_WF", false)
        }
    },

    //PC工作流人员选择
    OpenChooseW7: function (title, page, back) {
        $('#Choose').window({
            title: title,
            width: 700,
            height: 500,
            content: '<iframe scrolling="yes"  frameborder="0"  src="/Admin/Choose/' + page + '?Back=' + back + '" style="width:100%;height:98%;"></iframe> ',
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
    },

    //手机工作流人员选择
    OpenChooseMB: function (title, width, height, page, back) {
        $('#Choose').window({
            title: title,
            width: width,
            height: height-50,
            content: '<iframe scrolling="yes"  frameborder="0"  src="/Mobile/Choose/' + page + '?Back=' + back + '" style="width:100%;height:98%;"></iframe> ',
            modal: true,
            minimizable: false,
            maximizable: false,
            top:0,
            shadow: false,
            cache: false,
            closed: false,
            collapsible: false,
            resizable: false,
            draggable: false,
        });
    },

    //获取审批记录
    GetRecord: function () {
        //添加当前流程的审批记录
        $.ajax({
            url: "/WebApi/WorkFlow/GetWFLog?ResourceID=" + GetUrlParm("id"),
            async: false,
            success: function (data) {
                var wflog = $(".WFLog");
                for (var i = data.length - 1; i >= 0; i--) {
                    var _self = data[i];
                    var html = '<div class="layui-form-item" style="margin-bottom:1px!important;">';
                    html += '<label class="layui-form-label" style="width:100%!important; text-align:left!important;">';
                    html += _self.Title + "：";
                    if (_self.Opinion != "") {
                        html += _self.Opinion + "&emsp;";
                    }
                    html += _self.OperatorUName + "&emsp;";
                    html += _self.DeliverTime.replace("T", " ") + "&emsp;";
                    html += _self.AppendOpinion
                    html += '</div>';
                    wflog.append(html);
                }
            }
        })

        $.ajax({
            url: "/WebApi/WorkFlow/GetCurrentUser?ResourceID=" + GetUrlParm("id"),
            async: false,
            success: function (data) {
                var wflog = $(".WFLog");
                for (var i = 0; i < data.length; i++) {
                    var _self = data[i];
                    var html = '<div class="layui-form-item" style="margin-bottom:1px!important;">';
                    html += '<label class="layui-form-label" style="width:100%!important; text-align:left!important;">';
                    html += "待办环节：" + _self.Purpose + "&emsp;";
                    html += "待办人：" + _self.UName + "&emsp;";
                    html += '</div>';
                    wflog.prepend(html);
                }
            }
        })
    },

    //显示/隐藏审批记录
    ShowRecord: function () {
        $(".WFLog").toggle();
        $("#IconShow").toggle();
        $("#IconHide").toggle();
        //if ($(".WFLog").css("display") == "none") {
        //    $(".WFLog").show();
        //    $("#IconShow").show();
        //    $("#IconHide").hide();
        //}
        //else {
        //    $(".WFLog").hide();
        //    $("#IconShow").hide();
        //    $("#IconHide").show();
        //}
    }, 

    //审批页面 控件的控制
    WFControl: function () {
        $(".WFlow input").removeAttr("disabled");
        $(".WFlow select").removeAttr("disabled");
        if(UserData.length!=1)
            $(".NextUsers").attr("onclick", "wfFunction.ChooseNextUser()");
        
    }

}

$(function () {
    $.fn.SetWorkFlow = function (options)
    {
        wfParms = $.extend(wfParms, options);
        var _this = $(this);
        for (var i in wfParms)
        {
            _this.append("<input type='hidden' name=" + i + " class=" + i + " value='" + wfParms[i] + "' />");
        }


        if ($(".id").size() == 0) {
            _this.append("<input type='hidden' class='id' />")
        }
        if ($(".item").size() == 0) {
            _this.append("<input type='hidden' class='item' />")
        }
        if ($("#Choose").size() == 0) {
            _this.append("<div id='Choose'></div>");
        }
       
        wfFunction.AddWorkFlowDom(_this);
        $(".layui-colla-content").addClass("layui-show");

        if (GetUrlParm("ACTIVITY_ID") != null) { 
            $("#toolbar-del").remove();
            wfFunction.GetCurrentActivity();
        }
        wfFunction.GetRecord();
        wfFunction.GetNextStep();

        layui.use(["form"], function () {
            var form = layui.form;

            form.on("radio(Opinion)", function (data) {
                switch (data.value) {
                    case "不同意":
                        $(".IsProcessInstanceEnd").val("true");
                        $(".IsWithdraw").val("false");
                        $(".WorkFlow").hide();
                        $("#NextUsers").removeAttr("lay-verify");
                        break;
                    case "退回":
                        $(".IsProcessInstanceEnd").val("false");
                        $(".IsWithdraw").val("true");
                        $(".WorkFlow").hide();
                        $("#NextUsers").removeAttr("lay-verify");
                        break;
                    default:
                        if ($(".IsExitNode").val() == "false") {
                            $(".WorkFlow").show();
                            $("#NextUsers").attr("lay-verify");
                        }
                        $(".IsProcessInstanceEnd").val("false");
                        $(".IsWithdraw").val("false");
                }
            })

            form.on("select(NextStep)", function (data) {
                wfFunction.ChangeUsers($(".NextStep option:eq(" + data.value + ")")[0])
            })
             
        })

        if ($(".ActivityDCode").val() == "N1" || getUrlParam("id") == null) {
            $(".Opinion").hide();
        }
        else {
            $(".Opinion").show();
        }
    }
})
  
function close() {
    $("#Choose").window("close");
}

//获取url中参数
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name.toLowerCase() + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).toLowerCase().match(reg);
    if (r != null) return unescape(r[2]); return null; 
}

