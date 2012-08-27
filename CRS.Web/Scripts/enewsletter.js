(function ($) {
  Drupal.behaviors.enewsletter = {
    attach: function () {
      $('#newsletter-signup').submit(function () {
        var filter = /^([a-zA-Z0-9_.-])+@(([a-zA-Z0-9-])+.)+([a-zA-Z0-9]{2,4})+$/;
        var email = $('#newsletter-signup input[type="text"]');
        if (!filter.test(email.val())) {
          alert(Drupal.t('Nhập đúng địa chỉ email'));
          email.focus();
        } else {
          $.getJSON($('#newsletter-signup').attr('action') + email.val(), function (data) {
            if(data.message != undefined) {
              alert(data.message);
              email.focus();
            } else {
              window.location = data.redirect;
            }
          });
        }
        return false;
      });
    }
  };
})(jQuery);