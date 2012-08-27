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
