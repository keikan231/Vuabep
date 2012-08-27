(function($){
  Drupal.behaviors.userlike = {
    attach: function () {
      $('.user-like-action').click(function (e) {
        var self = $(this);
        $.getJSON(self.attr('href'), function (data) {
          self.html(data.message);
          $(data.selector).html(data.newcount);
          $(data.selector2).html(data.num);
          $(data.selector3).removeClass(data.oldclass).addClass(data.newclass);
        });
        e.preventDefault();
      });
    }
  };
})(jQuery);