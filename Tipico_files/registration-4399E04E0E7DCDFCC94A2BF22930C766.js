var ebetReg = (function(){
	function fieldChanged() {
		var n = jQuery(this);
	    var v = (n.attr("type") == "checkbox") ? (n.is(':checked') ? 1 : 0) : n.val();
	    var f = n.attr("_name");
	    var statusFields = jQuery(this).parent().parent().find(".status");
		jQuery.post('/spring/reg/s', {
			v: v, f: f
		});
		RefreshHandler.updateNodes(statusFields);
	}

	return {
		submit: function () {
			var statusFields = jQuery(".status");
			statusFields.attr("e:next", 1);
			RefreshHandler.requestNodeUpdates(true, "_=/reg/register", statusFields);
		},
		registerRow: function(node) {
			node = jQuery(node);
			var all = jQuery("input", node).add("select", node);
			all.on("blur", fieldChanged);
			all.on("click", fieldChanged);
			all.on("focus", fieldChanged);
		}
	};
})();

var regFunc = {
	updateRow: function (this_obj, fieldName, noError){
		var node = jQuery(this_obj).parent().find("[_name=" + fieldName.replace(/\./g,"\\.") + "]");
		
		if (noError){
			if (node.hasClass("redborder")){
				node.removeClass("redborder");
			}
		} else{
			if (!node.hasClass("redborder")){
				node.addClass("redborder");
			}
		}	
	},
	
	buildAndShowTip3: function(invalidHeaderText, headerText, message, this_obj, field){
		var inputNode = jQuery(this_obj).parent().find('[_name^="'+ field + '"]');
		tip_3(265, 552, 14, 
			'<h3 class="white bold">' + invalidHeaderText + '</h3>', 
			'<h3 class="bold">'  + headerText + '</h3>', 
			message,
			inputNode);
	}
};