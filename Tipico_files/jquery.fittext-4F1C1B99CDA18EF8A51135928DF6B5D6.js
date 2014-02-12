/*global jQuery */
/*!	
* FitText.js 1.1
*
* Copyright 2011, Dave Rupert http://daverupert.com
* Released under the WTFPL license 
* http://sam.zoy.org/wtfpl/
*
* Date: Thu May 05 14:23:00 2011 -0600
* Modified by metacab: February 24 2013
*/

(function( $ ){
	
	$.fn.fitText = function( cellWidth, minFontSize ) {
	
		return this.each(function(){
		
			// Store the object
			var $this = $(this); 
			
			// Resizer() resizes items
			var resizer = function () {
				if($this.width() > parseFloat(cellWidth) && parseFloat($this.css('font-size')) > parseFloat(minFontSize)) {
					$this.css('font-size', parseFloat($this.css('font-size')) - 1 + 'px');
					
					resizer();
				}
				$this.css('width', cellWidth);
			};
			
			// Call once to set.
			resizer();
					
			// Call on resize. Opera debounces their resize by default. 
			$(window).on('resize', resizer);
		});
	};

})( jQuery );


// fitText ('cell space', 'min. font-size')
jQuery(document).ready(function() {
	jQuery(".dyn_30").fitText('30px', '9px');
});