(function ($) {
  Drupal.behaviors.behaviors = {
    attach: function (context) {
      if ($.isFunction($.fn.easySlider)) {
        $("#slideshow", context).easySlider({numeric: true, auto: true, continuous: true, pause: parseInt(Drupal.settings.slideshow.timeout)});
        $('.autoslide', context).each(function (index, item) {
          var speed = $(this).attr('role');
          $(this).easySlider({auto: true, continuous: true, pause: parseInt(speed), controlsShow: false, speed: 0});
        });
      }
      $('.advanced-link', context).click(function() {
        $('#advanced-search').slideToggle();
      });
      uniform();
      // Check all
      $('.jCheckHandler:checkbox', context).jCheckHandler({'all': '.jCheckHandlerAll', 'none': '.jCheckHandlerNone'}).bind('change', function (e) {
        checkprint();
      });
      $('#print-handbook .xoa', context).click(function (e) {
        if($('.jCheckHandler:checkbox:checked', context).length == 0) {
          alert('Xin hãy chọn ít nhất một recipe');
        } else {
          $('#print-handbook', context).submit();
        }
        e.preventDefault();
      });
//      $('.field-add-more-submit', context).unbind('keypress keyup').once('nguyenlieu-remove').bind('keydown', function (e) {
//        $(this).trigger('mousedown');
//        e.preventDefault();
//      });
      $('.nguyenlieu-remove', context).once('nguyenlieu-remove').bind('click', function (e) {
    	  $(this).parent().children('input[type="text"]').each(function () {
    		  $(this).val('');
    	  });
    	  e.preventDefault();
//    	  $('#field-nguyen-lieu-add-more-wrapper .field-add-more-submit', context).trigger('mousedown');
      });
      $('.field-add-more-submit', context).bind('mousedown', function () {
        $(this).parents('div.form-item').find('.wysiwyg').each(function (index, item) {
          var id = $(item).attr('id');
          var params = Drupal.settings.wysiwyg.triggers[id];
          for (var format in params) {
            params[format].format = format;
            params[format].trigger = this.id;
            params[format].field = params.field;
          }
          var format = 'format' + this.value;
          Drupal.wysiwygDetach(context, params[format]);
        });
      });
      $('.jwysiwyg-remove', context).once('jwysiwyg-remove').bind('click keypress', function (e) {
        if ((e.keyCode == undefined) || (e.keyCode != undefined && e.keyCode == 0)) {
          //
          $(this).parent().children().find('.wysiwyg').each(function (index, item) {
            var id = $(item).attr('id');
            var params = Drupal.settings.wysiwyg.triggers[id];
            for (var format in params) {
              params[format].format = format;
              params[format].trigger = this.id;
              params[format].field = params.field;
            }
            var format = 'format' + this.value;
            Drupal.wysiwygDetach(context, params[format]);
          });
          var $textarea = $(this).parent().children().find('.jwysiwyg');
          $textarea.empty().val('').html('');
//          $(this).parent().hide();
//          $textarea.hide();
          e.preventDefault();
        }
      });
      // In
      $('.form_auto_expand_input', context).once('update').change(function (e) {
        var val = $(this).val();
        var id = $(this).attr('name') + '_input';
        if (val == 'other') {
          // Hiện ra textbox
          $('<input type="text" class="form-text" />').attr({
            'name': id,
            'id': id
          }).val($(this).attr('default_value')).insertBefore($(this).parent().parent().children('.description'));
        } else {
          $('#' + id).remove();
        }
      });
      $('.form_auto_expand_input', context).trigger('change');
      $('.print-all', context).once('print', function () {
        var $element = $(this);
        $element.data('href', $element.attr('href'));
        // Kiểm tra sự kiện click
        $element.click(function (e) {
          $element.unbind('click.fb');
          if($element.attr('href') == $element.data('href')) {
            alert('Xin hãy chọn ít nhất một recipe');
            e.preventDefault();
          } else {
            $element.fancybox({
              width: '75%',
              height: '75%',
              autoScale: false,
              overlayShow: true,
              hideOnOverlayClick: false,
              hideOnContentClick: false,
              onComplete: printrecipe,
              title: 'In',
              titleShow: true,
              type: 'iframe'
            }).trigger('click.fb');
            e.preventDefault();
          }
        })
      });
      checkprint();
      $('.print-recipe', context).fancybox({
        width: '75%',
        height: '75%',
        autoScale: false,
        overlayShow: true,
        hideOnOverlayClick: false,
        hideOnContentClick: false,
        onComplete: printrecipe,
        title: 'In',
        titleShow: true,
        type: 'iframe'
      });
      function printrecipe() {
        var framename = $('#fancybox-frame').attr('name');
        $('#fancybox-title-main').once('click').click(function () {
          window.frames[framename].focus();
          window.frames[framename].print();
        });
      }
      $('.imagefancy', context).fancybox();
      var addOK = '';
      $('.big-add-handbook', context).once('add-handbook', function () {
        var $element = $(this);
        addOK = '';
        $element.fancybox({
          overlayShow: true,
          hideOnOverlayClick: false,
          hideOnContentClick: false,
          onComplete: resize,
          onClosed: function () {
            if (addOK != '') {
              $element.remove();
            }
          }
        });
      });
      $('.add-handbook', context).once('add-handbook', function () {
        var $element = $(this);
        addOK = '';
        $element.fancybox({
          overlayShow: true,
          hideOnOverlayClick: false,
          hideOnContentClick: false,
          onComplete: resize,
          onClosed: function () {
            if (addOK != '') {
              $element.replaceWith(addOK);
            }
          }
        });
      });
      $('#handbook_category_select', context).once('change').change(function (e) {
        window.location = $(this).val();
      });
      
      // add handbook
      $('#handbook-add-form input[type="text"]', context).live('keypress', function (e) {
        // Nếu là nhấn enter
        if(e.keyCode == 13) {
          $('#add-hanbook-form .add-cat', context).trigger('click');
          e.preventDefault();
        }
      });
      $('#add-hanbook-form .add-cat', context).live('click', function (e) {
        // Lay URL
        var title = $(this).parent().find('input.title').val();
        var url = Drupal.settings.basePath + 'handbook/cat/' + title;
        $.ajax({
          url: url,
          dataType: 'json',
          beforeSend: function () {
            $.fancybox.showActivity();
          },
          success: function (data) {
            $.fancybox.hideActivity();
            if (data.cid != undefined) {
              var $li = $('<li />').html('<input type="checkbox" id="hbcid_' + data.cid + '" name="cid" value="' + data.cid + '" /><label for="hbcid_' + data.cid + '">' + data.name + '</label>');
              $li.insertBefore($('#add-hanbook-form li.form'));
              resize();
            }
          }
        });
        e.preventDefault();
      });
      // Lưu handbook
      $('#add-hanbook-form .submit', context).live('click', function (e) {
        // lấy danh sách catid
        var catId = Array();
        $('#add-hanbook-form input:checked', context).each(function (index, item) {
          catId.push($(item).val());
        });
        var url = Drupal.settings.basePath + 'handbook/save/' + $('#add_hb_nid').val();
        $.ajax({
          url: url,
          type: 'POST',
          data: {'cid[]': catId},
          beforeSend: function () {
            $.fancybox.showActivity();
          },
          success: function (data) {
            addOK = data;
            $.fancybox.close();
          }
        });
        e.preventDefault();
      });
//      var msie6 = $.browser == 'msie' && $.browser.version < 7;
//		if (!msie6) {
//			var top = $('.floating_share').offset().top	- parseFloat($('.floating_share').css('margin-top').replace(/auto/, 0));
//			$(window).scroll(function (event) {
//			var y = $(this).scrollTop();
//				if (y >= top) {
//					$('.floating_share').addClass('fixed');
//				}
//				else {
//					$('.floating_share').removeClass('fixed');
//				}
//			});
//		}
//		if (screen.width <= 1250) {
//		 $(".floating_share").addClass("disnone");
//		}
    }
  }
  
  function checkprint() {
    var checked = $('.jCheckHandler:checkbox:checked');
    var select = Array();
    $.each(checked, function (index, item) {
      select.push($(item).val());
    });
    $('.print-all').attr('href', $('.print-all').data('href') + select.join('+')).unbind('click.fb');
  }
  
  function resize() {
    uniform();
    if ($.isFunction($.fancybox)) {
      $.fancybox.resize();
    }
    $('.hint').once('hint').hint();
  }
  
  function uniform() {
    if ($.isFunction($.fn.uniform)) {
      if (typeof Drupal.settings.nouniform != 'boolean') {
        $("select, input:checkbox, input:radio, input:file").once('uniform').uniform();
      }
    }
  }
})(jQuery);
;
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
})(jQuery);;
(function ($) {
function checkBoxHandler(checkbox, all, none) {
  this.checkbox = checkbox;
  this.all = all;
  this.none = none;
}

checkBoxHandler.prototype.process = function () {
  var checkbox = this.checkbox;
  // bind click
  $(this.none).bind('click', function (e) {
    checkbox.attr('checked', false).trigger('change');
    $.uniform.update();
    e.preventDefault();
  });
  $(this.all).bind('click', function (e) {
    checkbox.attr('checked', true).trigger('change');
    $.uniform.update();
    e.preventDefault();
  });
}

$.fn.jCheckHandler = function (options) {
  var check = new checkBoxHandler(this, options.all, options.none);
  check.process();
  return this;
}
})(jQuery);
;
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
;
(function ($) {
  Drupal.behaviors.node_extra = {
    attach: function (context) {
//      var ajaxCache = [], contentType = '';
//      $('#mini-search-content-type').find('input[type=radio]').once('contenttype').bind('click', function () {
//    	  var searchType = $(this).val();
//    	  alert(searchType);	
//    	  if (searchType != 'all') {
//    		  alert(searchType);	
//    		  if (ajaxCache[searchType] == undefined) {
//            $.getJSON(Drupal.settings.basePath + 'behaviors/search/' + searchType, function(data) {
//              ajaxCache[searchType] = data;
//              buildData(ajaxCache[searchType], searchType);
//            });
//	      } else if(searchType == 'user'){
//	    	  alert(searchType);	
//	    	  $('#mini-search-list-category').data('current', 'user');
//	    	  $('#mini-search-list-category').empty();
//	      }else {
//	    	  alert(searchType);	
//	    	  buildData(ajaxCache[searchType], searchType);
//	      }
//          
//        } else{
//        	$('#mini-search-list-category').data('current', null);
//          $('#mini-search-list-category').empty();
//        }
//        function buildData(data, dataType) {
//          $('#mini-search-list-category').data('current', searchType);
//          var $select1 = $('<select id="term-parent-select" />');
//          $.each(data.parent, function (index, item) {
//            $select1.append('<option value="' + index + '">' + item + '</option>');
//          });
//          $('#mini-search-list-category').empty().append($select1).append('<select id="mini-search-term-select" />');
//          $select1.change(function () {
//            $('#mini-search-term-select').empty();
//            if (data.children[$select1.val()] != undefined) {
//              var $select2 = $('#mini-search-term-select');
//              $.each(data.children[$select1.val()], function (index, item) {
//                $select2.append('<option value="' + index + '">' + item + '</option>');
//              });
//            }
//            jQuery.uniform.update();
//          });
//          $select1.trigger('change', $("select, input:checkbox, input:radio, input:file").once('uniform').uniform());
//        }
//      });
    	$('#mini-search-list-category').prev().hide();
    	$('#mini-search-list-category').hide();
    	
      $('#mini-search-content-type').find('input[type=radio]:checked').trigger('click');
      var $form = $('#mini-search');
     
      $form.submit(function (e) {
    	  var action = $(this).attr('action')+'/';
        if ($('#mini-search-field').val().length < 2 || $('#mini-search-field').val() == 'Tìm kiếm trong hàng ngàn công thức nấu ăn ') {
          alert(Drupal.t('Phải điền ít nhất 2 kí tự'));
          $('#mini-search-field').focus();
          return false;
        } else {
          if (typeof $('#mini-search-match-type').val() != 'undefined' && $('#mini-search-match-type').val() != '') {
            if ($('#mini-search-match-type').val() == 'regular') {
              action += $('#mini-search-field').val();
            } else {
              action += '"' + $('#mini-search-field').val() + '"';
            }
          }

          if ($('#mini-search-content-type').find(':checked').val() != undefined && $('#mini-search-content-type').find(':checked').val() != 'all') {
            action += ' type:' + $('#mini-search-content-type').find(':checked').val();
          }
          
          if ($('#mini-search-content-type').find(':checked').val() == 'user') {
        	  var action = Drupal.settings.basePath + 'search/user/'+$('#mini-search-field').val();
          }
          if ($('#advanced-search').is(':visible')) {
            if (typeof $('#mini-search-term-select').val() != 'undefined' && $('#mini-search-term-select').val() != '') {
              action += ' term:' + $('#mini-search-term-select').val();
            }
            if (typeof $('#minisearch-username').val() != 'undefined' && $('#minisearch-username').val() != '') {
              action += ' name:' + $('#minisearch-username').val();
            }
            if (typeof $('#mini-search-orderby').val() != 'undefined' && $('#mini-search-orderby').val() == 'created') {
              action += '?order=created';
            }
          }
          $form.attr('action', action);
          window.location = action;
          action = Drupal.settings.basePath + 'search/node/';
          return false;
        }
      });
      
      var search_form_val = $('.search-box-form').val(); 
  	  $('.search-box-form').focusin(function(){
  		$(this).val('');
  	  });
  	  $('.search-box-form').focusout(function(){
  		if($(this).val() == ''){
  			$(this).val(search_form_val);
  		}    		
  	  });
  	  var advance_search = $('#mini-search-field').val();
  	  $('#mini-search-field').focusin(function(){
  		$(this).val('');
  	  });
  	  $('#mini-search-field').focusout(function(){
  		if($(this).val() == ''){
  			$(this).val(advance_search);
  		}    		
  	  });
    }
  }
})(jQuery);
;
(function ($) {
  Drupal.behaviors.fancyupload = {
    attach: function (context) {
      $('.fancybox_upload').once('fancybox_upload', function () {
        var name = $(this).attr('name');
        var url = Drupal.settings.fancyboxUpload + name;
        if (Drupal.settings.fancyImage && Drupal.settings.fancyImage[name] && Drupal.settings.fancyImage[name].directory != '') {
          url += '/' + Drupal.settings.fancyImage[name].directory;
        }
        $(this).fancybox({
          overlayShow: true,
          hideOnOverlayClick: false,
          hideOnContentClick: false,
          type: 'iframe',
          href: url,
          onClosed: fancyboxClose
        })
      });
    }
  }
  
  function fancyboxClose() {
    //for (var i in Drupal.settings.returnObjects) {
      var object = Drupal.settings.returnObjects;
      // Kiểm tra có phải hình chính hay không
      if (typeof Drupal.settings.fancyImage == 'object' && typeof Drupal.settings.fancyImage[object.field] != 'undefined') {
        var fancyField = Drupal.settings.fancyImage[object.field];
        // Thêm hình
        $(fancyField.selector).empty().append($('<img />').attr({'src': object.filesrc, 'title': object.title, 'alt': object.alt}));
        // Xóa nút
        //$(fancyField.selector).next().remove();
        // Cập nhật giá trị
        $(fancyField.selector).next().next().val(object.fid + '|' + object.title + '|' + object.alt);
      }
    //}
  }
  
})(jQuery);
;
/**
 * jQuery.ScrollTo
 * Copyright (c) 2007-2009 Ariel Flesler - aflesler(at)gmail(dot)com | http://flesler.blogspot.com
 * Dual licensed under MIT and GPL.
 * Date: 5/25/2009
 *
 * @projectDescription Easy element scrolling using jQuery.
 * http://flesler.blogspot.com/2007/10/jqueryscrollto.html
 * Works with jQuery +1.2.6. Tested on FF 2/3, IE 6/7/8, Opera 9.5/6, Safari 3, Chrome 1 on WinXP.
 *
 * @author Ariel Flesler
 * @version 1.4.2
 *
 * @id jQuery.scrollTo
 * @id jQuery.fn.scrollTo
 * @param {String, Number, DOMElement, jQuery, Object} target Where to scroll the matched elements.
 *	  The different options for target are:
 *		- A number position (will be applied to all axes).
 *		- A string position ('44', '100px', '+=90', etc ) will be applied to all axes
 *		- A jQuery/DOM element ( logically, child of the element to scroll )
 *		- A string selector, that will be relative to the element to scroll ( 'li:eq(2)', etc )
 *		- A hash { top:x, left:y }, x and y can be any kind of number/string like above.
*		- A percentage of the container's dimension/s, for example: 50% to go to the middle.
 *		- The string 'max' for go-to-end. 
 * @param {Number} duration The OVERALL length of the animation, this argument can be the settings object instead.
 * @param {Object,Function} settings Optional set of settings or the onAfter callback.
 *	 @option {String} axis Which axis must be scrolled, use 'x', 'y', 'xy' or 'yx'.
 *	 @option {Number} duration The OVERALL length of the animation.
 *	 @option {String} easing The easing method for the animation.
 *	 @option {Boolean} margin If true, the margin of the target element will be deducted from the final position.
 *	 @option {Object, Number} offset Add/deduct from the end position. One number for both axes or { top:x, left:y }.
 *	 @option {Object, Number} over Add/deduct the height/width multiplied by 'over', can be { top:x, left:y } when using both axes.
 *	 @option {Boolean} queue If true, and both axis are given, the 2nd axis will only be animated after the first one ends.
 *	 @option {Function} onAfter Function to be called after the scrolling ends. 
 *	 @option {Function} onAfterFirst If queuing is activated, this function will be called after the first scrolling ends.
 * @return {jQuery} Returns the same jQuery object, for chaining.
 *
 * @desc Scroll to a fixed position
 * @example $('div').scrollTo( 340 );
 *
 * @desc Scroll relatively to the actual position
 * @example $('div').scrollTo( '+=340px', { axis:'y' } );
 *
 * @dec Scroll using a selector (relative to the scrolled element)
 * @example $('div').scrollTo( 'p.paragraph:eq(2)', 500, { easing:'swing', queue:true, axis:'xy' } );
 *
 * @ Scroll to a DOM element (same for jQuery object)
 * @example var second_child = document.getElementById('container').firstChild.nextSibling;
 *			$('#container').scrollTo( second_child, { duration:500, axis:'x', onAfter:function(){
 *				alert('scrolled!!');																   
 *			}});
 *
 * @desc Scroll on both axes, to different values
 * @example $('div').scrollTo( { top: 300, left:'+=200' }, { axis:'xy', offset:-20 } );
 */
;(function( $ ){
	
	var $scrollTo = $.scrollTo = function( target, duration, settings ){
		$(window).scrollTo( target, duration, settings );
	};

	$scrollTo.defaults = {
		axis:'xy',
		duration: parseFloat($.fn.jquery) >= 1.3 ? 0 : 1
	};

	// Returns the element that needs to be animated to scroll the window.
	// Kept for backwards compatibility (specially for localScroll & serialScroll)
	$scrollTo.window = function( scope ){
		return $(window)._scrollable();
	};

	// Hack, hack, hack :)
	// Returns the real elements to scroll (supports window/iframes, documents and regular nodes)
	$.fn._scrollable = function(){
		return this.map(function(){
			var elem = this,
				isWin = !elem.nodeName || $.inArray( elem.nodeName.toLowerCase(), ['iframe','#document','html','body'] ) != -1;

				if( !isWin )
					return elem;

			var doc = (elem.contentWindow || elem).document || elem.ownerDocument || elem;
			
			return $.browser.safari || doc.compatMode == 'BackCompat' ?
				doc.body : 
				doc.documentElement;
		});
	};

	$.fn.scrollTo = function( target, duration, settings ){
		if( typeof duration == 'object' ){
			settings = duration;
			duration = 0;
		}
		if( typeof settings == 'function' )
			settings = { onAfter:settings };
			
		if( target == 'max' )
			target = 9e9;
			
		settings = $.extend( {}, $scrollTo.defaults, settings );
		// Speed is still recognized for backwards compatibility
		duration = duration || settings.speed || settings.duration;
		// Make sure the settings are given right
		settings.queue = settings.queue && settings.axis.length > 1;
		
		if( settings.queue )
			// Let's keep the overall duration
			duration /= 2;
		settings.offset = both( settings.offset );
		settings.over = both( settings.over );

		return this._scrollable().each(function(){
			var elem = this,
				$elem = $(elem),
				targ = target, toff, attr = {},
				win = $elem.is('html,body');

			switch( typeof targ ){
				// A number will pass the regex
				case 'number':
				case 'string':
					if( /^([+-]=)?\d+(\.\d+)?(px|%)?$/.test(targ) ){
						targ = both( targ );
						// We are done
						break;
					}
					// Relative selector, no break!
					targ = $(targ,this);
				case 'object':
					// DOMElement / jQuery
					if( targ.is || targ.style )
						// Get the real position of the target 
						toff = (targ = $(targ)).offset();
			}
			$.each( settings.axis.split(''), function( i, axis ){
				var Pos	= axis == 'x' ? 'Left' : 'Top',
					pos = Pos.toLowerCase(),
					key = 'scroll' + Pos,
					old = elem[key],
					max = $scrollTo.max(elem, axis);

				if( toff ){// jQuery / DOMElement
					attr[key] = toff[pos] + ( win ? 0 : old - $elem.offset()[pos] );

					// If it's a dom element, reduce the margin
					if( settings.margin ){
						attr[key] -= parseInt(targ.css('margin'+Pos)) || 0;
						attr[key] -= parseInt(targ.css('border'+Pos+'Width')) || 0;
					}
					
					attr[key] += settings.offset[pos] || 0;
					
					if( settings.over[pos] )
						// Scroll to a fraction of its width/height
						attr[key] += targ[axis=='x'?'width':'height']() * settings.over[pos];
				}else{ 
					var val = targ[pos];
					// Handle percentage values
					attr[key] = val.slice && val.slice(-1) == '%' ? 
						parseFloat(val) / 100 * max
						: val;
				}

				// Number or 'number'
				if( /^\d+$/.test(attr[key]) )
					// Check the limits
					attr[key] = attr[key] <= 0 ? 0 : Math.min( attr[key], max );

				// Queueing axes
				if( !i && settings.queue ){
					// Don't waste time animating, if there's no need.
					if( old != attr[key] )
						// Intermediate animation
						animate( settings.onAfterFirst );
					// Don't animate this axis again in the next iteration.
					delete attr[key];
				}
			});

			animate( settings.onAfter );			

			function animate( callback ){
				$elem.animate( attr, duration, settings.easing, callback && function(){
					callback.call(this, target, settings);
				});
			};

		}).end();
	};
	
	// Max scrolling position, works on quirks mode
	// It only fails (not too badly) on IE, quirks mode.
	$scrollTo.max = function( elem, axis ){
		var Dim = axis == 'x' ? 'Width' : 'Height',
			scroll = 'scroll'+Dim;
		
		if( !$(elem).is('html,body') )
			return elem[scroll] - $(elem)[Dim.toLowerCase()]();
		
		var size = 'client' + Dim,
			html = elem.ownerDocument.documentElement,
			body = elem.ownerDocument.body;

		return Math.max( html[scroll], body[scroll] ) 
			 - Math.min( html[size]  , body[size]   );
			
	};

	function both( val ){
		return typeof val == 'object' ? val : { top:val, left:val };
	};

})( jQuery );;
//-------------------------------------------------
//		Quick Pager jquery plugin
//		Created by dan and emanuel @geckonm.com
//		www.geckonewmedia.com
// 
//		v1.1
//		18/09/09 * bug fix by John V - http://blog.geekyjohn.com/
//-------------------------------------------------

(function($) {
	    
	$.fn.quickPager = function(options) {
	
		var defaults = {
			pageSize: 16,
			curpage: 1,
			holder: null,
			pagerLocation: "after"
		};
		
		var options = $.extend(defaults, options);
		
		
		return this.each(function() {
	
						
			var selector = $(this);	
			var pageCounter = 1;
			
			selector.wrap("<div class='item-list-thumb'></div>");
			
			selector.children().each(function(i){ 
					
				if(i < pageCounter*options.pageSize && i >= (pageCounter-1)*options.pageSize) {
				$(this).addClass("simplePagerPage"+pageCounter);
				}
				else {
					$(this).addClass("simplePagerPage"+(pageCounter+1));
					pageCounter ++;
				}	
				
			});
			
			// show/hide the appropriate regions 
			selector.children().hide();
			selector.children(".simplePagerPage"+options.curpage).show();
			
			if(pageCounter <= 1) {
				return;
			}
			
			//Build pager navigation
			var pageNav = "<ul class='pager'>";	
			for (i=1;i<=pageCounter;i++){
				if (i==options.curpage) {
					pageNav += "<li class='curpage page"+i+"'><a rel='"+i+"' href='#'>"+i+"</a></li>";	
				}
				else {
					pageNav += "<li class='page"+i+"'><a rel='"+i+"' href='#'>"+i+"</a></li>";
				}
			}
			pageNav += "</ul>";
			
			if(!options.holder) {
				switch(options.pagerLocation)
				{
				case "before":
					selector.before(pageNav);
				break;
				case "both":
					selector.before(pageNav);
					selector.after(pageNav);
				break;
				default:
					selector.after(pageNav);
				}
			}
			else {
				$(options.holder).append(pageNav);
			}
			
			//pager navigation behaviour
			selector.parent().find(".pager a").click(function() {
					
				//grab the REL attribute 
				var clickedLink = $(this).attr("rel");
				options.curpage = clickedLink;
				
				if(options.holder) {
					$(this).parent("li").parent("ul").parent(options.holder).find("li.curpage").removeClass("curpage");
					$(this).parent("li").parent("ul").parent(options.holder).find("a[rel='"+clickedLink+"']").parent("li").addClass("curpage");
				}
				else {
					//remove current current (!) page
					$(this).parent("li").parent("ul").parent(".item-list").find("li.curpage").removeClass("curpage");
					//Add current page highlighting
					$(this).parent("li").parent("ul").parent(".item-list").find("a[rel='"+clickedLink+"']").parent("li").addClass("curpage");
				}
				
				//hide and show relevant links
				selector.children().hide();			
				selector.find(".simplePagerPage"+clickedLink).show();
				$.scrollTo($('#paging'), 800);
				return false;
			});
		});
	}
	

})(jQuery);

;
(function($) {
  $(function() {
    $("ul#paging").quickPager();
    $("a.fancybox-link, a.record-photo-link").once("fancybox",
      function() {
        $(this).fancybox({
          overlayShow : true,
          hideOnOverlayClick : false,
          hideOnContentClick : false,
          titleShow : true,
          href : $(this).attr("href"),
          autoDimensions: true,
          onStart: function() {
            $('embed, object, select').css('visibility', 'hidden');
          },
          onClosed: function() {
            $('embed, object, select').css('visibility', 'visible');
          }
        });
      });
    });
  }
)(jQuery);;
/**
 * Anti-Spambot jQuery plugin
 * @author Ji Shan <jishanvn@gmail.com>
 * @version 1.0
 */
(function($){
  $.fn.antiSpamBot = function(regexAt, regexDot){
    if (typeof regexAt == 'undefined') regexAt = /\s\[-at-\]\s/ig;
    if (typeof regexDot == 'undefined') regexDot = /\s\[-dot-\]\s/ig;
      
    $(this).each(function() {             
      var _email = $(this).text();
      if (typeof _email=='undefined' || _email=='') return;
      _email = _email.replace(regexAt, '@').replace(regexDot, '.');
      $(this).html('<a href="mailto:'+_email+'">'+_email+'</a>');
    });           
  }
})(jQuery);
;
/*
 * FancyBox - jQuery Plugin
 * Simple and fancy lightbox alternative
 *
 * Examples and documentation at: http://fancybox.net
 * 
 * Copyright (c) 2008 - 2010 Janis Skarnelis
 *
 * Version: 1.3.1 (05/03/2010)
 * Requires: jQuery v1.3+
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */

(function($) {

	var tmp, loading, overlay, wrap, outer, inner, close, nav_left, nav_right,

		selectedIndex = 0, selectedOpts = {}, selectedArray = [], currentIndex = 0, currentOpts = {}, currentArray = [],

		ajaxLoader = null, imgPreloader = new Image(), imgRegExp = /\.(jpg|gif|png|bmp|jpeg)(.*)?$/i, swfRegExp = /[^\.]\.(swf)\s*$/i,

		loadingTimer, loadingFrame = 1,

		start_pos, final_pos, busy = false, shadow = 20, fx = $.extend($('<div/>')[0], { prop: 0 }), titleh = 0, 

		isIE6 = !$.support.opacity && !window.XMLHttpRequest,

		/*
		 * Private methods 
		 */

		fancybox_abort = function() {
			loading.hide();

			imgPreloader.onerror = imgPreloader.onload = null;

			if (ajaxLoader) {
				ajaxLoader.abort();
			}

			tmp.empty();
		},

		fancybox_error = function() {
			$.fancybox('<p id="fancybox_error">The requested content cannot be loaded.<br />Please try again later.</p>', {
				'scrolling'		: 'no',
				'padding'		: 20,
				'transitionIn'	: 'none',
				'transitionOut'	: 'none'
			});
		},

		fancybox_get_viewport = function() {
			return [ $(window).width(), $(window).height(), $(document).scrollLeft(), $(document).scrollTop() ];
		},

		fancybox_get_zoom_to = function () {
			var view	= fancybox_get_viewport(),
				to		= {},

				margin = currentOpts.margin,
				resize = currentOpts.autoScale,

				horizontal_space	= (shadow + margin) * 2,
				vertical_space		= (shadow + margin) * 2,
				double_padding		= (currentOpts.padding * 2),
				
				ratio;

			if (currentOpts.width.toString().indexOf('%') > -1) {
				to.width = ((view[0] * parseFloat(currentOpts.width)) / 100) - (shadow * 2) ;
				resize = false;

			} else {
				to.width = currentOpts.width + double_padding;
			}

			if (currentOpts.height.toString().indexOf('%') > -1) {
				to.height = ((view[1] * parseFloat(currentOpts.height)) / 100) - (shadow * 2);
				resize = false;

			} else {
				to.height = currentOpts.height + double_padding;
			}

			if (resize && (to.width > (view[0] - horizontal_space) || to.height > (view[1] - vertical_space))) {
				if (selectedOpts.type == 'image' || selectedOpts.type == 'swf') {
					horizontal_space	+= double_padding;
					vertical_space		+= double_padding;

					ratio = Math.min(Math.min( view[0] - horizontal_space, currentOpts.width) / currentOpts.width, Math.min( view[1] - vertical_space, currentOpts.height) / currentOpts.height);

					to.width	= Math.round(ratio * (to.width	- double_padding)) + double_padding;
					to.height	= Math.round(ratio * (to.height	- double_padding)) + double_padding;

				} else {
					to.width	= Math.min(to.width,	(view[0] - horizontal_space));
					to.height	= Math.min(to.height,	(view[1] - vertical_space));
				}
			}

			to.top	= view[3] + ((view[1] - (to.height	+ (shadow * 2 ))) * 0.5);
			to.left	= view[2] + ((view[0] - (to.width	+ (shadow * 2 ))) * 0.5);

			if (currentOpts.autoScale === false) {
				to.top	= Math.max(view[3] + margin, to.top);
				to.left	= Math.max(view[2] + margin, to.left);
			}

			return to;
		},

		fancybox_format_title = function(title) {
			if (title && title.length) {
				switch (currentOpts.titlePosition) {
					case 'inside':
						return title;
					case 'over':
						return '<span id="fancybox-title-over">' + title + '</span>';
					default:
						return '<span id="fancybox-title-wrap"><span id="fancybox-title-left"></span><span id="fancybox-title-main">' + title + '</span><span id="fancybox-title-right"></span></span>';
				}
			}

			return false;
		},

		fancybox_process_title = function() {
			var title	= currentOpts.title,
				width	= final_pos.width - (currentOpts.padding * 2),
				titlec	= 'fancybox-title-' + currentOpts.titlePosition;
				
			$('#fancybox-title').remove();

			titleh = 0;

			if (currentOpts.titleShow === false) {
				return;
			}

			title = $.isFunction(currentOpts.titleFormat) ? currentOpts.titleFormat(title, currentArray, currentIndex, currentOpts) : fancybox_format_title(title);

			if (!title || title === '') {
				return;
			}

			$('<div id="fancybox-title" class="' + titlec + '" />').css({
				'width'			: width,
				'paddingLeft'	: currentOpts.padding,
				'paddingRight'	: currentOpts.padding
			}).html(title).appendTo('body');

			switch (currentOpts.titlePosition) {
				case 'inside':
					titleh = $("#fancybox-title").outerHeight(true) - currentOpts.padding;
					final_pos.height += titleh;
				break;

				case 'over':
					$('#fancybox-title').css('bottom', currentOpts.padding);
				break;

				default:
					$('#fancybox-title').css('bottom', $("#fancybox-title").outerHeight(true) * -1);
				break;
			}

			$('#fancybox-title').appendTo( outer ).hide();
		},

		fancybox_set_navigation = function() {
			$(document).unbind('keydown.fb').bind('keydown.fb', function(e) {
				if (e.keyCode == 27 && currentOpts.enableEscapeButton) {
					e.preventDefault();
					$.fancybox.close();

				} else if (e.keyCode == 37) {
					e.preventDefault();
					$.fancybox.prev();

				} else if (e.keyCode == 39) {
					e.preventDefault();
					$.fancybox.next();
				}
			});

			if ($.fn.mousewheel) {
				wrap.unbind('mousewheel.fb');

				if (currentArray.length > 1) {
					wrap.bind('mousewheel.fb', function(e, delta) {
						e.preventDefault();

						if (busy || delta === 0) {
							return;
						}

						if (delta > 0) {
							$.fancybox.prev();
						} else {
							$.fancybox.next();
						}
					});
				}
			}

			if (!currentOpts.showNavArrows) { return; }

			if ((currentOpts.cyclic && currentArray.length > 1) || currentIndex !== 0) {
				nav_left.show();
			}

			if ((currentOpts.cyclic && currentArray.length > 1) || currentIndex != (currentArray.length -1)) {
				nav_right.show();
			}
		},

		fancybox_preload_images = function() {
			var href, 
				objNext;
				
			if ((currentArray.length -1) > currentIndex) {
				href = currentArray[ currentIndex + 1 ].href;

				if (typeof href !== 'undefined' && href.match(imgRegExp)) {
					objNext = new Image();
					objNext.src = href;
				}
			}

			if (currentIndex > 0) {
				href = currentArray[ currentIndex - 1 ].href;

				if (typeof href !== 'undefined' && href.match(imgRegExp)) {
					objNext = new Image();
					objNext.src = href;
				}
			}
		},

		_finish = function () {
			inner.css('overflow', (currentOpts.scrolling == 'auto' ? (currentOpts.type == 'image' || currentOpts.type == 'iframe' || currentOpts.type == 'swf' ? 'hidden' : 'auto') : (currentOpts.scrolling == 'yes' ? 'auto' : 'visible')));

			if (!$.support.opacity) {
				inner.get(0).style.removeAttribute('filter');
				wrap.get(0).style.removeAttribute('filter');
			}

			$('#fancybox-title').show();

			if (currentOpts.hideOnContentClick)	{
				inner.one('click', $.fancybox.close);
			}
			if (currentOpts.hideOnOverlayClick)	{
				overlay.one('click', $.fancybox.close);
			}

			if (currentOpts.showCloseButton) {
				close.show();
			}

			fancybox_set_navigation();

			$(window).bind("resize.fb", $.fancybox.center);

			if (currentOpts.centerOnScroll) {
				$(window).bind("scroll.fb", $.fancybox.center);
			} else {
				$(window).unbind("scroll.fb");
			}

			if ($.isFunction(currentOpts.onComplete)) {
				currentOpts.onComplete(currentArray, currentIndex, currentOpts);
			}

			busy = false;

			fancybox_preload_images();
		},

		fancybox_draw = function(pos) {
			var width	= Math.round(start_pos.width	+ (final_pos.width	- start_pos.width)	* pos),
				height	= Math.round(start_pos.height	+ (final_pos.height	- start_pos.height)	* pos),

				top		= Math.round(start_pos.top	+ (final_pos.top	- start_pos.top)	* pos),
				left	= Math.round(start_pos.left	+ (final_pos.left	- start_pos.left)	* pos);

			wrap.css({
				'width'		: width		+ 'px',
				'height'	: height	+ 'px',
				'top'		: top		+ 'px',
				'left'		: left		+ 'px'
			});

			width	= Math.max(width - currentOpts.padding * 2, 0);
			height	= Math.max(height - (currentOpts.padding * 2 + (titleh * pos)), 0);

			inner.css({
				'width'		: width		+ 'px',
				'height'	: height	+ 'px'
			});

			if (typeof final_pos.opacity !== 'undefined') {
				wrap.css('opacity', (pos < 0.5 ? 0.5 : pos));
			}
		},

		fancybox_get_obj_pos = function(obj) {
			var pos		= obj.offset();

			pos.top		+= parseFloat( obj.css('paddingTop') )	|| 0;
			pos.left	+= parseFloat( obj.css('paddingLeft') )	|| 0;

			pos.top		+= parseFloat( obj.css('border-top-width') )	|| 0;
			pos.left	+= parseFloat( obj.css('border-left-width') )	|| 0;

			pos.width	= obj.width();
			pos.height	= obj.height();

			return pos;
		},

		fancybox_get_zoom_from = function() {
			var orig = selectedOpts.orig ? $(selectedOpts.orig) : false,
				from = {},
				pos,
				view;

			if (orig && orig.length) {
				pos = fancybox_get_obj_pos(orig);

				from = {
					width	: (pos.width	+ (currentOpts.padding * 2)),
					height	: (pos.height	+ (currentOpts.padding * 2)),
					top		: (pos.top		- currentOpts.padding - shadow),
					left	: (pos.left		- currentOpts.padding - shadow)
				};
				
			} else {
				view = fancybox_get_viewport();

				from = {
					width	: 1,
					height	: 1,
					top		: view[3] + view[1] * 0.5,
					left	: view[2] + view[0] * 0.5
				};
			}

			return from;
		},

		fancybox_show = function() {
			loading.hide();

			if (wrap.is(":visible") && $.isFunction(currentOpts.onCleanup)) {
				if (currentOpts.onCleanup(currentArray, currentIndex, currentOpts) === false) {
					$.event.trigger('fancybox-cancel');

					busy = false;
					return;
				}
			}

			currentArray	= selectedArray;
			currentIndex	= selectedIndex;
			currentOpts		= selectedOpts;

			inner.get(0).scrollTop	= 0;
			inner.get(0).scrollLeft	= 0;

			if (currentOpts.overlayShow) {
				if (isIE6) {
					$('select:not(#fancybox-tmp select)').filter(function() {
						return this.style.visibility !== 'hidden';
					}).css({'visibility':'hidden'}).one('fancybox-cleanup', function() {
						this.style.visibility = 'inherit';
					});
				}

				overlay.css({
					'background-color'	: currentOpts.overlayColor,
					'opacity'			: currentOpts.overlayOpacity
				}).unbind().show();
			}

			final_pos = fancybox_get_zoom_to();

			fancybox_process_title();

			if (wrap.is(":visible")) {
				$( close.add( nav_left ).add( nav_right ) ).hide();

				var pos = wrap.position(),
					equal;

				start_pos = {
					top		:	pos.top ,
					left	:	pos.left,
					width	:	wrap.width(),
					height	:	wrap.height()
				};

				equal = (start_pos.width == final_pos.width && start_pos.height == final_pos.height);

				inner.fadeOut(currentOpts.changeFade, function() {
					var finish_resizing = function() {
						inner.html( tmp.contents() ).fadeIn(currentOpts.changeFade, _finish);
					};
					
					$.event.trigger('fancybox-change');

					inner.empty().css('overflow', 'hidden');

					if (equal) {
						inner.css({
							top			: currentOpts.padding,
							left		: currentOpts.padding,
							width		: Math.max(final_pos.width	- (currentOpts.padding * 2), 1),
							height		: Math.max(final_pos.height	- (currentOpts.padding * 2) - titleh, 1)
						});
						
						finish_resizing();

					} else {
						inner.css({
							top			: currentOpts.padding,
							left		: currentOpts.padding,
							width		: Math.max(start_pos.width	- (currentOpts.padding * 2), 1),
							height		: Math.max(start_pos.height	- (currentOpts.padding * 2), 1)
						});
						
						fx.prop = 0;

						$(fx).animate({ prop: 1 }, {
							 duration	: currentOpts.changeSpeed,
							 easing		: currentOpts.easingChange,
							 step		: fancybox_draw,
							 complete	: finish_resizing
						});
					}
				});

				return;
			}

			wrap.css('opacity', 1);

			if (currentOpts.transitionIn == 'elastic') {
				start_pos = fancybox_get_zoom_from();

				inner.css({
						top			: currentOpts.padding,
						left		: currentOpts.padding,
						width		: Math.max(start_pos.width	- (currentOpts.padding * 2), 1),
						height		: Math.max(start_pos.height	- (currentOpts.padding * 2), 1)
					})
					.html( tmp.contents() );

				wrap.css(start_pos).show();

				if (currentOpts.opacity) {
					final_pos.opacity = 0;
				}

				fx.prop = 0;

				$(fx).animate({ prop: 1 }, {
					 duration	: currentOpts.speedIn,
					 easing		: currentOpts.easingIn,
					 step		: fancybox_draw,
					 complete	: _finish
				});

			} else {
				inner.css({
						top			: currentOpts.padding,
						left		: currentOpts.padding,
						width		: Math.max(final_pos.width	- (currentOpts.padding * 2), 1),
						height		: Math.max(final_pos.height	- (currentOpts.padding * 2) - titleh, 1)
					})
					.html( tmp.contents() );

				wrap.css( final_pos ).fadeIn( currentOpts.transitionIn == 'none' ? 0 : currentOpts.speedIn, _finish );
			}
		},

		fancybox_process_inline = function() {
			tmp.width(	selectedOpts.width );
			tmp.height(	selectedOpts.height );

			if (selectedOpts.width	== 'auto') {
				selectedOpts.width = tmp.width();
			}
			if (selectedOpts.height	== 'auto') {
				selectedOpts.height	= tmp.height();
			}

			fancybox_show();
		},
		
		fancybox_process_image = function() {
			busy = true;

			selectedOpts.width	= imgPreloader.width;
			selectedOpts.height	= imgPreloader.height;

			$("<img />").attr({
				'id'	: 'fancybox-img',
				'src'	: imgPreloader.src,
				'alt'	: selectedOpts.title
			}).appendTo( tmp );

			fancybox_show();
		},

		fancybox_start = function() {
			fancybox_abort();

			var obj	= selectedArray[ selectedIndex ],
				href, 
				type, 
				title,
				str,
				emb,
				selector,
				data;

			selectedOpts = $.extend({}, $.fn.fancybox.defaults, (typeof $(obj).data('fancybox') == 'undefined' ? selectedOpts : $(obj).data('fancybox')));
			title = obj.title || $(obj).title || selectedOpts.title || '';
			
			if (obj.nodeName && !selectedOpts.orig) {
				selectedOpts.orig = $(obj).children("img:first").length ? $(obj).children("img:first") : $(obj);
			}

			if (title === '' && selectedOpts.orig) {
				title = selectedOpts.orig.attr('alt');
			}

			if (obj.nodeName && (/^(?:javascript|#)/i).test(obj.href)) {
				href = selectedOpts.href || null;
			} else {
				href = selectedOpts.href || obj.href || null;
			}

			if (selectedOpts.type) {
				type = selectedOpts.type;

				if (!href) {
					href = selectedOpts.content;
				}
				
			} else if (selectedOpts.content) {
				type	= 'html';

			} else if (href) {
				if (href.match(imgRegExp)) {
					type = 'image';

				} else if (href.match(swfRegExp)) {
					type = 'swf';

				} else if ($(obj).hasClass("iframe")) {
					type = 'iframe';

				} else if (href.match(/#/)) {
					obj = href.substr(href.indexOf("#"));

					type = $(obj).length > 0 ? 'inline' : 'ajax';
				} else {
					type = 'ajax';
				}
			} else {
				type = 'inline';
			}

			selectedOpts.type	= type;
			selectedOpts.href	= href;
			selectedOpts.title	= title;

			if (selectedOpts.autoDimensions && selectedOpts.type !== 'iframe' && selectedOpts.type !== 'swf') {
				selectedOpts.width		= 'auto';
				selectedOpts.height		= 'auto';
			}

			if (selectedOpts.modal) {
				selectedOpts.overlayShow		= true;
				selectedOpts.hideOnOverlayClick	= false;
				selectedOpts.hideOnContentClick	= false;
				selectedOpts.enableEscapeButton	= false;
				selectedOpts.showCloseButton	= false;
			}

			if ($.isFunction(selectedOpts.onStart)) {
				if (selectedOpts.onStart(selectedArray, selectedIndex, selectedOpts) === false) {
					busy = false;
					return;
				}
			}

			tmp.css('padding', (shadow + selectedOpts.padding + selectedOpts.margin));

			$('.fancybox-inline-tmp').unbind('fancybox-cancel').bind('fancybox-change', function() {
				$(this).replaceWith(inner.children());
			});

			switch (type) {
				case 'html' :
					tmp.html( selectedOpts.content );
					fancybox_process_inline();
				break;

				case 'inline' :
					$('<div class="fancybox-inline-tmp" />').hide().insertBefore( $(obj) ).bind('fancybox-cleanup', function() {
						$(this).replaceWith(inner.children());
					}).bind('fancybox-cancel', function() {
						$(this).replaceWith(tmp.children());
					});

					$(obj).appendTo(tmp);

					fancybox_process_inline();
				break;

				case 'image':
					busy = false;

					$.fancybox.showActivity();

					imgPreloader = new Image();

					imgPreloader.onerror = function() {
						fancybox_error();
					};

					imgPreloader.onload = function() {
						imgPreloader.onerror = null;
						imgPreloader.onload = null;
						fancybox_process_image();
					};

					imgPreloader.src = href;
		
				break;

				case 'swf':
					str = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" width="' + selectedOpts.width + '" height="' + selectedOpts.height + '"><param name="movie" value="' + href + '"></param>';
					emb = '';
					
					$.each(selectedOpts.swf, function(name, val) {
						str += '<param name="' + name + '" value="' + val + '"></param>';
						emb += ' ' + name + '="' + val + '"';
					});

					str += '<embed src="' + href + '" type="application/x-shockwave-flash" width="' + selectedOpts.width + '" height="' + selectedOpts.height + '"' + emb + '></embed></object>';

					tmp.html(str);

					fancybox_process_inline();
				break;

				case 'ajax':
					selector	= href.split('#', 2);
					data		= selectedOpts.ajax.data || {};

					if (selector.length > 1) {
						href = selector[0];

						if (typeof data == "string") {
							data += '&selector=' + selector[1];
						} else {
							data.selector = selector[1];
						}
					}

					busy = false;
					$.fancybox.showActivity();

					ajaxLoader = $.ajax($.extend(selectedOpts.ajax, {
						url		: href,
						data	: data,
						error	: fancybox_error,
						success : function(data, textStatus, XMLHttpRequest) {
							if (ajaxLoader.status == 200) {
								tmp.html( data );
								fancybox_process_inline();
							}
						}
					}));

				break;

				case 'iframe' :
					$('<iframe id="fancybox-frame" name="fancybox-frame' + new Date().getTime() + '" frameborder="0" hspace="0" scrolling="' + selectedOpts.scrolling + '" src="' + selectedOpts.href + '"></iframe>').appendTo(tmp);
					fancybox_show();
				break;
			}
		},

		fancybox_animate_loading = function() {
			if (!loading.is(':visible')){
				clearInterval(loadingTimer);
				return;
			}

			$('div', loading).css('top', (loadingFrame * -40) + 'px');

			loadingFrame = (loadingFrame + 1) % 12;
		},

		fancybox_init = function() {
			if ($("#fancybox-wrap").length) {
				return;
			}

			$('body').append(
				tmp			= $('<div id="fancybox-tmp"></div>'),
				loading		= $('<div id="fancybox-loading"><div></div></div>'),
				overlay		= $('<div id="fancybox-overlay"></div>'),
				wrap		= $('<div id="fancybox-wrap"></div>')
			);

			if (!$.support.opacity) {
				wrap.addClass('fancybox-ie');
				loading.addClass('fancybox-ie');
			}

			outer = $('<div id="fancybox-outer"></div>')
				.append('<div class="fancy-bg" id="fancy-bg-n"></div><div class="fancy-bg" id="fancy-bg-ne"></div><div class="fancy-bg" id="fancy-bg-e"></div><div class="fancy-bg" id="fancy-bg-se"></div><div class="fancy-bg" id="fancy-bg-s"></div><div class="fancy-bg" id="fancy-bg-sw"></div><div class="fancy-bg" id="fancy-bg-w"></div><div class="fancy-bg" id="fancy-bg-nw"></div>')
				.appendTo( wrap );

			outer.append(
				inner		= $('<div id="fancybox-inner"></div>'),
				close		= $('<a id="fancybox-close"></a>'),

				nav_left	= $('<a href="javascript:;" id="fancybox-left"><span class="fancy-ico" id="fancybox-left-ico"></span></a>'),
				nav_right	= $('<a href="javascript:;" id="fancybox-right"><span class="fancy-ico" id="fancybox-right-ico"></span></a>')
			);

			close.click($.fancybox.close);
			loading.click($.fancybox.cancel);

			nav_left.click(function(e) {
				e.preventDefault();
				$.fancybox.prev();
			});

			nav_right.click(function(e) {
				e.preventDefault();
				$.fancybox.next();
			});

			if (isIE6) {
				overlay.get(0).style.setExpression('height',	"document.body.scrollHeight > document.body.offsetHeight ? document.body.scrollHeight : document.body.offsetHeight + 'px'");
				loading.get(0).style.setExpression('top',		"(-20 + (document.documentElement.clientHeight ? document.documentElement.clientHeight/2 : document.body.clientHeight/2 ) + ( ignoreMe = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop )) + 'px'");

				outer.prepend('<iframe id="fancybox-hide-sel-frame" src="javascript:\'\';" scrolling="no" frameborder="0" ></iframe>');
			}
		};

	/*
	 * Public methods 
	 */

	$.fn.fancybox = function(options) {
		$(this)
			.data('fancybox', $.extend({}, options, ($.metadata ? $(this).metadata() : {})))
			.unbind('click.fb').bind('click.fb', function(e) {
				e.preventDefault();

				if (busy) {
					return;
				}

				busy = true;

				$(this).blur();

				selectedArray	= [];
				selectedIndex	= 0;

				var rel = $(this).attr('rel') || '';

				if (!rel || rel == '' || rel === 'nofollow') {
					selectedArray.push(this);

				} else {
					selectedArray	= $("a[rel=" + rel + "], area[rel=" + rel + "]");
					selectedIndex	= selectedArray.index( this );
				}

				fancybox_start();

				return false;
			});

		return this;
	};

	$.fancybox = function(obj) {
		if (busy) {
			return;
		}

		busy = true;

		var opts = typeof arguments[1] !== 'undefined' ? arguments[1] : {};

		selectedArray	= [];
		selectedIndex	= opts.index || 0;

		if ($.isArray(obj)) {
			for (var i = 0, j = obj.length; i < j; i++) {
				if (typeof obj[i] == 'object') {
					$(obj[i]).data('fancybox', $.extend({}, opts, obj[i]));
				} else {
					obj[i] = $({}).data('fancybox', $.extend({content : obj[i]}, opts));
				}
			}

			selectedArray = jQuery.merge(selectedArray, obj);

		} else {
			if (typeof obj == 'object') {
				$(obj).data('fancybox', $.extend({}, opts, obj));
			} else {
				obj = $({}).data('fancybox', $.extend({content : obj}, opts));
			}

			selectedArray.push(obj);
		}

		if (selectedIndex > selectedArray.length || selectedIndex < 0) {
			selectedIndex = 0;
		}

		fancybox_start();
	};

	$.fancybox.showActivity = function() {
		clearInterval(loadingTimer);

		loading.show();
		loadingTimer = setInterval(fancybox_animate_loading, 66);
	};

	$.fancybox.hideActivity = function() {
		loading.hide();
	};

	$.fancybox.next = function() {
		return $.fancybox.pos( currentIndex + 1);
	};
	
	$.fancybox.prev = function() {
		return $.fancybox.pos( currentIndex - 1);
	};

	$.fancybox.pos = function(pos) {
		if (busy) {
			return;
		}

		pos = parseInt(pos, 10);

		if (pos > -1 && currentArray.length > pos) {
			selectedIndex = pos;
			fancybox_start();
		}

		if (currentOpts.cyclic && currentArray.length > 1 && pos < 0) {
			selectedIndex = currentArray.length - 1;
			fancybox_start();
		}

		if (currentOpts.cyclic && currentArray.length > 1 && pos >= currentArray.length) {
			selectedIndex = 0;
			fancybox_start();
		}

		return;
	};

	$.fancybox.cancel = function() {
		if (busy) {
			return;
		}

		busy = true;

		$.event.trigger('fancybox-cancel');

		fancybox_abort();

		if (selectedOpts && $.isFunction(selectedOpts.onCancel)) {
			selectedOpts.onCancel(selectedArray, selectedIndex, selectedOpts);
		}

		busy = false;
	};

	// Note: within an iframe use - parent.$.fancybox.close();
	$.fancybox.close = function() {
		if (busy || wrap.is(':hidden')) {
			return;
		}

		busy = true;

		if (currentOpts && $.isFunction(currentOpts.onCleanup)) {
			if (currentOpts.onCleanup(currentArray, currentIndex, currentOpts) === false) {
				busy = false;
				return;
			}
		}

		fancybox_abort();

		$(close.add( nav_left ).add( nav_right )).hide();

		$('#fancybox-title').remove();

		wrap.add(inner).add(overlay).unbind();

		$(window).unbind("resize.fb scroll.fb");
		$(document).unbind('keydown.fb');

		function _cleanup() {
			overlay.fadeOut('fast');

			wrap.hide();

			$.event.trigger('fancybox-cleanup');

			inner.empty();

			if ($.isFunction(currentOpts.onClosed)) {
				currentOpts.onClosed(currentArray, currentIndex, currentOpts);
			}

			currentArray	= selectedOpts	= [];
			currentIndex	= selectedIndex	= 0;
			currentOpts		= selectedOpts	= {};

			busy = false;
		}

		inner.css('overflow', 'hidden');

		if (currentOpts.transitionOut == 'elastic') {
			start_pos = fancybox_get_zoom_from();

			var pos = wrap.position();

			final_pos = {
				top		:	pos.top ,
				left	:	pos.left,
				width	:	wrap.width(),
				height	:	wrap.height()
			};

			if (currentOpts.opacity) {
				final_pos.opacity = 1;
			}

			fx.prop = 1;

			$(fx).animate({ prop: 0 }, {
				 duration	: currentOpts.speedOut,
				 easing		: currentOpts.easingOut,
				 step		: fancybox_draw,
				 complete	: _cleanup
			});

		} else {
			wrap.fadeOut( currentOpts.transitionOut == 'none' ? 0 : currentOpts.speedOut, _cleanup);
		}
	};

	$.fancybox.resize = function() {
		var c, h;
		
		if (busy || wrap.is(':hidden')) {
			return;
		}

		busy = true;

		c = inner.wrapInner("<div style='overflow:auto'></div>").children();
		h = c.height();

		wrap.css({height:	h + (currentOpts.padding * 2) + titleh});
		inner.css({height:	h});

		c.replaceWith(c.children());

		$.fancybox.center();
	};

	$.fancybox.center = function() {
		busy = true;

		var view	= fancybox_get_viewport(),
			margin	= currentOpts.margin,
			to		= {};

		to.top	= view[3] + ((view[1] - ((wrap.height() - titleh) + (shadow * 2 ))) * 0.5);
		to.left	= view[2] + ((view[0] - (wrap.width() + (shadow * 2 ))) * 0.5);

		to.top	= Math.max(view[3] + margin, to.top);
		to.left	= Math.max(view[2] + margin, to.left);

		wrap.css(to);

		busy = false;
	};

	$.fn.fancybox.defaults = {
		padding				:	10,
		margin				:	20,
		opacity				:	false,
		modal				:	false,
		cyclic				:	false,
		scrolling			:	'auto',	// 'auto', 'yes' or 'no'

		width				:	560,
		height				:	340,

		autoScale			:	true,
		autoDimensions		:	true,
		centerOnScroll		:	false,

		ajax				:	{},
		swf					:	{ wmode: 'transparent' },

		hideOnOverlayClick	:	true,
		hideOnContentClick	:	false,

		overlayShow			:	true,
		overlayOpacity		:	0.3,
		overlayColor		:	'#666',

		titleShow			:	true,
		titlePosition		:	'outside',	// 'outside', 'inside' or 'over'
		titleFormat			:	null,

		transitionIn		:	'fade',	// 'elastic', 'fade' or 'none'
		transitionOut		:	'fade',	// 'elastic', 'fade' or 'none'

		speedIn				:	300,
		speedOut			:	300,

		changeSpeed			:	300,
		changeFade			:	'fast',

		easingIn			:	'swing',
		easingOut			:	'swing',

		showCloseButton		:	true,
		showNavArrows		:	true,
		enableEscapeButton	:	true,

		onStart				:	null,
		onCancel			:	null,
		onComplete			:	null,
		onCleanup			:	null,
		onClosed			:	null
	};

	$(document).ready(function() {
		fancybox_init();
	});

})(jQuery);;
(function(a){a.uniform={options:{selectClass:"selector",radioClass:"radio",checkboxClass:"checker",fileClass:"uploader",filenameClass:"filename",fileBtnClass:"action",fileDefaultText:"No file selected",fileBtnText:"Choose File",checkedClass:"checked",focusClass:"focus",disabledClass:"disabled",activeClass:"active",hoverClass:"hover",useID:true,idPrefix:"uniform",resetSelector:false},elements:[]};if(a.browser.msie&&a.browser.version<7){a.support.selectOpacity=false}else{a.support.selectOpacity=true}a.fn.uniform=function(c){c=a.extend(a.uniform.options,c);var e=this;if(c.resetSelector!=false){a(c.resetSelector).mouseup(function(){function i(){a.uniform.update(e)}setTimeout(i,10)})}function b(k){var l=a("<div />"),i=a("<span />");l.addClass(c.selectClass);if(c.useID){l.attr("id",c.idPrefix+"-"+k.attr("id"))}var j=k.find(":selected:first");if(j.length==0){j=k.find("option:first")}i.html(j.text());k.css("opacity",0);k.wrap(l);k.before(i);l=k.parent("div");i=k.siblings("span");k.bind({"change.uniform":function(){i.text(k.find(":selected").text());l.removeClass(c.activeClass)},"focus.uniform":function(){l.addClass(c.focusClass)},"blur.uniform":function(){l.removeClass(c.focusClass);l.removeClass(c.activeClass)},"mousedown.uniform":function(){l.addClass(c.activeClass)},"mouseup.uniform":function(){l.removeClass(c.activeClass)},"click.uniform":function(){l.removeClass(c.activeClass)},"mouseenter.uniform":function(){l.addClass(c.hoverClass)},"mouseleave.uniform":function(){l.removeClass(c.hoverClass)},"keyup.uniform":function(){i.text(k.find(":selected").text())}});if(a(k).attr("disabled")){l.addClass(c.disabledClass)}a.uniform.noSelect(i);h(k)}function d(j){var k=a("<div />"),i=a("<span />");k.addClass(c.checkboxClass);if(c.useID){k.attr("id",c.idPrefix+"-"+j.attr("id"))}a(j).wrap(k);a(j).wrap(i);i=j.parent();k=i.parent();a(j).css("opacity",0).bind({"focus.uniform":function(){k.addClass(c.focusClass)},"blur.uniform":function(){k.removeClass(c.focusClass)},"click.uniform":function(){if(!a(j).attr("checked")){i.removeClass(c.checkedClass)}else{i.addClass(c.checkedClass)}},"mousedown.uniform":function(){k.addClass(c.activeClass)},"mouseup.uniform":function(){k.removeClass(c.activeClass)},"mouseenter.uniform":function(){k.addClass(c.hoverClass)},"mouseleave.uniform":function(){k.removeClass(c.hoverClass)}});if(a(j).attr("checked")){i.addClass(c.checkedClass)}if(a(j).attr("disabled")){k.addClass(c.disabledClass)}h(j)}function f(j){var k=a("<div />"),i=a("<span />");k.addClass(c.radioClass);if(c.useID){k.attr("id",c.idPrefix+"-"+j.attr("id"))}a(j).wrap(k);a(j).wrap(i);i=j.parent();k=i.parent();a(j).css("opacity",0).bind({"focus.uniform":function(){k.addClass(c.focusClass)},"blur.uniform":function(){k.removeClass(c.focusClass)},"click.uniform":function(){if(!a(j).attr("checked")){i.removeClass(c.checkedClass)}else{a("."+c.radioClass+" span."+c.checkedClass+":has([name='"+a(j).attr("name")+"'])").removeClass(c.checkedClass);i.addClass(c.checkedClass)}},"mousedown.uniform":function(){if(!a(j).is(":disabled")){k.addClass(c.activeClass)}},"mouseup.uniform":function(){k.removeClass(c.activeClass)},"mouseenter.uniform":function(){k.addClass(c.hoverClass)},"mouseleave.uniform":function(){k.removeClass(c.hoverClass)}});if(a(j).attr("checked")){i.addClass(c.checkedClass)}if(a(j).attr("disabled")){k.addClass(c.disabledClass)}h(j)}function g(n){var l=a(n);var o=a("<div />"),m=a("<span>"+c.fileDefaultText+"</span>"),j=a("<span>"+c.fileBtnText+"</span>");o.addClass(c.fileClass);m.addClass(c.filenameClass);j.addClass(c.fileBtnClass);if(c.useID){o.attr("id",c.idPrefix+"-"+l.attr("id"))}l.wrap(o);l.after(j);l.after(m);o=l.closest("div");m=l.siblings("."+c.filenameClass);j=l.siblings("."+c.fileBtnClass);if(!l.attr("size")){var i=o.width();l.attr("size",i/10)}var k=function(){var p=l.val();if(p===""){p=c.fileDefaultText}else{p=p.split(/[\/\\]+/);p=p[(p.length-1)]}m.text(p)};k();l.css("opacity",0).bind({"focus.uniform":function(){o.addClass(c.focusClass)},"blur.uniform":function(){o.removeClass(c.focusClass)},"mousedown.uniform":function(){if(!a(n).is(":disabled")){o.addClass(c.activeClass)}},"mouseup.uniform":function(){o.removeClass(c.activeClass)},"mouseenter.uniform":function(){o.addClass(c.hoverClass)},"mouseleave.uniform":function(){o.removeClass(c.hoverClass)}});if(a.browser.msie){l.bind("click.uniform.ie7",function(){setTimeout(k,0)})}else{l.bind("change.uniform",k)}if(l.attr("disabled")){o.addClass(c.disabledClass)}a.uniform.noSelect(m);a.uniform.noSelect(j);h(n)}a.uniform.restore=function(i){a(i).each(function(){if(a(this).is(":checkbox")){a(this).unwrap().unwrap()}else{if(a(this).is("select")){a(this).siblings("span").remove();a(this).unwrap()}else{if(a(this).is(":radio")){a(this).unwrap().unwrap()}else{if(a(this).is(":file")){a(this).siblings("span").remove();a(this).unwrap()}}}}a(this).unbind(".uniform");a(this).css("opacity","1");var j=a.inArray(a(i),a.uniform.elements);a.uniform.elements.splice(j,1)})};function h(i){i=a(i).get();if(i.length>1){a.each(i,function(j,k){a.uniform.elements.push(k)})}else{a.uniform.elements.push(i)}}a.uniform.noSelect=function(i){function j(){return false}a(i).each(function(){this.onselectstart=this.ondragstart=j;a(this).mousedown(j).css({MozUserSelect:"none"})})};a.uniform.update=function(i){if(i==undefined){i=a(a.uniform.elements)}i=a(i);i.each(function(){var k=a(this);if(k.is("select")){var j=k.siblings("span");var m=k.parent("div");m.removeClass(c.hoverClass+" "+c.focusClass+" "+c.activeClass);j.html(k.find(":selected").text());if(k.is(":disabled")){m.addClass(c.disabledClass)}else{m.removeClass(c.disabledClass)}}else{if(k.is(":checkbox")){var j=k.closest("span");var m=k.closest("div");m.removeClass(c.hoverClass+" "+c.focusClass+" "+c.activeClass);j.removeClass(c.checkedClass);if(k.is(":checked")){j.addClass(c.checkedClass)}if(k.is(":disabled")){m.addClass(c.disabledClass)}else{m.removeClass(c.disabledClass)}}else{if(k.is(":radio")){var j=k.closest("span");var m=k.closest("div");m.removeClass(c.hoverClass+" "+c.focusClass+" "+c.activeClass);j.removeClass(c.checkedClass);if(k.is(":checked")){j.addClass(c.checkedClass)}if(k.is(":disabled")){m.addClass(c.disabledClass)}else{m.removeClass(c.disabledClass)}}else{if(k.is(":file")){var m=k.parent("div");var l=k.siblings(c.filenameClass);btnTag=k.siblings(c.fileBtnClass);m.removeClass(c.hoverClass+" "+c.focusClass+" "+c.activeClass);l.text(k.val());if(k.is(":disabled")){m.addClass(c.disabledClass)}else{m.removeClass(c.disabledClass)}}}}}})};return this.each(function(){if(a.support.selectOpacity){var i=a(this);if(i.is("select")){if(i.attr("multiple")!=true){if(i.attr("size")==undefined||i.attr("size")<=1){b(i)}}}else{if(i.is(":checkbox")){d(i)}else{if(i.is(":radio")){f(i)}else{if(i.is(":file")){g(i)}}}}}})}})(jQuery);;
/**
 * Enable uniform for specific form elements
 */
(function($) {
  Drupal.behaviors.jquery_uniform = {
    attach : function() {
      $("select, input:file, button").once('uniform', function() {
         $(this).uniform();
      });
    }
  };
})(jQuery);  ;
/*
 * 	Easy Slider 1.7 - jQuery plugin
 *	written by Alen Grakalic	
 *	http://cssglobe.com/post/4004/easy-slider-15-the-easiest-jquery-plugin-for-sliding
 *
 *	Copyright (c) 2009 Alen Grakalic (http://cssglobe.com)
 *	Dual licensed under the MIT (MIT-LICENSE.txt)
 *	and GPL (GPL-LICENSE.txt) licenses.
 *
 *	Built for jQuery library
 *	http://jquery.com
 *
 */
 
/*
 *	markup example for $("#slider").easySlider();
 *	
 * 	<div id="slider">
 *		<ul>
 *			<li><img src="images/01.jpg" alt="" /></li>
 *			<li><img src="images/02.jpg" alt="" /></li>
 *			<li><img src="images/03.jpg" alt="" /></li>
 *			<li><img src="images/04.jpg" alt="" /></li>
 *			<li><img src="images/05.jpg" alt="" /></li>
 *		</ul>
 *	</div>
 *
 */

(function($) {

	$.fn.easySlider = function(options){
	  
		// default configuration properties
		var defaults = {			
			prevId: 		'prevBtn',
			prevText: 		'Previous',
			nextId: 		'nextBtn',	
			nextText: 		'Next',
			controlsShow:	true,
			controlsBefore:	'',
			controlsAfter:	'',	
			controlsFade:	true,
			firstId: 		'firstBtn',
			firstText: 		'First',
			firstShow:		false,
			lastId: 		'lastBtn',	
			lastText: 		'Last',
			lastShow:		false,				
			vertical:		false,
			speed: 			800,
			auto:			false,
			pause:			2000,
			continuous:		false, 
			numeric: 		false,
			numericId: 		'controls'
		}; 
		
		var options = $.extend(defaults, options);  
				
		this.each(function() {  
			var obj = $(this); 				
			var s = $("li", obj).length;
			var w = $("li", obj).width(); 
			var h = $("li", obj).height(); 
			var clickable = true;
			obj.width(w); 
			obj.height(h); 
			obj.css("overflow","hidden");
			var ts = s-1;
			var t = 0;
			$("ul", obj).css('width',s*w);			
			
			if(options.continuous){
				$("ul", obj).prepend($("ul li:last-child", obj).clone().css("margin-left","-"+ w +"px"));
				$("ul", obj).append($("ul li:nth-child(2)", obj).clone());
				$("ul", obj).css('width',(s+1)*w);
			};				
			
			if(!options.vertical) $("li", obj).css('float','left');
								
			if(options.controlsShow){
				var html = options.controlsBefore;				
				if(options.numeric){
					html += '<ol id="'+ options.numericId +'"></ol>';
				} else {
					if(options.firstShow) html += '<span id="'+ options.firstId +'"><a href=\"javascript:void(0);\">'+ options.firstText +'</a></span>';
					html += ' <span id="'+ options.prevId +'"><a href=\"javascript:void(0);\">'+ options.prevText +'</a></span>';
					html += ' <span id="'+ options.nextId +'"><a href=\"javascript:void(0);\">'+ options.nextText +'</a></span>';
					if(options.lastShow) html += ' <span id="'+ options.lastId +'"><a href=\"javascript:void(0);\">'+ options.lastText +'</a></span>';				
				};
				
				html += options.controlsAfter;						
				$(obj).after(html);										
			};
			
			if(options.numeric){									
				for(var i=0;i<s;i++){						
					$(document.createElement("li"))
						.attr('id',options.numericId + (i+1))
						.html('<a rel='+ i +' href=\"javascript:void(0);\">'+ (i+1) +'</a>')
						.appendTo($("#"+ options.numericId))
						.click(function(){							
							animate($("a",$(this)).attr('rel'),true);
						}); 												
				};							
			} else {
				$("a","#"+options.nextId).click(function(){		
					animate("next",true);
				});
				$("a","#"+options.prevId).click(function(){		
					animate("prev",true);				
				});	
				$("a","#"+options.firstId).click(function(){		
					animate("first",true);
				});				
				$("a","#"+options.lastId).click(function(){		
					animate("last",true);				
				});				
			};
			
			function setCurrent(i){
				i = parseInt(i)+1;
				$("li", "#" + options.numericId).removeClass("current");
				$("li#" + options.numericId + i).addClass("current");
			};
			
			function adjust(){
				if(t>ts) t=0;		
				if(t<0) t=ts;	
				if(!options.vertical) {
					$("ul",obj).css("margin-left",(t*w*-1));
				} else {
					$("ul",obj).css("margin-left",(t*h*-1));
				}
				clickable = true;
				if(options.numeric) setCurrent(t);
			};
			
			function animate(dir,clicked){
				if (clickable){
					clickable = false;
					var ot = t;				
					switch(dir){
						case "next":
							t = (ot>=ts) ? (options.continuous ? t+1 : ts) : t+1;						
							break; 
						case "prev":
							t = (t<=0) ? (options.continuous ? t-1 : 0) : t-1;
							break; 
						case "first":
							t = 0;
							break; 
						case "last":
							t = ts;
							break; 
						default:
							t = dir;
							break; 
					};	
					var diff = Math.abs(ot-t);
					var speed = diff*options.speed;						
					if(!options.vertical) {
						p = (t*w*-1);
						$("ul",obj).animate(
							{ marginLeft: p }, 
							{ queue:false, duration:speed, complete:adjust }
						);				
					} else {
						p = (t*h*-1);
						$("ul",obj).animate(
							{ marginTop: p }, 
							{ queue:false, duration:speed, complete:adjust }
						);					
					};
					
					if(!options.continuous && options.controlsFade){					
						if(t==ts){
							$("a","#"+options.nextId).hide();
							$("a","#"+options.lastId).hide();
						} else {
							$("a","#"+options.nextId).show();
							$("a","#"+options.lastId).show();					
						};
						if(t==0){
							$("a","#"+options.prevId).hide();
							$("a","#"+options.firstId).hide();
						} else {
							$("a","#"+options.prevId).show();
							$("a","#"+options.firstId).show();
						};					
					};				
					
					if(clicked) clearTimeout(timeout);
					if(options.auto && dir=="next" && !clicked){;
						timeout = setTimeout(function(){
							animate("next",false);
						},diff*options.speed+options.pause);
					};
			
				};
				
			};
			// init
			var timeout;
			if(options.auto){;
				timeout = setTimeout(function(){
					animate("next",false);
				},options.pause);
			};		
			
			if(options.numeric) setCurrent(0);
		
			if(!options.continuous && options.controlsFade){					
				$("a","#"+options.prevId).hide();
				$("a","#"+options.firstId).hide();				
			};				
			
		});
	  
	};

})(jQuery);



;
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
})(jQuery);;
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
})(jQuery);;
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
})(jQuery);;
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
})(jQuery);
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
})(jQuery);;
// jQuery SWFObject v1.1.1 MIT/GPL @jon_neal
// http://jquery.thewikies.com/swfobject

(function($, flash, Plugin) {
 var OBJECT = 'object',
  ENCODE = true;

 function _compareArrayIntegers(a, b) {
  var x = (a[0] || 0) - (b[0] || 0);

  return x > 0 || (
   !x &&
   a.length > 0 &&
   _compareArrayIntegers(a.slice(1), b.slice(1))
  );
 }

 function _objectToArguments(obj) {
  if (typeof obj != OBJECT) {
   return obj;
  }

  var arr = [],
   str = '';

  for (var i in obj) {
   if (typeof obj[i] == OBJECT) {
    str = _objectToArguments(obj[i]);
   }
   else {
    str = [i, (ENCODE) ? encodeURI(obj[i]) : obj[i]].join('=');
   }

   arr.push(str);
  }

  return arr.join('&');
 }

 function _objectFromObject(obj) {
  var arr = [];

  for (var i in obj) {
   if (obj[i]) {
    arr.push([i, '="', obj[i], '"'].join(''));
   }
  }

  return arr.join(' ');
 }

 function _paramsFromObject(obj) {
  var arr = [];

  for (var i in obj) {
   arr.push([
    '<param name="', i,
    '" value="', _objectToArguments(obj[i]), '" />'
   ].join(''));
  }

  return arr.join('');
 }

 try {
  var flashVersion = Plugin.description || (function () {
   return (
    new Plugin('ShockwaveFlash.ShockwaveFlash')
   ).GetVariable('$version');
  }());
 }
 catch (e) {
  flashVersion = 'Unavailable';
 }

 var flashVersionMatchVersionNumbers = flashVersion.match(/\d+/g) || [0];

 $[flash] = {
  available: flashVersionMatchVersionNumbers[0] > 0,

  activeX: Plugin && !Plugin.name,

  version: {
   original: flashVersion,
   array: flashVersionMatchVersionNumbers,
   string: flashVersionMatchVersionNumbers.join('.'),
   major: parseInt(flashVersionMatchVersionNumbers[0], 10) || 0,
   minor: parseInt(flashVersionMatchVersionNumbers[1], 10) || 0,
   release: parseInt(flashVersionMatchVersionNumbers[2], 10) || 0
  },

  hasVersion: function (version) {
   var versionArray = (/string|number/.test(typeof version))
    ? version.toString().split('.')
    : (/object/.test(typeof version))
     ? [version.major, version.minor]
     : version || [0, 0];

   return _compareArrayIntegers(
    flashVersionMatchVersionNumbers,
    versionArray
   );
  },

  encodeParams: true,

  expressInstall: 'expressInstall.swf',
  expressInstallIsActive: false,

  create: function (obj) {
   var instance = this;

   if (
    !obj.swf ||
    instance.expressInstallIsActive ||
    (!instance.available && !obj.hasVersionFail)
   ) {
    return false;
   }

   if (!instance.hasVersion(obj.hasVersion || 1)) {
    instance.expressInstallIsActive = true;

    if (typeof obj.hasVersionFail == 'function') {
     if (!obj.hasVersionFail.apply(obj)) {
      return false;
     }
    }

    obj = {
     swf: obj.expressInstall || instance.expressInstall,
     height: 137,
     width: 214,
     flashvars: {
      MMredirectURL: location.href,
      MMplayerType: (instance.activeX)
       ? 'ActiveX' : 'PlugIn',
      MMdoctitle: document.title.slice(0, 47) +
       ' - Flash Player Installation'
     }
    };
   }

   attrs = {
    data: obj.swf,
    type: 'application/x-shockwave-flash',
    id: obj.id || 'flash_' + Math.floor(Math.random() * 999999999),
    width: obj.width || 320,
    height: obj.height || 180,
    style: obj.style || ''
   };

   ENCODE = typeof obj.useEncode !== 'undefined' ? obj.useEncode : instance.encodeParams;

   obj.movie = obj.swf;
   obj.wmode = obj.wmode || 'opaque';

   delete obj.fallback;
   delete obj.hasVersion;
   delete obj.hasVersionFail;
   delete obj.height;
   delete obj.id;
   delete obj.swf;
   delete obj.useEncode;
   delete obj.width;

   var flashContainer = document.createElement('div');

   flashContainer.innerHTML = [
    '<object ', _objectFromObject(attrs), '>',
    _paramsFromObject(obj),
    '</object>'
   ].join('');

   return flashContainer.firstChild;
  }
 };

 $.fn[flash] = function (options) {
  var $this = this.find(OBJECT).andSelf().filter(OBJECT);

  if (/string|object/.test(typeof options)) {
   this.each(
    function () {
     var $this = $(this),
      flashObject;

     options = (typeof options == OBJECT) ? options : {
      swf: options
     };

     options.fallback = this;

     flashObject = $[flash].create(options);

     if (flashObject) {
      $this.children().remove();

      $this.html(flashObject);
     }
    }
   );
  }

  if (typeof options == 'function') {
   $this.each(
    function () {
     var instance = this,
     jsInteractionTimeoutMs = 'jsInteractionTimeoutMs';

     instance[jsInteractionTimeoutMs] =
      instance[jsInteractionTimeoutMs] || 0;

     if (instance[jsInteractionTimeoutMs] < 660) {
      if (instance.clientWidth || instance.clientHeight) {
       options.call(instance);
      }
      else {
       setTimeout(
        function () {
         $(instance)[flash](options);
        },
        instance[jsInteractionTimeoutMs] + 66
       );
      }
     }
    }
   );
  }

  return $this;
 };
}(
 jQuery,
 'flash',
 navigator.plugins['Shockwave Flash'] || window.ActiveXObject
));;
(function ($) {

$(document).ready(function() {

  // Accepts a string; returns the string with regex metacharacters escaped. The returned string
  // can safely be used at any point within a regex to match the provided literal string. Escaped
  // characters are [ ] { } ( ) * + ? - . , \ ^ $ # and whitespace. The character | is excluded
  // in this function as it's used to separate the domains names.
  RegExp.escapeDomains = function(text) {
    return (text) ? text.replace(/[-[\]{}()*+?.,\\^$#\s]/g, "\\$&") : '';
  }

  // Attach onclick event to document only and catch clicks on all elements.
  $(document.body).click(function(event) {
    // Catch the closest surrounding link of a clicked element.
    $(event.target).closest("a,area").each(function() {

      var ga = Drupal.settings.googleanalytics;
      // Expression to check for absolute internal links.
      var isInternal = new RegExp("^(https?):\/\/" + window.location.host, "i");
      // Expression to check for special links like gotwo.module /go/* links.
      var isInternalSpecial = new RegExp("(\/go\/.*)$", "i");
      // Expression to check for download links.
      var isDownload = new RegExp("\\.(" + ga.trackDownloadExtensions + ")$", "i");
      // Expression to check for the sites cross domains.
      var isCrossDomain = new RegExp("^(https?|ftp|news|nntp|telnet|irc|ssh|sftp|webcal):\/\/.*(" + RegExp.escapeDomains(ga.trackCrossDomains) + ")", "i");

      // Is the clicked URL internal?
      if (isInternal.test(this.href)) {
        // Is download tracking activated and the file extension configured for download tracking?
        if (ga.trackDownload && isDownload.test(this.href)) {
          // Download link clicked.
          var extension = isDownload.exec(this.href);
          _gaq.push(["_trackEvent", "Downloads", extension[1].toUpperCase(), this.href.replace(isInternal, '')]);
        }
        else if (isInternalSpecial.test(this.href)) {
          // Keep the internal URL for Google Analytics website overlay intact.
          _gaq.push(["_trackPageview", this.href.replace(isInternal, '')]);
        }
      }
      else {
        if (ga.trackMailto && $(this).is("a[href^=mailto:],area[href^=mailto:]")) {
          // Mailto link clicked.
          _gaq.push(["_trackEvent", "Mails", "Click", this.href.substring(7)]);
        }
        else if (ga.trackOutbound && this.href) {
          if (ga.trackDomainMode == 2 && isCrossDomain.test(this.href)) {
            // Top-level cross domain clicked. document.location is handled by _link internally.
            _gaq.push(["_link", this.href]);
          }
          else if (ga.trackOutboundAsPageview) {
            // Track all external links as page views after URL cleanup.
            // Currently required, if click should be tracked as goal.
            _gaq.push(["_trackPageview", '/outbound/' + this.href.replace(/^(https?|ftp|news|nntp|telnet|irc|ssh|sftp|webcal):\/\//i, '').split('/').join('--')]);
          }
          else {
            // External link clicked.
            _gaq.push(["_trackEvent", "Outbound links", "Click", this.href]);
          }
        }
      }
    });
  });
});

})(jQuery);
;
/**
 * AMS javascript for Drupal
 */

(function($) {
  Drupal.behaviors.ams = {
    attach : function() {
      ams = Drupal.settings.ams;
      ams_zones_timeout = [];
      ams_zones_index = [];
      ams_zones_count = [];
      for (var $zone in ams.zones) {
        if (ams.banners.length<=1) continue;
        if (ams.banners[$zone].length<=1) continue;
        if (ams.zones[$zone].rotation) {
          var $rotation = ams.zones[$zone].rotation;
          if (ams.banners[$zone][0].rotation>$rotation)
            $rotation = ams.banners[$zone][0].rotation;
          ams_zones_count[$zone] = 1;
          ams_zones_timeout[$zone] = setTimeout('ams_display_zone('+$zone+');', $rotation*1000);
        }
        ams_zones_index[$zone] = 1;
      }
    }
  };
})(jQuery);

function ams_display_zone($zone) {
  if (ams.banners[$zone]!=undefined) {
    var $wrapper = jQuery(ams.zones[$zone].wrapper);
    var $index = ams_zones_index[$zone];
    var $banner = ams.banners[$zone][$index];
    jQuery($wrapper).data('banner', $banner);
    if ($banner.type=='image') {
      jQuery($wrapper).fadeOut('fast', function() {
        var $banner = jQuery(this).data('banner');
        var $target = '';
        if ($banner.target!=undefined && $banner.target) $target = 'target="_blank"';
        jQuery(this).html('<a href="'+$banner.url+'" '+$target+'><img src="'+$banner.media+'" width="'+$banner.width+'" height="'+$banner.height+'"\/><\/a>').fadeIn();
      });
    } else if ($banner.type=='flash') {
        jQuery($wrapper).flash({
          swf: $banner.media + '?&clickTAG='+escape($banner.url),
          width: $banner.width,
          height: $banner.height,
          wmode: "opaque"
        });
    } else if ($banner.type=='html') {
      jQuery($wrapper).fadeOut('fast', function() {
        var $banner = jQuery(this).data('banner');
        jQuery(this).html($banner.data).fadeIn();
      });
    } else return;
    
   	ams_zones_count[$zone]++;
   	if (ams.rotation && ams_zones_count[$zone]>=ams.rotation) {
   		return clearTimeout(ams_zones_timeout[$zone]);
   	}
    // ping back
   	if (!ams.expose)
   	  jQuery.ajax({type:'GET', url:ams.ping+'/'+$banner.cid+'/'+$banner.bid});
   	else {
      //var $src = ams.api + '/' + Math.floor(Math.random()*100000) + '?ams=banner&cid=' + $banner.cid + '&bid=' + $banner.bid + '&s=' + Math.random();
      var $src = ams.api + '?ams=banner&cid=' + $banner.cid + '&bid=' + $banner.bid + '&s=' + Math.random();
      var $img_temp = jQuery('<img\/>').attr('src', $src);
    }
    
    // rotate to next banner
    var $rotation = ams.zones[$zone].rotation;
    if ($banner.rotation>$rotation) {
      $rotation = $banner.rotation;
    }
    ams_zones_timeout[$zone] = setTimeout('ams_display_zone('+$zone+');', $rotation*1000);
    
    $index++;
    if ($index>=ams.banners[$zone].length) $index = 0;
    ams_zones_index[$zone] = $index;
  }
};
