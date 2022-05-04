
function DataValidation_UpdateCharacterCounter(e) {
    var target = $(e.target);
    if (target.length) {
        let textLength = target.val().length;
        let maxLength = $(e.target).data("val-maxlength-max");
        let name = $(e.target).attr('name');
        let label = $(`p[for="${name}"]`);
        let pct = (maxLength - textLength) / maxLength;

        label.html(`${maxLength - textLength}`);

        if (pct < 0.1) {
            label.addClass('text-danger');
            label.removeClass('text-warning');
        } else if (pct < 0.25) {
            label.removeClass('text-danger');
            label.addClass('text-warning');
        } else {
            label.removeClass('text-danger');
            label.removeClass('text-warning');
        }
    }
}

$(function () {

    $('.custom-validation').each((i,e) => {
        if (!e.target) { e = { target: e }; }
        DataValidation_UpdateCharacterCounter(e);
    });
    $('.custom-validation').on('paste', (e) => { DataValidation_UpdateCharacterCounter(e); });
    $('.custom-validation').on('change', (e) => { DataValidation_UpdateCharacterCounter(e); });
    $('.custom-validation').on('keyup', (e) => { DataValidation_UpdateCharacterCounter(e); });


    $('div.toast').each((i, e) => {
        let t = new bootstrap.Toast(e, {
            animation: $(e).data('bs-animation'),
            autohide: $(e).data('bs-autohide'),
            delay: $(e).data('bs-delay'),
        });

        if ($(e).hasClass('show'))
            t.show();
    });

    $('button.btn-close[data-bs-dismiss="toast"]').on('click', (e) => {
        $(e.target).closest('div.toast').each((i, e) => { $(e).toast('hide'); });
    });

    

});