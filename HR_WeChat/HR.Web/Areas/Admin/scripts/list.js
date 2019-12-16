function del(ids) {
    $.messager.confirm('删除记录', '确认要删除吗？', function (r) {
        if (r) {
            $.ajax({
                url: url.del,
                cache: false,
                type: "post",
                data: {
                    ids: ids
                },
                error: function () {
                    $.messager.alert('系统错误', '系统错误删除失败！', 'error');
                },
                success: function (data) {
                    if (!data.Success) {
                        $.messager.alert(data.Title, data.Message, 'error');
                    } else {
                        $.messager.alert('删除成功', '删除成功', 'info', function () {
                            if (window.isTreeGrid) {
                                gridJqObj.treegrid("reload");
                            } else {
                                gridJqObj.datagrid("reload");
                            }
                        });
                    }
                }
            })
        }
    });
}

function deleteAll() {
    var checkedItems = gridJqObj.datagrid('getChecked');
    var c = checkedItems.length;
    if (c == 0) {
        $.messager.alert("警告", "你没有选中任何项！", "warning")
        return false;
    } else {
        var ids = [];
        for (var i = 0; i < c; i++) {
            ids.push(checkedItems[i].ID || checkedItems[i].id);
        }
        ids = ids.join(",");
        del(ids);
    }
}


function editRow(id)
{
    if (url && url.edit) {
        if (url.edit.indexOf("?") > 0) {

            location.href = url.edit + "&id=" + id;
        }
        else {
            location.href = url.edit + "?id=" + id;
        }
    }
}

function edit() {
    var checkedItems = gridJqObj.datagrid('getChecked');
    if (checkedItems.length == 0) {
        $.messager.alert("警告", "你没有选中任何项！", "warning")
        return false;
    } else {
        var id = checkedItems[0].ID || checkedItems[0].id;


        if (url.edit.indexOf("?") > 0) {

            location.href = url.edit + "&id=" + id;
        }
        else {
            location.href = url.edit + "?id=" + id;
        }
    }
}

function create() {

    location.href = url.create;
      
}


function formatterID(value, row, index) {
    if (url.edit.indexOf("?") > 0) {
        return "<a href=\"" + url.edit + "&id=" + value + "\" class='oper edit' title='编辑'><i class='fa fa-pencil-square-o'></i></a>" +
        "<a href=\"javascript:del('" + value + "')\" class='oper delete'  title='删除'><i class='fa fa-trash-o'></i></a>";
    }
    else {
        return "<a href=\"" + url.edit + "?id=" + value + "\" class='oper edit' title='编辑'><i class='fa fa-pencil-square-o'></i></a>" +
        "<a href=\"javascript:del('" + value + "')\" class='oper delete'  title='删除'><i class='fa fa-trash-o'></i></a>";
    }
}


//#region Cookies保存相关

//获取当前第几页
function getPageNumber(key) {
    var result = parseInt(getCookie(key + "pageNumber"));
    if (result)
        return result;
    return 1
}

//保存当前第几页
function setPageNumber(key, value) {
    setCookie(key + "pageNumber", value);
}

//获取每页多少条
function getPageSize(key) {
    var result = parseInt(getCookie(key + "pageSize"));
    if (result)
        return result;
    return 15
}

//保存每页多少条
function setPageSize(key, value) {
    setCookie(key + "pageSize", value);
}

//获取排序
function getSort(key) {
    var result = getCookie(key + "sort");
    if (result)
        return result;
    return null
}

//保存排序
function setSort(key, value) {
    setCookie(key + "sort", value);
}

//获取升降序
function getOrder(key) {
    var result = getCookie(key + "order");
    if (result)
        return result;
    return null
}

//保存升降序
function setOrder(key, value) {
    setCookie(key + "order", value);
}

//获取查询字符串
function getSearchStr(key) {
    var result = getCookie(key + "searchStr");
    if (result)
        return result;
    return null
}

//保存查询字符串
function setSearchStr(key, value) {
    setCookie(key + "searchStr", value);
}

//#endregion Cookies保存相关

///获取查询的参数
///param:
///key datagrid的别名用于一个页面多个datagrid
function getQueryParams(key) {
    var datagridKey = getGridKey(key);
    //查询相关的参数
    var queryParams = {
        sort: getSort(datagridKey),
        order: getOrder(datagridKey),
        searchstr: getSearchStr(datagridKey)
    }

    return queryParams;
}

function getGridKey(key) {
    var datagridKey = getPageName();
    datagridKey += key || "";
    return datagridKey
}

function setPagerInfo(datagridKey,gridJqObj) {
    var pager = gridJqObj.datagrid("getPager");
    if (!pager || pager.length == 0) return;
    var opt = $(pager).pagination("options");
    setPageNumber(datagridKey, opt.pageNumber);
    setPageSize(datagridKey, opt.pageSize);
}