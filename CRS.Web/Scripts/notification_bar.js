(function($){
  Drupal.behaviors.notification_bar = {
    attach: function () {
      $('#notification-bar-start').click(function (e) {
	        var self = $(this);
	        if($('.notification-bar').is(":visible")){
	        	$('.notification-bar').hide();
	        }else{
	        	$('.notification-bar').show();
		        $.getJSON(self.attr('href'), function (data) {
		        	if(data == 'ok'){
		        		$('#notification-bar-start span').html('0');
		        		$('#notification-bar-start span').removeClass('notification-bar-number');
		        	}
		        });	
	        }
	        e.preventDefault();
      });
      $(document).click(function(event) {
	    if (!$(event.target).hasClass('notification-bar') &&
	    		!$(event.target).hasClass('notification-bar-link') &&
	    		!$(event.target).hasClass('notification-bar-number')) {
	         $(".notification-bar").hide();
	    }
	});
    }
  };
})(jQuery);