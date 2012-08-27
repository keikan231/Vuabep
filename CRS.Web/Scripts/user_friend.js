(function($) {
  Drupal.behaviors.user_friend = {
    attach : function() { 
        $('.add_friend_ajax').once('ajax_add_friend').click(function (e) {
          var self = $(this);
          $.getJSON(self.attr('href'),function(data){
        	  self.parent().html(data);
          });
          e.preventDefault();
        });
        $('.action-friend').click(function (e) {
          var self = $(this);
          $.getJSON(self.attr('href'),function(data){
            if(data.href != undefined) {
              self.attr('href', data.href).html(data.html);
            } else if (data.remove) {
              self.parent().remove();
            }
          });
          e.preventDefault();
        });
        $('.remove-friend').click(function (e) {
          var anwser = confirm(Drupal.t('Bạn chắc chắn muốn xóa?'));
          if (anwser) {
            var self = $(this);
            window.location = self.attr('href');
          }
          e.preventDefault();
        });
  }
  }
})(jQuery)