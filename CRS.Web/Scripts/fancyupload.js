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
