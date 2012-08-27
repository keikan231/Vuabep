(function ($) {
  $.fn.extend({
    hint: function () {
      return this.each(function () {
        var self = $(this);
        var currentValue = self.val();
        self.focus(function() {
          if(self.val() == currentValue) {
            self.val('');
          }
        }).blur(function() {
          if(self.val() == '') {
            self.val(currentValue);
          }
        });
//        self.parents('form').submit(function () {
//          if (self.val() == '' || self.val() == currentValue) {
//            alert(Drupal.t('Bạn phải thay đổi nội dung'));
//            return false;
//          }
//        });
        self.parents('form').find('.submit').not('.fancybox_upload').click(function (e) {
          if (self.val() == '' || self.val() == currentValue) {
            alert(Drupal.t('Bạn phải thay đổi nội dung'));
            e.preventDefault();
          }
        });
      });
    }
  })
})(jQuery);
