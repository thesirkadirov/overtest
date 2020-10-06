import $ from 'jquery';

/*
 * Web application preloader
 * Fade it out on page load
 */

$(window).on('load', function() {
    $('div.row.row-preloader').delay(300).fadeOut('slow');
});

/*
 * Enable Bootstrap features
 */

$('[title]').tooltip();

/*
 * Global navigation bar stylization
 */

$(".navbar-global").css("box-shadow", "none");

// On scroll event handler
$(window).scroll(function (event) {

    if ($(window).scrollTop() > 0)
    {
        // Enable shadow when necessary
        $(".navbar-global").css("box-shadow", "0 0 25px rgba(40, 47, 60, .05), 0 20px 25px rgba(40, 47, 60, .05), 0 3px 4px rgba(40, 47, 60, .05)");
    }
    else
    {
        // Otherwise, disable shadow
        $(".navbar-global").css("box-shadow", "none");
    }

});

$(window).on('load', function () {
    if ($(".navbar.navbar-global-scroller.navbar-scroller ul.navbar-nav").children().length <= 0){
        $(".navbar.navbar-global-scroller.navbar-scroller").hide();
    }
});

/*
 * Shadow text copy
 */

window.CopyTextToClipboard = function (text) {
    let $temp = $('<input>');
    $("body").append($temp);
    $temp.val(text);
    $temp.select();
    document.execCommand("copy");
    $temp.remove();
}

/*
 * Bootstrap forms
 */
$.validator.unobtrusive.options = {
    // validClass: "is-valid",
    errorClass: "is-invalid"
};
$.validator.setDefaults($.validator.unobtrusive.options);