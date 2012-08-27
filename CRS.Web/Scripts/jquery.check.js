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
