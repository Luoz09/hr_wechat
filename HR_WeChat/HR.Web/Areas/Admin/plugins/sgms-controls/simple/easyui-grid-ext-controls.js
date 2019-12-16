$.fn.datagrid.defaults.editors = $.extend($.fn.datagrid.defaults.editors, {
    select: {
        init: function (container, options) {
            var select = $("<select />");
            select.select(options);
            container.append(select);
            return select;
            /*var input = $('<input type="text" class="datagrid-editable-input">').appendTo(container);
            return input;*/
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(value);
        },
        resize: function (target, width) {
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    },
    date: {
        init: function (container, options) {
            var input = $('<input type="text" class="Wdate" onclick="WdatePicker({dateFmt:\'yyyy-MM-dd\'})" />');
            container.append(input);
            return input;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(formatterDate(value));
        },
        resize: function (target, width) {
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    },
    datetime: {
        init: function (container, options) {
            var input = $('<input type="text" class="Wdate" onclick="WdatePicker({dateFmt:\'yyyy-MM-dd hh:mm:ss\'})" />');
            container.append(input);
            return input;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(formatterDateTime(value));
        },
        resize: function (target, width) {
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    },
    combogrid: {
        init: function (container, options) {
            var input = $('<input type="text" />');
            container.append(input);
            var isNew = true;
            if (options.onChangeButNotCreate) {
                options.onChange = function (oldValue, newValue) {
                    if (isNew) {
                        isNew = false;
                        return;
                    }
                    options.onChangeButNotCreate.call(this, oldValue, newValue)
                }
            }
            input.combogrid(options);
            return input;
        },
        getValue: function (target) {
            return $(target).combo("getValue");
        },
        setValue: function (target, value) {
            $(target).combo("setValue", value)
        },
        resize: function (target, width) {
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    },
    extText: {
        init: function (container, options) {
            var input = $('<input type="text" style="width:100%" />');
            container.append(input);
            var attrs = options.attrs || {};
            for (var elem in attrs) {
                input.attr(elem, attrs[elem]);
            }
            var events = options.events || {};
            for (var elem in events) {
                input.bind(events, events[elem]);
            }
            return input;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(value)
        },
        resize: function (target, width) {
        }
    },
    imageUpload: {
        init: function (container, options) {
            var input = $('<input type="text" class="form-control" />');
            container.append(input);
            input.ueditorUpImage();
            return input;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(value)
        },
        resize: function (target, width) {
        }
    }
});