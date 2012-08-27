(function($) {
	Drupal.behaviors.emotions = {
		attach : function() {
			$('.emotions-wrapper').hide();
			$('.emotions').once('emotions').click(function(e) {
				var em = "<img src='"+$(this).attr('src')+"' />";
				var textarea = $(this).parent().parent().parent().parent().parent().children().find('.jwysiwyg');
				Drupal.wysiwyg.instances[textarea.attr('id')].insert(em);
				//textarea.wysiwyg('insertHtml', em);
			});
			var sfEls = $('.iconhover > div');// document.getElementById("iconhover").getElementsByTagName("div");
			for (var i=0; i<sfEls.length; i++) {
				sfEls[i].onmouseover=function() {
					this.className+=" hover";
				}
				sfEls[i].onmouseout=function() {
					this.className=this.className.replace(new RegExp(" hover\\b"), "");
				}
			}
		}
	};
})(jQuery);