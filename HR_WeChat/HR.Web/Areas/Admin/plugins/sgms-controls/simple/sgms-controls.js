(function ($) {
    var urlPrefix = "/Admin/";
    //#region 快速搜索

    ///params
    /// 参数名          说明                         默认值
    /// delay           setInterval 的第二个参数      100
    /// maxCount        计数器                        9
    ///gridJqObj    datagrid                      null
    $.fn.quickSearch = function (params) {
        var dParams = {
            delay: 100,
            maxCount: 9,
            gridJqObj: null
        }
        params = $.extend(dParams, params || {});
        var self = this;

        var datagridKey = $.fn.quickSearch.methods["getDatagridKey"](params.gridJqObj);
        self.val(getCookie(datagridKey + "searchStr") || "");
        self.data("datagrid", params.gridJqObj)
        self.keyup(function () {
            $.fn.quickSearch.methods["onkeyup"](self, params)
        });
    }

    $.fn.quickSearch.methods = {
        onkeyup: function (jqObj, params) {
            if (jqObj.data("timer") != null) {
                clearInterval(jqObj.data("timer"));
            }
            jqObj.data("count", 0);
            jqObj.data("timer", setInterval(function () {
                $.fn.quickSearch.methods["interval"](jqObj, params)
            }, params.delay));
        },
        interval: function (jqObj, params) {
            var count = jqObj.data("count");
            if (count > params.maxCount) {
                clearInterval(jqObj.data("timer"));
                jqObj.data("count", 0);
                $.fn.quickSearch.methods["search"](jqObj);
            }
            count++;
            jqObj.data("count", count)
        },
        search: function (jqObj) {
            var gridJqObj = jqObj.data("datagrid");
            if (!gridJqObj || gridJqObj.length == 0) return;

            //存cookies的key
            var datagridKey = $.fn.quickSearch.methods["getDatagridKey"](gridJqObj);

            var searchBoxStr = jqObj.val();

            setCookie(datagridKey + "searchStr", searchBoxStr);

            var options = gridJqObj.datagrid("options").queryParams;
            options = $.extend(options, {
                searchstr: searchBoxStr
            })
            gridJqObj.datagrid('load', options);
        },
        //存cookies的key
        getDatagridKey: function (gridJqObj) {
            var datagridKey = location.href/*.replace(/.*(?:\/|\\)/g, "")*/.replace(/\?.*/, "");
            datagridKey += gridJqObj.attr("key") || "";
            return datagridKey;
        }
    }

    //#endregion 快速搜索

    //#region 百度地图相关

    $.fn.baiduMap = {
        isJsLoaded: false,
        key: "y8fTv8qe9Gt0SnGYZemRtQei",
        geocodingUrl: "http://api.map.baidu.com/geocoder/v2/?ak={ak}&callback=renderReverse&location={latlng}&output=json&pois=1",
        geodecodingUrl: "http://api.map.baidu.com/geocoder/v2/?ak={ak}&callback=renderOption&output=json&address={address}",
        curAddress: "",
        curAddressJson: null,
        curPoint: null,
        methods: {
            reloadPoint: function (jqObj, params) {
                var methods = $.fn.baiduMap.privateMethods
                var map = jqObj.data("map");
                if (map == null) {
                    return;
                }
                methods.setPointByParams(jqObj, map, params);
            }
        },
        privateMethods: {
            loadBaiduMap: function (jqObj, params, windowID) {
                var methods = $.fn.baiduMap.privateMethods
                var map = new BMap.Map(windowID);
                jqObj.data("map", map)
                methods.setPointByParams(jqObj, map, params);
            },

            setPointByParams: function (jqObj, map, params) {
                var methods = $.fn.baiduMap.privateMethods
                if (params.point) {
                    methods.setPoint(jqObj, map, params.point, 12);
                } else if (params.address) {
                    var geodecodingUrl = $.fn.baiduMap.geodecodingUrl.replace("{ak}", $.fn.baiduMap.key).replace("{address}", params.address)
                    loadJs(geodecodingUrl, function () {
                        $(this).remove();
                        var point = $.fn.baiduMap.curPoint
                        methods.setPoint(jqObj, map, point, 12);
                    })
                } else {
                    var geodecodingUrl = $.fn.baiduMap.geodecodingUrl.replace("{ak}", $.fn.baiduMap.key).replace("{address}", "浙江省宁波市鄞州区")
                    loadJs(geodecodingUrl, function () {
                        $(this).remove();
                        var point = $.fn.baiduMap.curPoint
                        methods.setPoint(jqObj, map, point, 12);
                    })

                    /*var myCity = new BMap.LocalCity();
                    myCity.get(function (result) {
                    methods.setPoint(map, result.center, result.level);
                    });*/
                }
            },

            setPoint: function (jqObj, map, point, level) {
                var methods = $.fn.baiduMap.privateMethods
                level = level || map.getZoom() || 15;
                if (typeof (point) == "string") {
                    var pointArr = params.point.split(",");
                    point = new BMap.Point(parseFloat(pointArr[0]), parseFloat(pointArr[1]))
                }
                var marker = methods.getSingleMarker(map);
                if (!marker) {
                    marker = new BMap.Marker(point);
                    map.addOverlay(marker);
                    marker.enableDragging();
                } else {
                    marker.setPosition(point)
                }
                jqObj.data("point", point);
                map.centerAndZoom(point, level)
            },

            getSingleMarker: function (map) {
                var overlays = map.getOverlays();
                for (var i = 0, c = overlays.length; i < c; i++) {
                    var elem = overlays[i];
                    if (elem instanceof BMap.Marker) {
                        return elem
                    }
                }
                return null
            }
        }
    }

    //#region 获取位置

    ///params
    /// 参数名          说明                         默认值
    /*/// addressJqObj   用于赋值地理位置的元素        null
    /// lngJqObj       用于赋值经度的元素            null
    /// latJqObj       用于赋值纬度的元素            null*/
    /// windowJqObj    弹出百度地图的窗口            null 自动创建
    /// dataGeted      数据获取到事件
    $.fn.getLocation = function (params, opts) {
        var pMethods = $.fn.baiduMap.privateMethods
        var self = this;
        if (typeof (params) == "string") {
            $.fn.baiduMap.methods[params](self, opts)
            return;
        }
        //var methods = $.fn.getLocation.methods;
        var dParams = {
            /*addressJqObj: null,
            lngJqObj: null,
            latJqObj: null,*/
            point: null,
            address: null,
            windowJqObj: null,
            dataGeted: function () {
            }
        }
        params = $.extend(dParams, params || {});

        var isJsLoaded = $.fn.baiduMap.isJsLoaded;
        if (!isJsLoaded) {
            loadJs("http://api.map.baidu.com/api?v=2.0&ak=" + $.fn.baiduMap.key + "&callback=baiduMapJsLoad",
                function () {
                    //clearInterval
                    var t = setInterval(function () {
                        if ($.fn.baiduMap.isJsLoaded) {
                            clearInterval(t);
                            initEvent(params);
                        }
                    }, 100);
                }
            )

        } else {
            initEvent();
        }

        var windowID = "";
        var mapLoaded = false;
        //var marker = null;

        function initEvent(params) {
            var windowJqObj = params.window || $("<div />");
            if (!windowJqObj.attr("id")) {
                windowJqObj.attr("id", "id_" + guid());
            }
            windowID = windowJqObj.attr("id")
            windowJqObj.dialog({
                width: 800,
                height: 600,
                modal: true,
                closed: true,
                title: "请选择地理位置",
                buttons: [{
                    text: '　　选　择　　',
                    handler: function () {
                        var marker = pMethods.getSingleMarker(self.data("map"));
                        var point = marker.getPosition();
                        var geocodingUrl = $.fn.baiduMap.geocodingUrl.replace("{ak}", $.fn.baiduMap.key).replace("{latlng}", point.lat + "," + point.lng)
                        loadJs(geocodingUrl, function () {
                            $(this).remove();
                            //params.addressJqObj.val($.fn.baiduMap.curAddress)
                            params.dataGeted(point.lng, point.lat, $.fn.baiduMap.curAddress, $.fn.baiduMap.curAddressJson);
                            windowJqObj.window('close')
                        })
                    }
                }, {
                    text: '　　取　消　　',
                    handler: function () {
                        windowJqObj.window('close')
                    }
                }]
            });

            var btns = $("#" + windowID).next(".dialog-button").find(".l-btn-left");
            btns.eq(0).addClass("btn-primary")
            btns.eq(1).addClass("btn-default")


            self.click(function () {
                windowJqObj.window('open')
                if (!mapLoaded) {
                    mapLoaded = true;
                    pMethods.loadBaiduMap(self, params, windowID);
                }
                var map = self.data("map")
                var point = self.data("point");
                if (point) {
                    map.centerAndZoom(point, map.getZoom())
                    self.data("point", null)
                }
            })
        }
    }

    //#endregion 获取位置

    //#endregion 百度地图相关

    $.fn.numberSelect = function (param) {
        var dParam = {
            min: 1,
            max: 999,
            formatterText: function (value) {
                return value
            }
        }
        this.empty();

        dParam = $.extend(dParam, param);

        for (var i = dParam.min; i < dParam.max; i++) {
            this.append("<option value='" + i + "'>" + dParam.formatterText(i) + "</option>")
        }

        var valAttr = this.attr("val");
        if (valAttr) {
            this.val(valAttr)
        }
    }

    $.fn.sortSelect = function (param) {
        var dParam = {
            min: 1,
            max: 999,
            formatterValue: function (value) {
                if (value < 10) {
                    value = "00" + value;
                } else if (value < 100) {
                    value = "0" + value;
                }
                return value
            },
            formatterText: function (value) {
                if (value < 10) {
                    value = "00" + value;
                } else if (value < 100) {
                    value = "0" + value;
                }
                return value
            }
        }
        this.empty();
        dParam = $.extend(dParam, param);

        for (var i = dParam.min; i < dParam.max; i++) {
            this.append("<option value='" + dParam.formatterValue(i) + "'>" + dParam.formatterText(i) + "</option>")
        }

        var valAttr = this.attr("val");
        if (valAttr) {
            valAttr = "00" + valAttr;
            valAttr = valAttr.substr(valAttr.length - 3, valAttr.length);
            this.val(valAttr)
        }
    }

    $.fn.select = function (param) {
        var dParam = {
            data: [],
            textField: "Text",
            valueField: "Value",
            nullable: false,
            nullableText: "--请选择--"
        }
        this.empty();
        dParam = $.extend(dParam, param);
        if (dParam.nullable) {
            this.append("<option value=''>" + dParam.nullableText + "</option>");
        }
        var data = dParam.data
        var c = data.length
        for (var i = 0; i < c; i++) {
            this.append("<option value='" + data[i][dParam.valueField] + "'>" + data[i][dParam.textField] + "</option>")
        }

        if (this.attr("val")) {
            this.val(this.attr("val"))
        }
    }

    $.fn.ueditor = function (params) {
        var id = this.attr("id")
        if (!id) {
            id = "id" + guid()
            this.attr("id", id);
        }
        var dParam = {
            initialFrameWidth: this.parent().width(),
            initialFrameHeight: 150
        }
        params = $.extend(dParam, params);
        var editor = UE.getEditor(id, params);
    }

    $.fn.ueditorUpImage = function () {
        var self = this;
        self.attr("readonly", "readonly")
        var id = "id" + guid();
        self.after("<div style='display:none'><textarea id='" + id + "'></textarea></div>");
        self.wrap('<div class="input-group"></div>')

        var showJqObj = $('<a class="input-group-addon"><i class="fa fa-eye"></i></a>')
        self.before(showJqObj)
        showJqObj.lightbox();
        if (trim(self.val()) != "") {
            showJqObj.attr("href", self.val());
        }


        var editor = UE.getEditor(id, {
            initialFrameWidth: 100,
            initialFrameHeight: 10,
            toolbars: [["insertimage"]]
        });
        editor.ready(function () {
            /*editor.setDisabled();*/
            editor.hide();
            editor.addListener('beforeInsertImage', function (t, arg) {
                //将地址赋值给相应的input,只去第一张图片的路径
                self.val(arg[0].src);
                showJqObj.attr("href", arg[0].src)
            });
        })

        var uploadJqObj = $('<a class="input-group-addon"><i class="fa fa-upload"></i></a>')
        self.after(uploadJqObj);
        uploadJqObj.click(function () {
            var myImage = editor.getDialog("insertimage");
            myImage.open();
        });

    }

    $.fn.my97DatePicker = function (param) {
        param = param || {}
        this.addClass("Wdate");
        this.click(function () {
            WdatePicker(param)
        })
    }

    ///insertRowTemplate 插入行模板
    $.fn.childTable = function (param, opt) {
        var self = this;
        if (param == "setToHidden") {
            self.datagrid("endEdit", self.data("curIndex"));
            var changes = {
                inserted: self.datagrid("getChanges", "inserted"),
                deleted: self.datagrid("getChanges", "deleted"),
                updated: self.datagrid("getChanges", "updated")
            }
            $(opt).val(JSON.stringify(changes))
            return;
        }
        var dParam = {
            fitColumns: true,
            striped: true,
            singleSelect: true,
            onSelect: function (rowIndex, rowData) {
                self.datagrid("endEdit", self.data("curIndex"))
                self.data("curIndex", rowIndex);
                self.datagrid("beginEdit", rowIndex)
            },
            toolbar: [{
                iconCls: 'icon-add',
                handler: function () {
                    self.datagrid("endEdit", self.data("curIndex"))
                    self.datagrid("insertRow", {
                        index: 0,
                        row: param.insertRowTemplate || {}
                    })
                    self.data("curIndex", 0);
                    self.datagrid("beginEdit", 0)

                    if (param.onCreate) {
                        param.onCreate(1);
                    }
                }

            },
            {
                iconCls: 'icon-remove',
                handler: function () {
                    var selected = self.datagrid("getSelected");
                    if (!selected) {
                        alert("你没有选中任何项")
                        return;
                    }
                    self.datagrid("deleteRow", (self.datagrid("getRowIndex", selected)))
                    if (param.onDelete) {

                        param.onDelete(selected);
                    }
                }
            },
            {
                iconCls: 'icon-save',
                handler: function () {
                    self.datagrid("endEdit", self.data("curIndex"));
                    //self.datagrid("acceptChanges")
                }
            }]
        }
        param = $.extend(dParam, param);

        if (param.fkID && param.tableName) {
            param.url = urlPrefix + param.tableName + "/GetData";
            param.queryParams = $.extend(param.queryParams || {}, { "advanced": '[{"Field":"' + param.fkName + '", "IsRange":false,"SearchStr":"' + param.fkID + '"}]' });
        }

        self.datagrid(param);
        var insertRowTemplate = param.insertRowTemplate;
        if (!insertRowTemplate) {
            var columns = self.datagrid("options").columns[0];
            for (var i = 0, c = columns.length; i < c; i++) {
                var elem = columns[i];

            }
        }
    }
})(jQuery);

(function ($) {
    $.fn.extEUCombotree = function (param) {
        var dParam = {
            onLoadSuccess: function () {
                if (self.combotree("getValue") == "") {
                    var tree = self.combotree('tree');
                    self.combotree("setValue", tree.tree("getRoot")["id"])
                }
            }
        };
        param = $.extend(dParam, param || {});
        var self = this;
        self.combotree(param)
    }
})(jQuery);

function renderReverse(data) {
    try {
        $.fn.baiduMap.curAddress = data.result.formatted_address
        $.fn.baiduMap.curAddressJson = data
    } catch (e) {
    }
}

function renderOption(data) {
    try {
        $.fn.baiduMap.curPoint = new BMap.Point(data.result.location.lng, data.result.location.lat);
    } catch (e) {
    }
}

function baiduMapJsLoad() {
    $.fn.baiduMap.isJsLoaded = true;
}

function loadJs(url, callback) {
    var done = false;
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.language = 'javascript';
    script.src = url;
    script.onload = script.onreadystatechange = function () {
        if (!done && (!script.readyState || script.readyState == 'loaded' || script.readyState == 'complete')) {
            done = true;
            script.onload = script.onreadystatechange = null;
            if (callback) {
                callback.call(script);
            }
        }
    }
    document.getElementsByTagName("head")[0].appendChild(script);
}

function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

$.fn.extHighcharts = function (params) {
    var dParams = {
        nameField: "Name",
        groupField: "GroupData",
        xAxisField: "Date",
        valueField: "Total",
        defaultValue: 0,
        highchartsParams: {
            chart: {
                type: 'column'
            },
            title: {
                text: '',
                x: -20 //center
            },
            subtitle: {
                text: '',
                x: -20
            },
            xAxis: {
                /*categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']*/
            },
            yAxis: {
                title: {
                    text: '工时（分钟）'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                valueSuffix: ''
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0
            },
            credits: ""
        }
    }
    params = $.extend(true, dParams, params)
    if (params.title) {
        params.highchartsParams.title.text = params.title
    }
    params.highchartsParams.xAxis.categories = params.data.XAxis
    params.highchartsParams.series = setSeries()

    this.highcharts(params.highchartsParams)

    function setSeries() {
        var nameField = params.nameField;
        var groupField = params.groupField;
        var xAxisField = params.xAxisField;
        var valueField = params.valueField;
        var defaultValue = params.defaultValue;

        var xAxis = params.data.XAxis;
        var data = params.data.Data;
        var result = [];
        for (var i = 0, c = data.length; i < c; i++) {
            var groupInfo = data[i];
            params.highchartsParams.title.text = groupInfo.Title+"（分钟）";
            var groupData = groupInfo[groupField];
            //var groupData = data[i];;

            var seriesData = [];

            for (var j = 0, c1 = xAxis.length; j < c1; j++) {
                var hasValue = false;
                var xColumn = xAxis[j];
                for (var k = 0, c2 = groupData.length; k < c2; k++) {
                    var groupElem = groupData[k] 
                        if (groupElem[xAxisField] == xColumn || groupElem["JobDate"]==xColumn) {
                            seriesData.push(groupElem[valueField])
                            hasValue = true;
                            break;
                        } 
                }
                if (!hasValue) {
                    seriesData.push(defaultValue)
                }
            }
            result.push({
                name: groupInfo[nameField],
                data: seriesData
            })
        }
        return result;
    }
}