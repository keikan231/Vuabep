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
}