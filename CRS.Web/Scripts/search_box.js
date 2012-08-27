(function ($) {
  Drupal.behaviors.search_box = {
    attach: function () {
    	var search_form_val = $('.search-box-form').val(); 
    	$('.search-box-form').focusin(function(){
    		$(this).val('');
    	});
    	$('.search-box-form').focusout(function(){
    		if($(this).val() == ''){
    			$(this).val(search_form_val);
    		}    		
    	});
    }
  }
})(jQuery)  