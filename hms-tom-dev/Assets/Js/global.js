(function($) {
    'use strict';

    if (typeof window.$ === 'undefined' || !window.$)
        throw "This app need jQuery";

    /**
     * Document Ready
     */
    $(function () {
        $('.datetimepickerside').datetimepicker({
            format: 'DD/MM/YYYY HH:mm',
            sideBySide: true
        });
        
        // Linked Pickers
        // https://eonasdan.github.io/bootstrap-datetimepicker/
        // begin - no have future date
        $('.datepickerFrom').datetimepicker({
            format: 'DD/MM/YYYY',
            maxDate: new Date,
            useCurrent: false
        });
        $('.datepickerTo').datetimepicker({
            format: 'DD/MM/YYYY',
            maxDate: new Date,
            useCurrent: false //Important! See issue #1075
        });
        $(".datepickerFrom").on("dp.change", function (e) {
            $('.datepickerTo').data("DateTimePicker").minDate(e.date);
        });
        $(".datepickerTo").on("dp.change", function (e) {
            $('.datepickerFrom').data("DateTimePicker").maxDate(e.date);
        });
        // endOf - no have future date

        // begin - have future date
        $('.datepickerFromHaveFuture').datetimepicker({
            format: 'DD/MM/YYYY'
        });
        $('.datepickerToHaveFuture').datetimepicker({
            format: 'DD/MM/YYYY',
            useCurrent: false
        });
        $(".datepickerFromHaveFuture").on("dp.change", function (e) {
            $('.datepickerToHaveFuture').data("DateTimePicker").minDate(e.date);
        });
        $(".datepickerToHaveFuture").on("dp.change", function (e) {
            $('.datepickerFromHaveFuture').data("DateTimePicker").maxDate(e.date);
        });
        // endOf - have future date

        //membandingkan dengan today (hanya select today ke belakang)
        $('.datepickerFromToday').datetimepicker({
            format: 'DD/MM/YYYY',
            maxDate: new Date,
            useCurrent: false
        });

        //membandingkan dengan today (hanya select today ke depan)
        $('.datepickerToToday').datetimepicker({
            format: 'DD/MM/YYYY',
            minDate: new Date,
            useCurrent: false //Important! See issue #1075
        });

        //$(".datepickerToToday").on("dp.change", function (e) {
        //    $('.datepickerFrom').data("DateTimePicker").maxDate(e.date);
        //});

        $(document).on('keyup keydown', '.IsDecimal', function (e) {
            if (e.shiftKey == true) {
                e.preventDefault();
            }

            if ((e.keyCode >= 48 && e.keyCode <= 57)
                || (e.keyCode >= 96 && e.keyCode <= 105)
                || ((e.keyCode == 65 || e.keyCode == 86 || e.keyCode == 67) && (e.ctrlKey === true || e.metaKey === true)) // Allow: Ctrl+A, Ctrl+C, Ctrl+V, Command+A
                || e.keyCode == 8 || e.keyCode == 9 || e.keyCode == 37 || e.keyCode == 39 || e.keyCode == 46 || e.keyCode == 190
                ) {
                // do nothing
            } else {
                e.preventDefault();
            }

            //if a decimal has been added, disable the "."-button
            if ($(this).val().indexOf('.') !== -1 && e.keyCode == 190) {
                e.preventDefault();
            }
        });

        $(document).on('keyup keydown', '.IsInteger', function (e) {
            // Allow: backspace, delete, tab, escape, and enter
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
                // Allow: Ctrl+A, Ctrl+C, Ctrl+V, Command+A
                ((e.keyCode == 65 || e.keyCode == 86 || e.keyCode == 67) && (e.ctrlKey === true || e.metaKey === true)) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        });

        $("section.sidebar ul.menu li.not-active a").each(function (e) {
            $(this).attr("style", "zoom: 1; opacity: 0.25; filter: alpha(opacity = 25); text-shadow: 0 0 8px #000;");
        });

        $(document).on('hidden.bs.collapse', '.filter-deck div.collapse', function (e) {
            $('div.filter-deck').attr('class', 'filter-deck nmb');
        });
        $(document).on('show.bs.collapse', '.filter-deck div.collapse', function (e) {
            $('div.filter-deck').attr('class', 'filter-deck');
        });
    });

})(jQuery);

// alert search
var alertSearchNoResult = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'>There are no results that match your search.</div>",
        size: 'small',
        closeButton: false
    });
};

// alert save
var alertSaveSuccess = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/checked.png' alt=''> Data has been successfully save to the database.</div>",
        size: 'small',
        closeButton: false
    });
};

var alertSaveFail = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/cancel.png' alt=''> Data failed to save to the database.</div>",
        size: 'small',
        closeButton: false
    });
};

var alertSaveErrorDuplicate = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'>You can't save this record,<br>Data already exists in the database.</div>",
        size: 'small',
        closeButton: false
    });
};

// alert delete
var alertDeleteSuccess = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/checked.png' alt=''> Data has been successfully deleted from the database.</div>",
        size: 'small',
        closeButton: false
    });
};

var alertDeleteFail = function () {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/cancel.png' alt=''> Data failed to delete from the database.</div>",
        size: 'small',
        closeButton: false
    });
};

// alert recovery
var alertRecoverySuccess = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/checked.png' alt=''> Data has been successfully recovered from the database.</div>",
        size: 'small',
        closeButton: false
    });
};

var alertRecoveryFail = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/cancel.png' alt=''> Data failed to recover from the database.</div>",
        size: 'small',
        closeButton: false
    });
};

// alert info/warning
var alertPleaseFill = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-left'><img src='assets/images/danger.png' alt=''> Please fill yellow field!</div>",
        size: "small",
        closeButton: false
    });
};

var alertPleaseFillRemark = function () {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-left'><img src='assets/images/danger.png' alt=''> Please fill Remark!</div>",
        size: "small",
        closeButton: false
    });
};

var alertPleaseSelect = function() {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-left'><img src='assets/images/danger.png' alt=''> Please select your data!</div>",
        size: "small",
        closeButton: false
    });
};

var alertNoRecord = function (msg) {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-left'><img src='assets/images/danger.png' alt=''> No record with " + msg + ".</div>",
        size: "medium",
        closeButton: false
    });
};

// alert Custom Report
var alertCrSaveSuccess = function () {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/checked.png' alt=''> Data has been successfully saved to the database.</div>",
        size: 'small',
        closeButton: false
    });
};

var alertCrSaveFail = function () {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-center'><img src='assets/images/cancel.png' alt=''> Data failed to save to the database.</div>",
        size: 'small',
        closeButton: false
    });
};

var alertCrPleaseFill = function () {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-left'><img src='assets/images/danger.png' alt=''> Please fill yellow field!</div>",
        size: "small",
        closeButton: false
    });
};

var alertDate = function () {
    bootbox.alert({
        title: "TOM Notification Center",
        message: "<div class='text-left'><img src='assets/images/danger.png' alt=''> <b>Expired Date</b> should be greater than Effective Date</div>",
        size: "small",
        closeButton: false
    });
};


var testsetButtonEdit = function() {
    return true;
};

function countChar(val, max) {
    var len = val.value.length,
        theInfo = max - len + " characters left.";
    if (len >= max) {
        val.value = val.value.substring(0, max);
        theInfo = "0 characters left.";
    }
    $('#charNum').text(theInfo);
}
