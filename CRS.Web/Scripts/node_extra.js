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
