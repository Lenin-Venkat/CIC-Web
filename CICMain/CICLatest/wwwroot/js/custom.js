$(document).ready(function () {

    $('#btnCicLogin').click(function () {
        window.localStorage.removeItem('selectedCompanyLogo');
    });

    $('#pwdhideshow').click(function () {

        if ($(this).hasClass('eye-close')) {
            $(this).removeClass('eye-close');
            $(this).addClass('eye-open');
            $('#psw').attr('type', 'text');
        } else {
            $(this).removeClass('eye-open');
            $(this).addClass('eye-close');
            $('#psw').attr('type', 'password');
        }
    });

    $('#cpwdhideshow').click(function () {

        if ($(this).hasClass('eye-close')) {
            $(this).removeClass('eye-close');
            $(this).addClass('eye-open');
            $('#cpsw').attr('type', 'text');
        } else {
            $(this).removeClass('eye-open');
            $(this).addClass('eye-close');
            $('#cpsw').attr('type', 'password');
        }
    });



    $(".image-box").click(function (event) {
        var previewImg = $(this).children("img");

        $(this)
            .siblings()
            .children("input")
            .trigger("click");

        $(this)
            .siblings()
            .children("input")
            .change(function () {
                var reader = new FileReader();

                reader.onload = function (e) {
                    var urll = e.target.result;
                    $(previewImg).attr("src", urll);
                    previewImg.parent().css("background", "transparent");

                    window.localStorage.setItem('selectedCompanyLogo', urll);

                    previewImg.show();
                    previewImg.siblings("p").hide();
                };
                reader.readAsDataURL(this.files[0]);
            });
    });


    $("#file-upload").change(function () {
        var filepath = this.value;
        var m = filepath.match(/([^\/\\]+)$/);
        var filename = m[1];
        $("#filename").html(filename);
    });



    $('.fileAttachment input[type=file]').each(function () {
        var eventNamespace = '.fileAttachment';
        var labelInputValueAttr = 'data-input-value';
        var $input = $(this);
        var $inputClone = $input.clone(true, true);
        $inputClone.removeClass('empty');
        var $label = $input.next('label');
        var setLabelInputValue = function () {
            var $input = $(this);
            if ($input.val() && $input.val() !== '') {
                $input.removeClass('empty');
                $label.attr(labelInputValueAttr, $input.val().split('\\').pop());
            } else {
                $label.attr(labelInputValueAttr, '');
                $input.addClass('empty');
            }
        }
        if (!$input.val() || $input.val() === '') {
            $input.addClass('empty');
        }
        $label.attr(labelInputValueAttr, '');
        $input.on('change' + eventNamespace, setLabelInputValue);
        $label.on('click' + eventNamespace, function (event) {
            if ($input.val() && $input.val() !== '' && $input.is(':valid')) {
                event.preventDefault();
                $input.remove();
                $label.before($inputClone); // cant just empty val because of ie
                $input = $inputClone;
                if (!$input.val() || $input.val() === '') {
                    $input.addClass('empty');
                }
                $inputClone = $input.clone(true, true);
                $inputClone.removeClass('empty');
                $input.off('change' + eventNamespace);
                $input.on('change' + eventNamespace, setLabelInputValue);
                $label.attr(labelInputValueAttr, '');
            }
        });
    });



    //delete row
    $(".btnDeleteRow").click(function () {
        var rowCount = $(this).closest('table').find('tbody').length;
        if (rowCount > 1) {
            $(this).closest('tbody').remove();
        }
        rowCount--;
        if (rowCount <= 1) {
            $(document).find('.btnDeleteRow').prop('disabled', true);
        }
    });

    //add row
    $(".Addnewrecord").click(function () {
        var table = $(this).closest('table');
        var lastRow = table.find('tbody').last();
        var newRow = lastRow.clone(true, true);
        newRow.find('input, select').val('');
        newRow.insertAfter(lastRow);
        table.find('.btnDeleteRow').removeAttr("disabled");


    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });

    $('#Previewbtn').on('click', function () {
        window.print();
    });

});


// ------------step-wizard-------------

$(document).ready(function () {
    $('.nav-tabs > li a[title]').tooltip();

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target);
        if (target.parent().hasClass('disabled')) {
            return false;
        }

    });

    $(".next-step").click(function (e) {
        var active = $('.wizard .nav-tabs li.active');
        active.next().removeClass('disabled');

        nextTab(active);
    });
    $(".prev-step").click(function (e) {
        var active = $('.wizard .nav-tabs li.active');
        active.next().removeClass('active');
        prevTab(active);
    });
});

function nextTab(elem) {
    $(elem).next().find('a[data-toggle="tab"]').click();
}
function prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
}


$('.nav-tabs').on('click', 'li', function () {
    /*$('.nav-tabs li.active').removeClass('active');*/
    $(this).addClass('active');
});




(function (document, window, index) {
    var inputs = document.querySelectorAll('.inputfile');
    Array.prototype.forEach.call(inputs, function (input) {
        var label = input.nextElementSibling,
            labelVal = label.innerHTML;

        input.addEventListener('change', function (e) {
            var fileName = '';
            if (this.files && this.files.length > 1)
                fileName = (this.getAttribute('data-multiple-caption') || '').replace('{count}', this.files.length);
            else
                fileName = e.target.value.split('\\').pop();

            if (fileName)
                label.querySelector('span').innerHTML = fileName;
            else
                label.innerHTML = labelVal;
        });

        // Firefox bug fix
        input.addEventListener('focus', function () { input.classList.add('has-focus'); });
        input.addEventListener('blur', function () { input.classList.remove('has-focus'); });
    });
}(document, window, 0));


