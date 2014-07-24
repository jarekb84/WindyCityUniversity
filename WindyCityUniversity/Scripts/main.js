(function() {
    $(document).ready(function() {
        $('.expandClassmates').click(function() {
            $(this).closest('tr').find('.remainingClassmates').slideToggle();

            $(this).toggleClass('fa-plus');
            $(this).toggleClass('fa-minus');
        });
    });
}());
