(function ($) {
    "use strict";

    $.fn.customColumns = function (param) {
        var dParam = {
            tableClass: null,
            columnSelectClass: null,
            preLoadColumns: undefined,
            saveUrl: null,
            pageName: null
        }
        return this.each(function (i, o) {
            var self = $(o);
            var partParam = $.extend(true, dParam, param);
            partParam.tableClass = partParam.tableClass || new EasyuiTableClass(partParam);
            partParam.columnSelectClass = partParam.columnSelectClass || new EasyuiColumnSelectClass(partParam);
            self.data("instance", new CustomColumns(partParam))

            self.click(function () {
                partParam.columnSelectClass.show();
            })
        });
    };

    ///自定义列类
    //parameters
    //tableClass               表格的类
    //columnSelectClass        列选择的类
    function CustomColumns(param) {
        var tableClass, columnSelectClass;
        init();

        //初始化
        function init() {
            tableClass = param.tableClass;
            columnSelectClass = param.columnSelectClass;


            //获取表格的列
            var colu = tableClass.getColumns();

            //填充到列选择类中
            columnSelectClass.fillColumns(colu);

            if (param.preloadJson) {
                tableClass.preload(param.preloadJson);
                columnSelectClass.setPreloadFields(param.preloadJson)
                //columnSelectClass.setPreloadFields(param.preLoadFields)
            }

            $(columnSelectClass).on("ok", function (e, fields) {
                //重新加载表格
                tableClass.reload(fields);
            })
        }

        //重新加载表格
        this.reloadTable = function () {
            //获取选中列
            var fields = columnSelectClass.getFields();

            //重新加载表格
            tableClass.reload(fields);
        }
    }

    //EasyuiTableClass类
    function EasyuiTableClass(param) {

        //原始列
        var srcColumns = null;

        //获取列
        this.getColumns = function () {
            var options = param.$datagrid.datagrid("options");

            if (srcColumns == null) {
                srcColumns = [options.frozenColumns[0]||[], options.columns[0]||[]];
            }
            var columns = srcColumns[0].concat(srcColumns[1]);
            var result = [];
            for (var i = 0, c = columns.length; i < c; i++) {
                var elem = columns[i];
                if (elem.checkbox === true || elem.field === "ID") continue;
                result.push(elem)
            }
            return result;
        };

        //重新加载
        this.reload = function (fileds) {
            setColumns(fileds);
            param.$datagrid.datagrid();
        };

        //预加载
        this.preload = function (preloadJson) {
            if (preloadJson) {
                var fileds = [];
                for (var i = 0, c = preloadJson.length; i < c; i++) {
                    var elem = preloadJson[i];
                    if (elem.c) {
                        fileds.push(elem.v);
                    }
                }
                this.reload(fileds);
            }
        };

        //设置列
        function setColumns(fileds) {
            var dgFrozenColumns = [], dgColumns = [];
            var frozenColumns = srcColumns[0];
            var columns = srcColumns[1];
            fileds.splice(0, 0, "ID");
            fileds.splice(0, 0, "ck");
            for (var j = 0, c1 = fileds.length; j < c1; j++) {
                var fieldName = fileds[j];
                var founded = false;
                for (var i = 0, c = frozenColumns.length; i < c; i++) {
                    var elem = frozenColumns[i];
                    if (elem.field == fieldName) {
                        dgFrozenColumns.push(elem);
                        founded = true;
                        break;
                    }
                }
                if (founded) continue;
                for (var i = 0, c = columns.length; i < c; i++) {
                    var elem = columns[i];
                    if (elem.field == fieldName) {
                        dgColumns.push(elem);
                        break;
                    }
                }
            }
            var options = param.$datagrid.datagrid("options");
            options.frozenColumns = [dgFrozenColumns];
            options.columns = [dgColumns]
        }
    }

    //EasyuiColumnSelectClass类
    function EasyuiColumnSelectClass(param) {
        var self = this;
        var $dialog;
        var dParam = {
            dialogJqSelector: null,
            //列容器选择器
            columnContainerJqSelector: "ul",
            //列模板
            columnTemplate: '<li class="col-item"><input type="checkbox" value="${field}" checked /><span>${title}</span></li>'
        }

        param = $.extend(true, dParam, param);

        init();

        //初始化
        function init() {
            $dialog = $(param.dialogJqSelector);
            $dialog.dialog({
                closed: true,
                modal: true,
                buttons: getButtons()
            });
        }

        //设置预加载数据
        this.setPreloadFields = function (preloadJson) {
            /*var $container = $dialog.find(param.columnContainerJqSelector);
            $container.html(html);
            initDraggableEvent($container);*/
            setHtml(preloadJson);

            /*var $input = $container.find("input")
            $input.each(function (i, o) {
                o = $(o);
                var isInFields = false;
                for (var i = 0, c = fields.length; i < c; i++) {
                    if (o.val() == fields[i]) {
                        isInFields = true;
                        o.prop("checked", true);
                        break;
                    }
                }
                if (!isInFields) {
                    o.prop("checked", false);
                }
            })*/
        };

        //设置HTML
        function setHtml(preloadJson) {
            var $parent = $dialog.find(param.columnContainerJqSelector);
            $parent.empty();
            for (var i = 0, c = preloadJson.length; i < c; i++) {
                var elem = preloadJson[i];
                var rowStr = param.columnTemplate.replace("${field}", elem.v).replace("${title}", elem.t);
                if(!elem.c) rowStr=rowStr.replace("checked","")
                $parent.append(rowStr);
            }
            initDraggableEvent($parent);
        }

        //显示对话框
        this.show = function () {
            $dialog.dialog("open");
        };

        //填充列
        this.fillColumns = function (colu) {
            var $parent = $dialog.find(param.columnContainerJqSelector);
            for (var i = 0, c = colu.length; i < c; i++) {
                var elem = colu[i];
                var rowStr = param.columnTemplate.replace("${field}", elem.field).replace("${title}", elem.title);
                $parent.append(rowStr);
            }
            initDraggableEvent($parent);
        };

        //初始化拖动事件
        function initDraggableEvent($parent) {
            var draggableOk = false;
            $parent.find("li").draggable({
                handle: 'span',
                proxy: 'clone',
                onStopDrag: function (e) {
                    if (!draggableOk) {
                        $(this).css("position", "static")
                    }
                    draggableOk = false;
                }
            }).droppable({
                accept: "li",
                onDrop: function (e, s) {
                    $(s).css("position", "static")
                    $(e.target).before($(s))
                    draggableOk = true;
                }
            });
        }

        //获取选中行
        this.getFields = function ($obj) {
            var result = [];
            $obj.each(function (i, o) {
                o = $(o);
                if (!o.prop("checked")) return true;
                result.push(o.val());
            })
            return result;
        }

        //保存字段
        function saveFields($container, fields) {
            /*window.localStorage.setItem("fields", fields);
            window.localStorage.setItem("html", $container.html());*/
            var saveUrl = param.saveUrl
            var pageName = param.pageName
            var json = serialize($container);
            $.post(saveUrl, {
                page: pageName,
                json: json
            }, function (data) {
                if (!data.Success) {
                    console.error("自定义列保存失败原因为：" + data.Message)
                }
            })
        }

        //将字段内容序列化成json
        function serialize($container) {
            var result = [];
            $container.find("li.col-item").each(function (i, o) {
                o = $(o);
                var title = o.find("span").text().replace(/\n|\s/g, "");
                var $input = o.find("input");
                result.push({
                    c: $input.prop("checked"),
                    t: title,
                    v: $input.val()
                })
            });
            return JSON.stringify(result);
        }

        //获取按钮
        function getButtons() {
            return [{
                text: "确定",
                iconCls: 'icon-ok',
                handler: function () {
                    $(".custom-columns-dialog").dialog("close");
                    var input = $dialog.find(param.columnContainerJqSelector + " input")
                    var fields = self.getFields(input)
                    saveFields($dialog.find(param.columnContainerJqSelector), fields);
                    $(self).trigger("ok", [fields]);
                }
            },
            /*{
                text: "恢复默认设置",
                iconCls: 'icon-ok',
                handler: function () {
                    if (!confirm("你确认要恢复默认设置吗？")) return;
                }
            },*/
            {
                text: "全选",
                handler: function () {
                    batchChose(true)
                }
            },
            {
                text: "全不选",
                handler: function () {
                    batchChose(false)
                }
            },
            {
                text: "反选",
                handler: function () {
                    batchChose()
                }
            }];
        }

        //批量选择
        function batchChose(status) {
            var $parent = $dialog.find(param.columnContainerJqSelector);
            if (status !== undefined) {
                $parent.find("input").prop("checked", status)
            } else {
                $parent.find("input").each(function (i, o) {
                    o = $(o);
                    o.prop("checked", !o.prop("checked"));
                })
            }
        }
    }
})(jQuery)