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
$('[data-toggle="tooltip"]').tooltip();

/*
 * Global navigation bar stylization
 */
if ($(".navbar-global").length)
{

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

}