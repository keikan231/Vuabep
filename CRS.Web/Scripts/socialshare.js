(function ($) {
  Drupal.behaviors.socialshare = {
    attach: function () {
      $('.shared_window').once('fancybox', function () {
        var self = this;
        $(this).fancybox({
          overlayShow: true,
          hideOnContentClick: true,
          titleShow: false,
          showCloseButton: true,
          type: 'ajax'
        });
      });
    }
  };
})(jQuery);