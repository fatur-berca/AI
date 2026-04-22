$(document).on('keydown', '.inputqty', function (e) {
    if (e.keyCode == 9) {  //tab pressed
        e.preventDefault(); // stops its action
    }
});

$(document).on('dblclick', '.remarks', function (e) {    
    $('#textModalWarning').html('Remarks : <br>'+  $(this).val());
    $('#warningModal').modal('show');
});

$(document).on('keyup', '.inputqty', function (e) {
    var code = e.which;
    var obj = $(this);
    var tmpClassName = obj.attr('class').split(" ");
    var className = tmpClassName[tmpClassName.length - 1];
    if (code == 9) e.preventDefault();

    if (code == 13 || code == 9) {
        var tbody = obj.closest('tbody');
        var nextRow = obj.closest('tr').next();
        //console.log(tbody);
        if (nextRow.length == 1 && nextRow.is('tr')) {
            nextRow.find('.' + className).focus();
        }
        else //jika bukan tr / object tidak ditemukan maka pindah ke row pertama kolom berikutnya
        {
            var NextClass = '';
            var IsMatch = false;
            //console.log(tbody.find('tr:first .inputqty').html());
            /*
            tbody.find('tr:first .inputqty').each(function (e) {
                var tmpClassName = $(this).attr('class').split(" ");
                if (IsMatch) {
                    NextClass = tmpClassName[tmpClassName.length - 1];
                    IsMatch = false;
                }
                IsMatch = className == tmpClassName[tmpClassName.length - 1];
            });
            */
            var idx = InputClasses.indexOf(className) + 1;
            NextClass = idx >= InputClasses.length ? '' : InputClasses[idx];
            //console.log(NextClass);
            if (NextClass != '') {
                tbody.find('tr:first .' + NextClass).focus();
                //var top = tbody.position().top;
                //console.log(top)
                window.scrollTo(0, 500)
            }
        }
    }
});