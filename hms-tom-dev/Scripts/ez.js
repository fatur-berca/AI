//import { lchmod } from "fs";

function ez_core() {
    this.Initialize.apply(this, arguments);
}

ez_core.prototype.Initialize = function () {
    this.FormSet = new ez_FormSet(); // legacy support, DO NOT REMOVE

    this.Version = 1.1;
    this.Form = this.FormSet;
}

ez_core.prototype.CollapseNext = function (ele) {
    $(ele).on("click", function () {
        $(this).next().collapse('toggle');
    });
}

ez_core.prototype.log = function (msg, color, background, size, weight) {
    var sty = "color: " + (color || "#000000") + "; background: " + (background || "#FFFFFF") + "; font-size: " + (size || "!inherit") + "; font-weight: " + (weight || "normal");
    console.log("%c" + msg, sty);
}

ez_core.prototype.linq = function (context) {
    return new ezLINQ(context);
}

if ([].includes === undefined) {
    Object.defineProperty(Array.prototype, 'includes', {
        enumerable: false,
        value: function (item) {
            for (var it = 0; it < this.length; it++)
                if (item == this[it])
                    return true;
            return false;
        }
    });
    console.warn("This browser does not support Array.includes!");
}

function ez_isNaN(num) {
    if (Number.isNaN === undefined) {
        return typeof (num) === 'number' && isNaN(num);
    }
    else
        return Number.isNaN(num);
}
function ez_trunc(val) {
    if (ez_isNaN(val))
        return NaN;
    return val > 0 ? Math.floor(val) : Math.ceil(val);
}

/* === Functions - BEGIN */
function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
}

function isValidEmailList(mailist) {
    var spl = mailist.split(/[\s,;]+/);
    var errs = [];
    $.each(spl, function (key, val) {
        if (val != "" && !isValidEmailAddress(val))
            errs[errs.length] = val;
    });
    return errs.length == 0 ? true : errs;
}

function isNotEmpty(value) {
    return value !== undefined && value !== null && value !== "";
}

function dateParse(val, format, fromFormat) {
    var regex = /^\/Date\(([0-9]*)\)\/$/;
    while ((m = regex.exec(val)) !== null) {
        if (m.index === regex.lastIndex) {
            regex.lastIndex++;
        }
        m.forEach(function (match, groupIndex) {
            if (groupIndex == 1) {
                val = moment(parseInt(match, 10)).format(format || "DD/MM/YYYY");
            }
        });
        return val;
    }

    return moment(val, fromFormat).format(format || "DD/MM/YYYY");;
}
/* === Functions - END */

/* === Classes FormSet - BEGIN */
function ez_FormSet() {
    this.Initialize.apply(this, arguments);
}

ez_FormSet.prototype.Initialize = function () {
    this._fields = [];
    this.error = null;
    this.dropDownList = [];
    this.debugMode = false;
    this.dropDownHasBlank = true;
    this.dropDownBlankLabel = "";
}

ez_FormSet.prototype.AddFieldDef = function (fdef) {
    var flds = arguments;
    if (Array.isArray(fdef)) {
        flds = fdef;
    }
    var fsAFDef = this;
    $.each(flds, function (k, def) {
        var spl = def.split(";");
        var fObj = {};
        $.each(spl, function (key, val) {
            var spl2 = val.split(/[:=]/, 2);
            if (spl2.length == 2) {
                var id = spl2[0].trim();
                var cfg = spl2[1].trim();

                if (id == "id" || id == "input")
                    fObj.name = cfg;
                if (id == "class" || id == "type") {
                    fObj.type = cfg;
                    fObj.types = cfg.split(" ");
                }
                if (id == "label")
                    fObj.label = cfg;
                if (id == "json" || id == "db")
                    fObj.jsonId = cfg;
                if (id == "default")
                    fObj.defaultValue = cfg;
                if (id == "format")
                    fObj.format = cfg;
                if (id == "displayFormat")
                    fObj.displayFormat = cfg;
                if (id == "dataType")
                    fObj.dataType = cfg;
            }
        });
        // cleaning
        if (fObj.name === undefined)
            return;
        if (fObj.type === undefined) fObj.type = "";
        if (fObj.types === undefined) fObj.types = [];
        if (fObj.label === undefined) fObj.label = fObj.name;
        if (fObj.jsonId === undefined) fObj.jsonId = fObj.name;

        fsAFDef._fields[fsAFDef._fields.length] = fObj;
        fsAFDef[fObj.name] = fObj;
    });
}

ez_FormSet.prototype.AddField = function (fname) {
    console.warn("ez_FormSet.AddField is deprecated, please use ez_FormSet.AddFieldDef instead.");
    var flds = arguments;
    if (Array.isArray(fname)) {
        flds = fname;
    }
    for (var i = 0; flds.length && i < flds.length; i++) {
        var value = flds[i];
        if (typeof value == "object") {
            if (value.name !== undefined && value.type !== undefined) {
                value.types = val.type.split(" ");
                value.label = value.label || value.name;
                value.jsonId = value.jsonId || value.name;
                this._fields[this._fields.length] = value;
                this[value.name] = value;
            }
            continue;
        }
        if (Array.isArray(value))
            value = (value[0] || "") + ":" + (value[1] || "") + ">" + (value[2] || "") + "#" + (value[3] || "");
        if (value.split === undefined)
            return;
        var fObj = {};
        var spl = value.split(":", 2);
        fObj.name = spl[0];
        spl = (spl[1] || "").split(">", 2);
        fObj.type = spl[0];
        fObj.types = spl[0].split(" ");
        spl = (spl[1] || "").split("#", 2);
        fObj.label = spl[0] === "" ? fObj.name : spl[0];
        fObj.jsonId = spl[1] === "" || spl[1] === undefined ? fObj.name : spl[1];
        this._fields[this._fields.length] = fObj;
        this[fObj.name] = fObj;
    }
}

ez_FormSet.prototype.ClearField = function () {
    this._fields = [];
}

ez_FormSet.prototype.AddProp = function (fname, prop) {
    if (fname.substr(0, 1) == ".") {
        for (var i = 0; i < this._fields.length; i++)
        {
            var fld = this._fields[i];
            if (fld && fld.type.includes(fname.substr(0, 1)))
                this.AddProp(fld.name, prop);
        }
    }
    else {
        var fld = this[fname];
        if (fld !== undefined) {
            fld.type += " " + prop;
            fld.types = fld.type.split(" ");
        }
    }
}

ez_FormSet.prototype.RemoveProp = function (fname, prop) {
    if (fname.substr(0, 1) == ".") {
        for (var i = 0; i < this._fields.length; i++) {
            var fld = this._fields[i];
            if (fld && fld.type.includes(fname.substr(0,1)))
                this.RemoveProp(fld.name, prop);
        }
    }
    else {
        var fld = this[fname];
        if (fld !== undefined) {
            fld.type = fld.type.replace(prop, "").replace("  ", "");
            fld.types = fld.type.split(" ");
        }
    }
}

ez_FormSet.prototype.SetData = function (data, fldType) {
    fldType = fldType || "*";
    for (var i = 0; i < this._fields.length; i++) {
        var fld = this._fields[i];
        if (fld.types.includes("container"))
            continue;
        if (fld.types.includes(fldType) >= 0 || fldType == "*") {
            var val = data[fld.jsonId] || data[fld.label] || data[fld.name];

            if (fld.dataType == "date")
                val = dateParse(val);

            if (val) {
                if (fld.types.includes("dd")) {
                    ez.Form.RefreshDropDown(fld.name, val);
                }
                else {
                    if (fld.displayFormat !== undefined) {
                        console.log(val, fld.displayFormat);
                        if (fld.dataType === 'date')
                            val = dateParse(val, fld.displayFormat);
                    }
                    $("#" + fld.name).val(val).trigger("change");
                }
            }
            else
                $("#" + fld.name).val(null).trigger("change");
        }
    }
    if (this.debugMode) {
        console.log("Data pushed to inputs:", data);
    }
}

ez_FormSet.prototype.GetData = function (fldType) {
    var res = {};
    fldType = fldType || "*";
    for (var i = 0; i < this._fields.length; i++) {
        var fld = this._fields[i];
        if (fld.types.includes("container"))
            continue;
        if (fld.types.includes(fldType) || fldType == "*") {
            var val = $("#" + fld.name).val();
            if (fld.types.includes("num") || fld.types.includes("num+") || fld.types.includes("num-"))
                val = Number(val);
            if (fld.types.includes("bool"))
                val = val !== null && val !== undefined && (val === "1" || val === 1 || val === true || (val.toLowerCase === undefined || val.toLowerCase() === "y" || val.toLowerCase() === "yes" || val.toLowerCase() === "true"));

            if (fld.dataType == "date" && fld.displayFormat !== undefined) {
                var date = moment(val, fld.displayFormat).toDate();
                if (date) {
                    if (fld.format !== undefined) {
                        val = moment(date).format(fld.format);
                    }
                    else
                        val = moment(date).format("DD MMM YYYY");
                }

            }

            if (val === "ez:null") val = null;
            res[fld.jsonId] = val;
        }
    }
    if (this.debugMode) {
        console.log("Data fetched from inputs:", res);
    }
    return res;
}

ez_FormSet.prototype.Clear = function (fldType, triggerChange) {
    fldType = fldType || "*";
    triggerChange = triggerChange === undefined ? true : triggerChange;
    for (var i = 0; i < this._fields.length; i++) {
        var fld = this._fields[i];
        if (fld.types.includes(fldType) || fldType == "*") {
            var def = fld.defaultValue || null;
            $("#" + fld.name).val(def).trigger("change");
            if (fld.types.includes("dd"))
                this.RefreshDropDown(fld.name, def);

        }
    }
}

ez_FormSet.prototype.Enable = function (fldType, value) {
    fldType = fldType || "*";
    value = value === undefined ? true : value;
    for (var i = 0; i < this._fields.length; i++) {
        var fld = this._fields[i];
        if (fld.types.includes(fldType) || fldType == "*") {
            $("#" + fld.name).prop("disabled", value === false);
        }
    }
}

ez_FormSet.prototype.Disable = function (fldType) {
    this.Enable(fldType, false);
}

ez_FormSet.prototype.Show = function (fldType, value) {
    fldType = fldType || "*";
    value = value === undefined ? true : value;
    for (var i = 0; i < this._fields.length; i++) {
        var fld = this._fields[i];
        if (fld.types.includes(fldType) || fldType == "*") {
            console.log(fld.name, value);
            if (value)
                $("#" + fld.name).show();
            else
                $("#" + fld.name).hide();
        }
    }
}

ez_FormSet.prototype.Hide = function (fldType) {
    this.Show(fldType, false);
}

ez_FormSet.prototype.Validate = function () {
    var err = "";
    this.error = null;
    for (var i = 0; i < this._fields.length; i++) {
        var fld = this._fields[i];
        var val = $("#" + fld.name).val();
        if (fld.types.includes("req")) {
            if (val === null || val === undefined || (val.trim !== undefined && val.trim() == "") || val == "ez:null") {
                err += "&bull; " + fld.label + " is required and cannot be empty.<br />\r\n";
                continue;
            }
        }
        if (isNotEmpty(val) && (fld.types.includes("num") || fld.type.includes("num+") || fld.type.includes("num-"))) {
            if (ez_isNaN(val) || val === undefined) {
                err += "&bull; " + fld.label + " must be filled with number.<br />\r\n";
                continue;
            }
            else {
                if (fld.type.includes("num+") && val < 0) {
                    err += "&bull; " + fld.label + " must be filled with positive number.<br />\r\n";
                    continue;
                }
                else if (fld.type.includes("num-") && val < 0) {
                    err += "&bull; " + fld.label + " must be filled with negative number.<br />\r\n";
                    continue;
                }
            }
        }
        if (isNotEmpty(val) && fld.types.includes("mail")) {
            if (!isValidEmailAddress(val)) {
                err += "&bull; " + fld.label + " must be filled with a valid email.<br />\r\n";
                continue;
            }
        }
        if (isNotEmpty(val) && fld.types.includes("mlist")) {
            var v = isValidEmailList(val);
            if (v !== true) {
                $.each(v, function (key, val) {
                    err += "&bull; " + fld.label + "> [" + val + "] is not a valid email.<br />\r\n";
                });
                continue;
            }
        }
    }
    if (err != "") {
        this.error = '<strong style="font-size:larger">Please resolve the following problem(s):</strong><br />\r\n' + err;
        return false;
    }

    return true;
}

ez_FormSet.prototype.SetDropDownDataSource = function (ddId, dataSource) {
    if (this.dropDownList[ddId] && this.dropDownList[ddId].dataSource)
        this.dropDownList[ddId].dataSource = dataSource;
}

ez_FormSet.prototype.RegisterDropDown = function (ddId, dataSource, valueName, labelName, callback, validatedata) {
    this.dropDownList[ddId] = {
        ddId: ddId,
        dataSource: dataSource,
        valueName: valueName,
        labelName: labelName,
        callback: callback,
        validation: validatedata
    };
}

ez_FormSet.prototype.RefreshDropDown = function (ddId, defaultValue, callback, validatedata) {
    var dd = this.dropDownList[ddId];
    if (dd) {
        this.GenerateDropDown(dd.ddId, dd.dataSource, dd.valueName, dd.labelName, defaultValue, false, callback || dd.callback, validatedata || dd.validatedata);
    }
    else
        console.warn("DropDown with id #" + ddId + " not found!");
}

ez_FormSet.prototype.GenerateDropDown = function (ddId, dataSource, valueName, labelName, defaultValue, register, callback, validatedata) {
    var dropDown = $("#" + ddId);
    dropDown.html('');
    if (dropDown.hasClass("select2"))
        dropDown.append('<option></option>');
    if (this.dropDownHasBlank)
        dropDown.append('<option value="ez:null">' + this.dropDownBlankLabel + '</option>');
    valueName = valueName || "value";
    labelName = labelName || "label";

    if (Array.isArray(dataSource)) {
        if (ez && ez.debugMode) {
            if (ez && ez.debugMode) console.log("Dropdown value fetched for #" + ddId + ": ", dataSource, "\r\nAssigned value:", defaultValue);
        }
        $.each(dataSource, function (key, val) {
            if (typeof val === "object")
                dropDown.append('<option value="' + val[valueName] + '">' + val[labelName] + '</option>');
            else
                dropDown.append('<option value="' + val + '">' + val + '</option>');

        });
        if (defaultValue)
            $('#' + ddId).val(defaultValue).trigger("change");
    }
    // should be a url
    else {
        $.ajax({
            url: dataSource,
            method: 'get',
            dataType: 'json',
            cache: false,
            success: function (data) {
                if (ez && ez.debugMode) console.log("Dropdown value fetched for #" + ddId + ": ", data, "\r\nAssigned value:", defaultValue);
                $.each(data, function (key, val) {
                    if (typeof validatedata !== "function" || validatedata(val) !== false)
                        dropDown.append('<option value="' + val[valueName] + '">' + val[labelName] + '</option>');
                });
                if (defaultValue)
                    $('#' + ddId).val(defaultValue).trigger("change");
                if (typeof callback === "function")
                    callback(data, defaultValue);
            },
            error: function () {
                if (typeof callback === "function")
                    callback(null);
            }
        });
    }

    if (this.dropDownList[ddId] === undefined && register !== false)
        this.RegisterDropDown(ddId, dataSource, valueName, labelName, callback, validatedata);
}

ez_FormSet.prototype.NumberOnly = function (id, allowDecimal) {
    id = typeof (id) == "string" ? "#" + id : id;
    $(id).attr('data-allow-decimal', allowDecimal ? 'number': 'integer');
    $(id).keydown(function (e) {
        var charset = "-0123456789";
        if ($(id).attr('data-allow-decimal') == "number")
            charset += ".,e+";
        if (charset.indexOf(e.key) < 0 && 
            e.key != "Backspace" &&
            e.key != "Delete" &&
            e.key != "Tab" &&
            e.key != "Shift" &&
            e.key != "ArrowUp" &&
            e.key != "ArrowDown" &&
            e.key != "ArrowLeft" &&
            e.key != "ArrowRight"
            )
            e.preventDefault();
    });
}

ez_FormSet.prototype.DevHelp = function () {
    alert("Please check your console!");
    ez.log("ezFormSet");
}
/* === Classes FormSet - END */
/* === Classes TabController - BEGIN */
function ez_TabCtl()
{
    this.initialize.apply(this, arguments);
}

ez_TabCtl.prototype.initialize = function (container, type)
{
    type = type || "tab";
    this.cntType = type;
    this.cntPrefix = container;
    this.cntAll = $("#" + container);

    this.cntButtons = $('<ul></ul>').addClass("nav nav-" + this.cntType + "s");
    this.cntContainers = $('<div></div>').addClass("tab-content");
    this.cntAll.html('').append(this.cntButtons).append(this.cntContainers);
    this.clear();

    this.ContentTemplate = null;
    this.events = {};
}

ez_TabCtl.prototype.on = function (evt, callback)
{
    this.events[evt] = this.events[evt] || [];
    this.events[evt].push(callback);
}

ez_TabCtl.prototype.trigger = function (evt, args)
{
    var funcArr = this.events[evt];
    if (funcArr)
        for (var i = 0; i < funcArr.length; i++)
            if (funcArr[i]) funcArr[i].apply(this, args);
}

ez_TabCtl.prototype.clear = function ()
{
    this.cntButtons.html('');
    this.cntContainers.html('');
    this.Pages = [];
    this.Contents = [];
    this.UX = 0;
    if (this.collChanged) this.collChanged();
}

ez_TabCtl.prototype.count = function ()
{
    return this.Pages.length;
}

ez_TabCtl.prototype.selectTab = function (index)
{
    var cnt = this.count();
    if (index >= 0 && index < cnt)
    {
        for (var page in this.Pages)
        {
            this.Pages[page].removeClass("active");
        }
        this.Pages[index].addClass("active");
        for (var page in this.Contents) {
            this.Contents[page].removeClass("active in");
        }
        this.Contents[index].addClass("active in");
    }
}

ez_TabCtl.prototype.close = function (index)
{
    var cnt = this.count();
    if (index >= 0 && index < cnt) {
        var pg = this.Pages[index];
        var cnt = this.Contents[index];
        this.Pages.splice(index, 1);
        this.Contents.splice(index, 1);
        $(pg).remove();
        $(cnt).remove();
        this.trigger("content-deleted", [index]);
        this.trigger("changed", []);

        var selLast = Math.min(this.Pages.length - 1, index);
        this.selectTab(selLast);
    }
}

ez_TabCtl.prototype.closeBtn = function (cb)
{
    var id = $(cb).first().attr("data-id");
    for (var i = 0; i < this.count(); i++)
    {
        var cid = this.Contents[i].attr("id");
        if (cid == id) {
            this.close(i);
            return
        };
    }
}


ez_TabCtl.prototype.createPage = function (name, closeButton)
{
    var cnt = this.UX;
    this.UX++;
    var id = "tb" + this.cntPrefix + cnt;
    cnt = this.count();

    var eleA = $("<a></a>").attr("data-toggle", this.cntType).attr("href", "#" + id);
    var eleBtn = $("<li></li>").attr("id", id + "_btn");
    var eleCnt = $("<div></div>").addClass("tab-pane fade").addClass(cnt == 0 ? "in active" : "")
        .attr("id", id);

    if (this.ContentTemplate)
    {
        var tmplClone = $(this.ContentTemplate).clone();
        $(tmplClone).prop("hidden", false);
        eleCnt.append(tmplClone);
    }
    else if (this.ContentConstructor)
    {
        var tmplClone = this.ContentConstructor();
        eleCnt.append(tmplClone);
    }

    var cb = null;
    name = '<span class="tab-button-name">' + name + "</span>";
    if (closeButton !== false)
    {
        name += '&nbsp;&nbsp;&nbsp;&nbsp;';
        cb = $('<a href="javascript:void()" class="ez-btn-closetab" data-id="' + id + '">&times;</a>');
        cb.on("click", this.closeBtn.bind(this, cb));
        eleA.html(name).append(cb);
    }
    else
        eleA.html(name);
    eleBtn.append(eleA).addClass(cnt == 0 ? "active" : "");

    this.cntButtons.append(eleBtn);
    this.cntContainers.append(eleCnt);

    this.Pages.push(eleBtn);
    this.Contents.push(eleCnt);

    var ret = {};
    ret.Button = eleBtn;
    ret.Content = eleCnt;
    if (cb) ret.CloseButton = cb;

    this.trigger("changed", []);
    this.trigger("content-created", [ret]);
    return ret;
}
/* === Classes TabController - END */

/* === LINQ */
function ezLINQ() {
    this.initialize.apply(this, arguments);
}

ezLINQ.prototype.initialize = function (context) {
    if (context === undefined || (typeof (context) !== "object" && !Array.isArray(context) && !(context._type && context._type == "ezLINQ")))
        throw new Error("Context must be an object or array!");
    if (context._type && context._type == "ezLINQ")
        this.context = context.context;
    else
        this.context = context;
    this._type = "ezLINQ";
}

ezLINQ.prototype.each = function (func) {
    var coll = [];
    for (var key in this.context) {
        var val = func !== undefined ? func(key, this.context[key]) : this.context[key];
        if (val !== undefined) {
            if (val._type && val._type == "ezLINQ")
                val = val.context;
            coll.push(val);
        }
    }
    return new ezLINQ(coll);
}

ezLINQ.prototype.do = function (func) {
    this.each(function (key, val) { func(val); });
    return this;
}

ezLINQ.prototype.where = function (func) {
    return this.each(function (key, val) {
        if (func(val) === true)
            return val;
    });
}

ezLINQ.prototype.select = function (func) {
    return this.each(function (key, val) {
        return func(val);
    });
}

ezLINQ.prototype.flatten = function () {
    var coll = [];
    this.each(function (key, val) {
        if (Array.isArray(val)) {
            var linq = new ezLINQ(val);
            var vals = linq.flatten().getContext();
            for (var k in vals)
                coll.push(vals[k]);
        }
        else {
            coll.push(val);
        }
    });
    return new ezLINQ(coll);
}

ezLINQ.prototype.getContext = function () {
    return this.context;
}

ezLINQ.prototype.count = function () {
    if (Array.isArray(this.context))
        return this.context.length;
    var cnt = 0;
    for (var k in this.context) cnt++;
    return cnt;
}

function linq(context) {
    return new ezLINQ(context);
}
/* === LINQ - END */

var ez = new ez_core();

// ** Library Intro ** //
var ez_intro = "EZ Library v" + ez.Version + " by Zecchan Silverlake";
ez.log(ez_intro, "#DD11AA", "#333", "larger", "bold");

var ezNotif = ezNotif || false;
if (ezNotif) {
    ez.Modals = ezNotif;
}