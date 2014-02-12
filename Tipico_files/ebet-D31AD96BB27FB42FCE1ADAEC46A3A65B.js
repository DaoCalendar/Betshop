/*
 * workaround for bug #8152: IE8 ajax jquery/sarissa problem
 */
jQuery.ajaxSettings.xhr = function() {
	return window.ActiveXObject ? new ActiveXObject("Microsoft.XMLHTTP") : new XMLHttpRequest();
};
//
jQuery(function() {
	RefreshHandler.initialHash += RefreshHandler.hash(jQuery.cookie("timeZoneId"));
});

/*---------------------------------------------------------------------------------------------------------*/
/* -------------------------- metacap js ----------------------------------------------------------------- */
/* ------------------------------------------------------------------------------------------------------- */

//Logging in Kosole von Firebug/Chrome/IE8 (Developer Tools) 
function log(text) {
	if (window['console'] !== undefined) {
		console.info(text);
	}
}

var eBet = eBet || {};
(function() {
	eBet.util = {

		//management von komponenten state (zB Sonderwettlayer -> auf/zu)
		compStates : [],
		toggleState : function(compName, id, defaultOn) {
			var id = id + '';
			var states = this.getStates(compName),
				index = jQuery.inArray(id, states),
				defaultOn = defaultOn || false,
				on;
			if (index === -1) {
				states.push(id);
				on = !defaultOn;
			} else {
				states.splice(index, 1);
				on = defaultOn;
			}
			this.compStates[compName] = states;
			return on;
		}, 
		
		getStates : function(compName) {
			var states = this.compStates[compName] = this.compStates[compName] || [];
			return states;
		}, 
		
		isStateOn : function(compName, id, defaultOn) {
			var id = id + '';
			var index = jQuery.inArray(id, this.getStates(compName));
			return (index !== -1 && !defaultOn) || (index === -1 && defaultOn);
		},
		
		alternateWorkers : [],
		
		alternate: function(alternationId, parent, dataTypes, breakDataTypes) {
			if (jQuery.inArray(alternationId, this.alternateWorkers) === -1) {
				this.alternateWorkers.push(alternationId);
				var that = this;
				var fn = function alternationWorkerWrapper() {
					var index = jQuery.inArray(alternationId, that.alternateWorkers);
					if (index !== -1) {
						that.alternateWorkers.splice(index,1);
					}
					that.alternateWorker(alternationId, parent, dataTypes, breakDataTypes);
				};
				RefreshHandler.onComplete(fn);
			}
		},
		
		/**
		 * @param workerId - einen namen für diese Funktion (fürs Debugging)
		 * @param parent - HTML-Node der als Anfangspunkt für die jQuery-Abfragen dient
		 * @param dataTypes - CSS Klass-Namen. Elementen, die diese Selektoren besitzen, werden 
		 * mit 'odd' bzw 'even' markiert
		 * @param breakDataTypes - CSS Klass-Namen. Elemente, die diese Selektoren haben, werden
         * setzen die alternierung der nachfolgenden Zeile auf zur�ck auf odd.
		 *  */
		alternateWorker : function(workerId, parent, dataTypes, breakDataTypes) {
			//log('starting worker: ' + workerId);
			var selector = "";
			for (var i = 0; i < dataTypes.length; i++) {
				if (selector) {
					selector += ",";
				}
				selector += '.' + dataTypes[i]; 
			}
			for (var i = 0; i < breakDataTypes.length; i++) {
				if (selector) {
					selector += ",";
				}
				selector += '.' + breakDataTypes[i]; 
			}
			var odd = true;
			var nodes = parent.find(selector);
			//log('nodes found:' + nodes.length);
			nodes.each(function() {
				var jq_el = jQuery(this);
				var reset = false;
				for (var i = 0; i < breakDataTypes.length; i++) {
					//zB Header -> Alternierung neu anfangen
					if (jq_el.hasClass(breakDataTypes[i])) {
						odd = true;
						reset = true;
						break;
					}
				} 
				if (!reset) {
					if (odd) {
						jq_el.removeClass('even').addClass('odd');
						//log('setting odd on ' + jq_el);
					} else {
						jq_el.removeClass('odd').addClass('even');
						//log('setting even on ' + jq_el);
					}
					odd = !odd;
				}
			});
		}
	};
	
	/*Begin: SW-Layer*/
	eBet.sbLayer = {
		getCompName : function(el) {
			return jQuery(el).closest(".jq-event-row-cont").find('.jq-special-bet-layer').attr('e:url');
		},
		
		getLimitPopupName: function(el){
			return  jQuery(el).parent().find('.jq-special-bet-layer-category-popup');
		},
		
		defaultOn : false,

		update : function(el, eventId) {
			if (eBet.util.isStateOn(this.getCompName(el), eventId, this.defaultOn)) {
				jQuery(el).show();
				
				var hover = jQuery(el).parent().find('.limits_hover');
				
				// Mouseover nur hinzuf�gen, wenn die neue Version verwendet wird
				if(hover && !hover.hasClass('init') && this.getCompName(el).indexOf('specialBetLayer3') != -1){
					hover.addClass('init');
					hover.hoverIntent(
							function(){eBet.sbLayer.showLimits(el)}, 
							function(){eBet.sbLayer.hideLimits(el)}
					);
				}
			}
		},
		
		toggle : function(el, eventId, restore) {
			if (!eventId) {
				return;
			}
			var on; 
			if(restore) {
				on = eBet.util.isStateOn(this.getCompName(el), eventId, this.defaultOn);
			} else {
				on =  eBet.util.toggleState(this.getCompName(el), eventId, this.defaultOn);
			}
			var eventRow = jQuery(el).closest(".jq-event-row-cont"),
				sbButton = eventRow.find('.t_more'),
				sbLayer = eventRow.find('.jq-special-bet-layer'),
				sbLayerUrl = sbLayer.attr('e:url');
			if (on) {
				RefreshHandler.loadIfEmpty(sbLayerUrl, false);
				sbButton.addClass('on');
				restore ?  sbLayer.show() : sbLayer.slideDown('fast');
			} else {
				sbButton.removeClass('on');
				restore ?  sbLayer.hide() : sbLayer.slideUp('fast');
			}
		},
		
		updateCategory: function(category, sportId, newStatus){
			var query = "?category="+encodeURIComponent(category);
			query += "&sportId="+encodeURIComponent(sportId);
			query += "&newStatus="+encodeURIComponent(newStatus);
			RefreshHandler.nav("/program/specialbet/updateCategory/"+query);
		},
		
		toggleCategory: function(element, category, sportId){
			// GUI weicht von Auswahl ab, da es default-Werte gibt
			// Daher ist ein serverseitiges Togglen nicht m�glich
			var jqElement = jQuery(element);
			var newStatus = !jqElement.hasClass('on');
			jqElement.toggleClass('on');
			if(!jqElement.hasClass('limit'))
				jqElement.next('div').slideToggle();
			eBet.sbLayer.updateCategory(category, sportId, newStatus);
		},
		
		showLimits: function(element){
			element = eBet.sbLayer.getLimitPopupName(element);
			var sbLayer = element.parent().parent().find(".jq-special-bet-layer");
			if(!sbLayer.is(":visible"))
				return;
			
			// Attribute vom SW-Layer �bernehmen (gleiches Modell)
			var url = sbLayer.attr('e:url');
			url = url.replace('@@sbLayer', '@@specialBetLayerCategoryPopup');
			url = url.replace('specialBetLayer3', 'specialBetLayerCategoryPopup');
	
			RefreshHandler.nav(url);
		},
		
		hideLimits: function(element){
			element = eBet.sbLayer.getLimitPopupName(element);
			element.hide();
		},
	};
	
	eBet.resultSetRow = {
		update : function(el, eventId) {
			markLiveResults(el);
		}
	};

	eBet.eventRow = {
		update : function(el, eventId, showImmediately) {
			markLiveResults(el);
			eBet.sbLayer.toggle(el, eventId, !showImmediately);
		}
	};
	/*End: SW-Layer*/

})();


function addBet(){
	}

var Zeit1 = new Date();
var AbsolutJetzt = Zeit1.getTime();

var ua = window.navigator.userAgent;
var msie = ua.indexOf("MSIE ");
var chrome = ua.indexOf("Chrome");
var safari = ua.indexOf("Safari");
var fireFox = ua.indexOf("Firefox");
var IE_vers = parseInt(ua.substring (msie+5, ua.indexOf (".", msie)));

function popup(type, template, dx, dy, style){
	fenster = window.open("popup_"+type+".php?main="+template+"&style="+style,"popup","toolbar=no,scrollbars=yes,menubar=no,resizable=yes,status=no,width="+dx+",height="+dy+",dependent=yes");
	fenster.focus();
	}

function game(dx, dy){
	fenster = window.open("game.php","game","toolbar=no,scrollbars=no,menubar=no,resizable=yes,status=no,width="+dx+",height="+dy+",dependent=yes");
	fenster.focus();
	}

function open_page(page, close){
	document.location.href = 'index.php?main='+page;
	if(close)
		fenster.close();
	}

function layer_offset(this_obj){
	var offset = jQuery(window).scrollTop() + 165;
	jQuery(this_obj).css({'top' : offset + 'px'});
	}

jQuery.noConflict();
jQuery(document).ready(function() {
	
	 jQuery(window).scroll(function() {layer_offset(jQuery('.main_layer:hidden'))});
	/* ### Head-Drop-Menue: Mein [Branding], Kasse ### */
	
	jQuery("#mytipico_foot, #mytipico_title, #head_mytipico .head_shadow").click(function() {
		jQuery("#mytipico").slideToggle("fast");
		jQuery("#mytipico_foot div").toggle();
		});
			
	jQuery("#headdrop2_foot, #headdrop2_title, #head_headdrop2 .head_shadow").click(function() {
		jQuery("#headdrop2").slideToggle("fast");
		jQuery("#headdrop2_foot div").toggle();
		});
			
	/* ### Highlights ### */
	
	jQuery("#hilights_1 .hilights_head_2").click(function () {
		jQuery("#hilights_1").hide();
		jQuery("#hilights_2").show();
		jQuery(this).blur();
		});
	
	jQuery("#hilights_2 .hilights_head_2").click(function () {
		jQuery("#hilights_2").hide();
		jQuery("#hilights_1").show();
		jQuery(this).blur();
		});
	
	/* ### Tooltip ### */

	jQuery(".nav_main_1 .left, .nav_main_2 .left, .check_l, .arrow_l").hover(function () {
		tip(jQuery(this).text(), 20);
		document.onmousemove = update_tip;
    	}, function () {
		untip();
		});
	
	//globaler Diashow Start (kann auch in onLoad gepackt werden)
	jQuery(function() {
		jQuery("#main_teaser_rot.img2, #main_teaser_rot.img3, #main_teaser_rot.img4").show();
		slide_start();
	});
	
	jQuery(".check_r").hover(function () {
		tip(jQuery(this).parent().find(".left").text(), 20);
		document.onmousemove = update_tip;
    	}, function () {
    		untip();
    });
	
	jQuery(document).click(function(e) {
		var tar =  jQuery(e.target);
		jQuery('.drop_panel').hide();
    	if(e.target.className == 'drop_but')
    		tar.parent().find('.drop_panel').toggle();
    	else if(tar.parent().prop('class') == 'drop_but')
    		tar.parent().parent().find('.drop_panel').toggle();
    	else if (jQuery('#help_resultType_layer_inner').is(":visible") && !tar.hasClass('jq-helpbutton') && tar.attr("id") != 'help_resultType_layer_inner' && tar.parents('#help_resultType_layer_inner').length == 0){
    		closeHelp();
    	}
	});
	
	/* ### Placeholder-Fix fuer Internet Explorer ### */

	jQuery('[placeholder]').focus(function() {
		var input = jQuery(this);
		if (input.val() == input.attr('placeholder')) {
			if (msie == -1) {
				if (this.originalType) {
					this.type = this.originalType;
					delete this.originalType;
				}
			}
			input.val('');
			input.removeClass('placeholder');
		}
	}).blur(function() {
		var input = jQuery(this);
		if (input.val() == '') {
			if (msie == -1) {
				if (this.type == 'password') {
					this.originalType = this.type;
					this.type = 'text';
				}
			}
			input.addClass('placeholder');
			input.val(input.attr('placeholder'));
		}
	}).blur().parents('form').submit(function() {
		jQuery(this).find('[placeholder]').each(function() {
			var input = jQuery(this);
			if (input.val() == input.attr('placeholder')) {
				input.val('');
			}
		})
	});
	
	/* ### Registrierung: Validierung der Felder 'login' und 'email' und Verstecken des Bot-Feldes ### */
	
	jQuery('#registration').ready(function() {
		jQuery(document.getElementById('registerForm:login')).ready(function() {
			jQuery(document.getElementById('registerForm:email')).ready(function() {
				var login = document.getElementById('registerForm:login');
				var email = document.getElementById('registerForm:email');
				if(login != null && login.value != '' && email != null && email.value != '') {
					validateLoginAndEMailOnPageLoad();
				} else if(login != null && login.value != '') {
					validateLoginOnPageLoad();
				} else if(email != null && email.value != '') {
					validateEMailOnPageLoad();
				}
			});
		});
		jQuery('#registerForm').mousemove(function(e) {
			jQuery('#emailConfirmLeft').hide();
			jQuery('#emailConfirmRight').hide();
		});
	});
	
	jQuery.urlParam = function(name){
	    var results = new RegExp('[\\?&]' + name + '=([^&#]*)').exec(window.location.href);
	    if(results == null)
	    	return "";
	    return results[1] || 0;
	}
	
	jQuery('#searchField').ready(function(){
		var text = decodeURIComponent(jQuery.urlParam('s'));
		if(text){
			var decodedText = text.replace(/\+/g," ");
			jQuery('#searchField').val(decodedText);
		}
	});	
	
	// CI/CD Check
	check_ci();
	window.onresize = check_ci;
	});

function check_ci(go){
	if(jQuery(window).width() > 1250){
		jQuery('#ci_l, #ci_r, #ci_b').show();
		// damit rechtes Dreieck nicht unter dem Footer herausrutscht
		jQuery(window).height() > jQuery('body').height() ? jQuery('#ci_r').hide() : jQuery('#ci_r').show();	
		}
	else
		jQuery('#ci_l, #ci_r, #ci_b').hide();
	}


function drop_click(this_obj){
	var all = this_obj.parent().parent().parent();
	all.find('a').removeClass('on');
	this_obj.addClass('on');
	all.find('.drop_but span').html(this_obj.text());
	};
	
function drop_setValue(this_obj){
		var all = this_obj.parent().parent().parent();
		all.find('a').removeClass('on');
		this_obj.addClass('on');
		all.find('input[type^=hidden]').val(this_obj.text());
	};
/***********************
 * Navigation Anfang
 ***********************/	

function bindNavigationBehavior(){
	// Verhalten f�r die Gruppenlinks in der Navigation
	jQuery("a[id^='navEntry']").click(function(e) {
		var rootGroup = jQuery(this).hasClass('nav_1');
		var selfSelected = jQuery(this).hasClass('on');
		
		// keine Rootgruppe ist selektiert == Startseite
		var rootGroupSelected = jQuery('a.nav_1.on').length > 0;
		
		var selectedChildGroup = false;
		if(rootGroup && selfSelected)
			selectedChildGroup = jQuery(jQuery(this)).parent().find('a.nav_3.on').length > 0;

		
		// Regeln:
		// Startseite: 1. Aufruf einer Rootgruppe: Link aufrufen
		// Auswahlseite: Rootgruppe und mind. eine Untergruppe markiert: Link aufrufen
		if(!rootGroupSelected || rootGroup && selectedChildGroup || jQuery(e.target).is('img'))
			return;
		
		// Linkausf�hrung verhindern
		e.preventDefault();
		
		// Gruppen 1. und 2. Ebene nicht togglen, 
		// falls wirklich keine Untergruppen vorhanden sind.
		var noChildren = !jQuery(this).hasClass('nav_3') 
			&& jQuery(this).find('span.num_r:first').text() == 0 
			&& jQuery(this).parent().find('ul:first').children().length == 0;
		if(noChildren)
			return;
		
		// Link
		if(jQuery(this).hasClass('nav_3')){	
			toggleGroup(getGroupId(jQuery(this)));
		// Navigation
		}else{
			jQuery(this).toggleClass(getNumClass(this));
			jQuery(this).find('span.num_r').toggle();
		
			if(jQuery(this).hasClass('nav_2'))
				jQuery(this).parent().find('ul:first').toggle();
			else{
				var groupLists = jQuery(this).parent().find('ul.level_2');
				// Hier kein toggle, da die "more groups"-liste einen anderen Status haben kann
				var hide = jQuery(this).hasClass('on');
				var moreGroups = groupLists.length>1;
				if(hide){
					jQuery(groupLists[0]).hide();
					jQuery(groupLists[1]).hide();
					jQuery(groupLists[2]).hide();
				}else{
					jQuery(groupLists[0]).show();
					jQuery(groupLists[2]).show();
					var link = jQuery(groupLists[2]).find('span:first');
					if(jQuery(link).is(':visible'))
						jQuery(groupLists[1]).hide();
					else
						jQuery(groupLists[1]).show();
				}
	
			}
		}
		jQuery(this).toggleClass('on');
	});
	
	// "Alle hinzuf�gen/entfernen" anzeigen
	jQuery("a[id^='navEntry']").mouseover(function(){
		if(jQuery(this).hasClass('nav_3'))
			tip(jQuery(this).find('span:first').text(),18);
		else if(jQuery(this).hasClass('on'))
			jQuery(this).children(':last-child').show();
	});
	
	// "Alle hinzuf�gen/entfernen" ausblenden
	jQuery("a[id^='navEntry']").mouseout(function(){
		untip();
		if(!jQuery(this).hasClass('nav_3') && jQuery(this).hasClass('on'))
			jQuery(this).children(':last-child').hide();
	});
	
	// Verhalten f�r die Bilder "Alle hinzuf�gen/entfernen"
	jQuery("img[id^='navEntry']").click(function(e){
		e.preventDefault();
		untip();
		var id = jQuery(this).prop('id');
		var linkElement = jQuery(this).parent().parent();
		var groupId = getGroupId(linkElement);
		var add = id.indexOf("all") != -1;
		toggleAll(groupId,add?'add':'remove');
	});
	
	// Rotes Bild anzeigen
	jQuery("img[id^='navEntry']").mouseover(function(){
		tip(jQuery(this).prop('alt'));
		var src = jQuery(this).prop('src');
		if(src.indexOf("_on-") == -1){
			src = src.replace('-','_on-');
			jQuery(this).prop('src',src);
		}
	});
	
	// Rotes Bild ausblenden
	jQuery("img[id^='navEntry']").mouseout(function(){
		untip();
		var src = jQuery(this).prop('src');
		if(src.indexOf("_on-") != -1){
			src = src.replace('_on-','-');
			jQuery(this).prop('src',src);
		}
	});
	
	// 1. Ebene "weitere Gruppen"
	jQuery('#nav_more_groups').click(function(e){
		e.preventDefault();
		jQuery(this).parent().find('ul.level_1:last').toggle();
		jQuery(this).find('span').toggle();
		toggleNavMoreNew();
	});
	
	// 2. Ebene "weitere Gruppen"
	jQuery("a[id^='nav_more_subGroups']").click(function(e){
		var toggleId = jQuery(this).prop('id').substr("nav_more_subGroups_".length);
		var toggleList = jQuery(this).parent().parent().parent().find('ul.level_2')[1];
		jQuery(toggleList).toggle();
		jQuery(this).find('span').toggle();
		toggleNavMoreNew(toggleId);
	});
}

// Parsed die Gruppen-Id aus der id des Links
function getGroupId(element){
	var id = jQuery(element).prop('id');
	return id.toString().substr("navEntry".length);
}


var markedNavGroups = new Array();

// Markiert eine Gruppe der Ebene 3 (aufgerufen in eventList.xhtml pro Block)
function remarkGroup(id){
	var element = jQuery('#navEntry'+id);
	if(jQuery(element).hasClass('nav_3'))
		markedNavGroups.push(id);
}

// Baut die Navigation neu auf:
// - Markiert alle selektieren Gruppen
// - Klappt alle Ebenen auf, die selektierte Gruppen enthalten
// reset: Setzt alle Markierungen zur�ck (vor dem Neuaufbau)
// toggleAll: Gibt an, ob Bereiche zugeklappt werden sollen, 
// wenn keine markierten Untergruppen mehr vorhanden sind
// toggleGroupId: Die Id der Gruppe, mit der getoggled wurde 
function rebuildNavigation(toggleAll,toggleGroupId){
	var links = jQuery("a[id^='navEntry']");
	var toggleElement = jQuery('#navEntry'+toggleGroupId)[0];
	for(i=0;i<links.length;i++){
		if(jQuery(links[i]).hasClass('nav_3'))
			continue;
		var selected = false;
		var childGroups = jQuery(links[i]).parent().find('a.nav_3');
		for(j=0;j<childGroups.length;j++){
			var groupSelected = jQuery.inArray(getGroupId(childGroups[j]),markedNavGroups);
			if(groupSelected > -1){
				jQuery(childGroups[j]).addClass('on');
				selected = true;
			}else{
				jQuery(childGroups[j]).removeClass('on');		
			}
		}
		
		var childNodeList = jQuery(links[i]).parent().find('ul:first');
		var numClass = getNumClass(links[i]);
		if(selected){
			jQuery(links[i]).removeClass(numClass);
			jQuery(links[i]).addClass('on');
			jQuery(links[i]).find('span.num_r').first().hide();
			jQuery(childNodeList).show();
		}else if(toggleAll && (links[i] == toggleElement || jQuery(toggleElement).parent().find(links[i]).length > 0)){
			jQuery(links[i]).addClass(numClass);
			jQuery(links[i]).removeClass('on');
			jQuery(links[i]).find('span.num_r').first().show();
			jQuery(links[i]).find('span.num_r').last().hide();
			jQuery(childNodeList).hide();
		}
	}
	
	markedNavGroups = new Array();
}

function getNumClass(element){
	var value = jQuery(element).find('span.num_r').first().text();
	return 'num_' + value.length;
}

/***********************
 * Navigation Ende
 ***********************/

/************************************
 * Navigation Ergebnisarchiv Anfang *
 ************************************/
function bindArchiveNavigationBehavior(){
	jQuery("a.nav_2").click(function(e){
		if(jQuery(this).prop('id') == 'nav_more_groups_sub')
			return;
		e.preventDefault();
		var groupId = getArchiveGroupId(jQuery(this));
		jQuery(this).toggleClass('on');
		jQuery(this).parent().find('ul.level_3:first').toggle();
	});
	
	jQuery('#nav_more_groups').click(function(e){
		e.preventDefault();
		jQuery(this).parent().find('ul.level_1:last').toggle();
		jQuery(this).find('span').toggle();
	});
	
	jQuery('#nav_more_groups_sub').click(function(e){
		e.preventDefault();
		jQuery('#furtherSubGroups').toggle();
		jQuery(this).find('span').toggle();
	});
}

function markArchiveNavTreeNode(parentGroupId,groupId) {
	jQuery('#archiveEntry'+groupId).addClass('on');
	jQuery('#archiveEntry'+parentGroupId).addClass('on');
	jQuery('#archiveEntry'+parentGroupId).parent().find('ul.level_3:first').show();
	var hiddenList = jQuery('#archiveEntry'+groupId).closest('#furtherSubGroups');
	if(hiddenList.length > 0){
		jQuery(hiddenList[0]).show();
		jQuery('#nav_more_groups_sub').find('span').toggle();
	}
}

function getArchiveGroupId(element){
	var id = jQuery(element).prop('id');
	return id.toString().substr("archiveEntry".length);
}

/************************************
 * Navigation Ergebnisarchiv Ende   *
 ************************************/

/************************************************
 * Navigation Ergebnis & Livescore Popup Anfang
 ************************************************/

function rebuildScorePopupNavigation(id){
	if(id == undefined)
		id = "AllLive";
	var elements = jQuery("a[id^='navEntry']");
	for(i=0;i<elements.length;i++){
		jQuery(elements[i]).addClass(getNumClass(elements[i]));
		if(id != "Search" && jQuery(elements[i]).prop('id') == "navEntry"+id)
			jQuery(elements[i]).addClass('on');
		else
			jQuery(elements[i]).removeClass('on');
	}
}

/************************************************
 * Navigation Ergebnis & Livescore Popup Ende
 ************************************************/


function addLinkParameters(linkId) {
	var link = jQuery('#rfSubmit');
	var href = link.prop('href');
	var login = jQuery('#rfLogin');
	var email = jQuery('#rfEmail');
	var hrefLogin = login.val();
	var hrefEmail = email.val();
	if(login.attr('placeholder') == hrefLogin) {
		hrefLogin = "";
	}
	if(email.attr('placeholder') == hrefEmail) {
		hrefEmail = "";
	}
	href = href + '?rflogin=' + encodeURIComponent(hrefLogin) + '&rfemail=' + encodeURIComponent(hrefEmail);
	link.prop('href', href);
}

//Diashow Start 
var timer = null;

function slide_start(){
	timer = setInterval("slide_switch()", 7000);
	}

// Diashow Stop 
function slide_stop(){
	clearTimeout(timer);
	}

// Diashow
function slide_switch(){
	var active = jQuery('#slideshow .main_teaser_rot.active');

	if(active.length == 0)
		active = jQuery('#slideshow .main_teaser_rot:last');
	
	var next = active.next().length ? active.next() : jQuery('#slideshow .main_teaser_rot:first');

	active.addClass('last-active');
	
	active.animate({opacity: 0.0}, 1500);
	
	next.css({opacity: 0.0})
		.addClass('active')
		.animate({opacity: 1.0}, 1500, function() {
			active.removeClass('active last-active');
		});
	}

// Diashow Jump
function slide_jump(id){
	//dia_stop = id;
	jQuery('#slideshow .main_teaser_rot.active').stop(true, true);
	jQuery('#slideshow .main_teaser_rot').removeClass('active');
	jQuery('#slideshow .main_teaser_rot').removeClass('last-active');
	jQuery('#slideshow .main_teaser_rot.img'+id).addClass('active');
	jQuery('#slideshow .main_teaser_rot.img'+id).css({opacity: 1.0});
	jQuery('.main_teaser_but_row a').blur();
	}

//Diashow RollOver
function slide_rollover(id){
	if(id == 1)
		slide_stop();
	else
		slide_start();
	}

//Textroller
var timerID = null;
function textrollInit(){
	clearTimeout(timerID);
	speed = 1;
	mov = document.getElementById('box_roll');
	if (mov != undefined){
		movHeight = mov.offsetHeight;
		textroll();
	}
}

function textroll(){
	var top = mov.offsetTop;
	mov.style.top = (top - speed) + 'px';
	
	if(top < -41){
		jQuery('#box_roll li:first').clone().appendTo('#box_roll');
		jQuery('#box_roll li:first').detach();
		mov.style.top = (top + 41) + 'px';
		timerID = setTimeout('textroll()', 2000);
		}
	else
		timerID = setTimeout('textroll()', 60);
	}

//Jackpotroller
var jpTimerID = null;
function jackpotInit(){
	// Animation nicht doppelt qstarten
	if(jpTimerID != null)
		return;
	clearTimeout(jpTimerID);
	jpSpeed = 8;
	jpMov = document.getElementById('jackpot_roll');

	// Counter initialisieren
	loadCounter();

	// nur der erste Platzhalter ist anfangs sichtbar
	jQuery('#jpCounter_temp_2').remove();
	jQuery('#jpCounter_temp_3').remove();
	// dafür aber der erste richtige Z�hler anfangs nicht
	jQuery('#jpCounter_1').hide();

	// Animation zeitverzögert starten
	jpTimerID = setTimeout('jackpot()', 1000);
}

function jackpot(){
	var left = jpMov.offsetLeft;
	jpMov.style.left = (left - jpSpeed) + 'px';
	
	// Animation am Ende angekommen?
	if(left < -272){
		// der erste Platzhalter ist nun nicht mehr sichtbar
		jQuery('#jpCounter_temp_1').remove();
		// dafür wird ab nun der richtige Zahler verwendet
		jQuery('#jpCounter_1').show();
		// Zahlerwert wieder auf 0 setzen, damit die Animation neu starten kann
		loadCounter(1);
		// nach Links raus geschobenes Element ganz Rechts erneut einfügen
		jQuery('#jackpot_roll li:first').appendTo('#jackpot_roll');
		jpMov.style.left = (left + 280) + 'px';
		// Animation nach einem Moment neu starten
		jpTimerID = setTimeout('jackpot()', 5000);
	} else {
		// beim IE8 sind die "left"-Werte um 2px verschoben
		if ((left - (left % 8)) == -96) {
			// Animation starten
			loadCounter(2);
		}
		// Animation fortsetzen
		jpTimerID = setTimeout('jackpot()', 60);
	}
}

jpCounterVal = new Array();
counterImgPaths = new Array();
function addCounterVals(counterVals, numeric, marker, point){
	jpCounterVal = counterVals;
	counterImgPaths[0] = numeric;
	counterImgPaths[1] = marker;
	counterImgPaths[2] = point;
}

var jpCounter_1, jpCounter_2, jpCounter_3;
function loadCounter(fire) {
	var idString = jQuery('#jackpot_roll li:first').find('.txt_2').prop("id");
	var idVal = idString.substr(10);
	if(parseInt(idVal) > jpCounterVal.length-1)
		idVal = 0;

	if(fire == 1){
		/*
		var addVal = '1111111111111';
		addVal.substr(0, jpCounterVal[idVal].length - 1);
		
		//valReverse = jpCounterVal[idVal].split('').reverse().join('');
		jpCounterVal[idVal] += parseInt(addVal);
		eval(idString).setValue(jpCounterVal[idVal], 1500);
		*/
		eval(idString).setValue(0, 1500);
		}
	else if(fire == 2){
		eval('jpCounter_'+(parseInt(idVal)+1)).setValue(jpCounterVal[idVal], 1500);
		}
	else{
		jpCounter_1 = new Counter("jpCounter_1",{
		digitsNumber: jpCounterVal[0].length,
		direction: Counter.ScrollDirection.Upwards,
		characterSet: Counter.DefaultCharacterSets.numericDown,
		charsImageUrl: counterImgPaths[0],
		markerImageUrl: counterImgPaths[1],
		point: counterImgPaths[2]});
		
		jpCounter_2 = new Counter("jpCounter_2",{
		digitsNumber: jpCounterVal[1].length,
		direction: Counter.ScrollDirection.Upwards,
		characterSet: Counter.DefaultCharacterSets.numericDown,
		charsImageUrl:counterImgPaths[0],
		markerImageUrl: counterImgPaths[1],
		point: counterImgPaths[2]});
		
		jpCounter_3 = new Counter("jpCounter_3",{
		digitsNumber: jpCounterVal[2].length,
		direction: Counter.ScrollDirection.Upwards,
		characterSet: Counter.DefaultCharacterSets.numericDown,
		charsImageUrl: counterImgPaths[0],
		markerImageUrl: counterImgPaths[1],
		point: counterImgPaths[2]});
		}
	}

// Generelles RollOver Ein/Aus
// @Entwickler: könnte u.U. popMobileOffering() weiter unten ersetzen...
function rollOverShow(what){
	var this_obj = jQuery(what);
	if(this_obj.is(':hidden'))
		jQuery(this_obj).show();
	else
		jQuery(this_obj).hide();
	}


//Tab Navigation
function tab_menue(this_obj, what, action, hide, tab, bgImg){
	var tabs = this_obj.parent().parent().find("." + tab);
	tabs.removeClass('on');
	tabs.children(':first-child').css('color', '#999');
	this_obj.parent().addClass('on');
	this_obj.css('color', '#666').blur();
	if(tab == 'tab_select'){
		action ? jQuery("#" + what).show() : jQuery("#" + what).hide();
		if(hide)
			jQuery("#" + hide).hide();
		}
	else if(tab == 'tab_3_select'){
		tabs.children(':first-child').css('color', '#666');
		this_obj.css('color', '#dedede').blur();
		}
	else{
		jQuery('body').find(".tab_cont").hide();
		jQuery("#tab_cont_" + action).show();
		if(tab == 'tab_2_select'){
			var tabs_cont = this_obj.parent().parent().parent().find(".tab_2_cont");
			tabs_cont.addClass('hide');
			jQuery(".tab_2_cont.num_" + action).removeClass('hide');
			}
		}
	
	if(bgImg == 'nemid'){
		if(action == 2){
			tabs.children(':first-child').css('backgroundImage', 'url(img/nem_id.gif)');
			this_obj.css('backgroundImage', 'url(img/digital_signatur_on.gif)');
			}
		else{
			tabs.children(':first-child').css('backgroundImage', 'url(img/digital_signatur.gif)');
			this_obj.css('backgroundImage', 'url(img/nem_id_on.gif)');
			}
		}
	}

//Toggle DropDown Navigation
function drop_menue(id){
	jQuery("#menue_" + id).slideToggle("fast");
	}

//Close DropDown Navigation
function close_menue(id){
jQuery("#" + id).mouseleave(function() {
	jQuery("#menue_" + id).hide();
	});
}

//Select  DropDown Navigation
function select_menue(this_obj){
	var idString = this_obj.parent().parent().prop("id");
	idString = idString.substr(6);
	jQuery("#"+idString).find("div:first").text(this_obj.text());
	if(idString == 'drop_1' && this_obj.prop("id") != 'period')
		jQuery("#datepicker").hide();
	else if(this_obj.prop("id") == 'period')
		jQuery("#datepicker").show();
   	}

//Ticket + Open bets
function tog_slider(what){
	jQuery("#"+what+"_body").slideToggle("slow");
	jQuery("#"+what+"_foot div").toggle();
}

function nav_more() {
	jQuery("#nav_more").hide();
	jQuery("#nav_corner_off").show();
	jQuery("#root_box").slideDown("fast");
}

function nav_less() {
	jQuery("#nav_corner_off").hide();
	jQuery("#root_box").slideUp("fast");
	jQuery("#nav_more").show();
}

//Slider (Laschen) öffnen
function show_slider(this_obj, type) {
	var this_obj_2 = this_obj.parent().parent();
	if(this_obj_2.find(".hide").is(":hidden")){
		this_obj.addClass(type + "_on");
		this_obj_2.find(".hide").show();
		this_obj_2.find(".sheet_slider_body_off").addClass("sheet_slider_body").removeClass("sheet_slider_body_off");
	}
	else{
		this_obj.removeClass(type + "_on");
		this_obj_2.find(".hide").hide();
		this_obj_2.find(".sheet_slider_body").addClass("sheet_slider_body_off").removeClass("sheet_slider_body");
	}
}

//Slider (Laschen) schliessen
function hide_slider(this_obj, type) {
	type == 'm' ? this_obj = this_obj.parent().parent() : this_obj = this_obj.parent();
	this_obj.find(".hide").hide();
	this_obj.find(".sheet_slider_" + type).removeClass(type + "_on");
	this_obj.find(".sheet_slider_body").addClass("sheet_slider_body_off").removeClass("sheet_slider_body");
}

//Upcoming Live Wetten
function tog_upcoming(this_obj){
	var parent_table = this_obj.parents("table:first");
	var slider = parent_table.parent().find(".sheet_body_live");
	if(slider.is(':hidden')){
		slider.slideDown();
		parent_table.find(".tog").addClass("open");
	}
	else {
		slider.slideUp();
		parent_table.find(".tog").removeClass("open");
	}
	parent_table.find(".tog div").toggle();
}

function stopEverything() {
	showDelayLayer();
	RefreshHandler.run=function() {};
	lock = true;
}

//Layer �ffnen
function show_layer(id, top, left, width, mtop, mleft, dynTop) {
	hideCasinoGame(function() {
		// for our lovely IE
		top ? top + 'px' : '';
		left ? left + 'px' : '';
		width ? width + 'px' : '';
		mtop ? mtop + 'px' : '';
		mleft ? mleft + 'px' : '';
		
		jQuery('.condition').hide();
		jQuery(".fly_message_layer").hide();
		if ((typeof top != 'undefined') && (typeof left != 'undefined') && 
			(typeof mtop != 'undefined') && (typeof mleft != 'undefined')) {
			jQuery('#' + id + '_layer').css({'top' : top, 'left' : left, 'width' : width, 'margin-top' : mtop, 'margin-left' : mleft}).show();
		}
		else if ((typeof top != 'undefined') && (typeof left != 'undefined')) {
			jQuery('#' + id + '_layer').css({'top' : top+'px', 'left' : left+ 'px'}).show();
		}
		jQuery('#' + id + '_layer').show();

		if(dynTop)
			jQuery('#' + id + '_layer').css('top', (dynTop + jQuery(window).scrollTop()) + 'px');
		layer_offset(jQuery('.main_layer'));
	});
}

// Layer schliessen
function hide_layer(id) {
	jQuery('#' + id + '_layer').hide();
	if('dim' == id)
		showCasinoGame();
}

function toggleSpecialBetPanel(divId, eventId) {
	return _toggleSpecialBetPanel(divId, eventId, false, '');	
}

function toggleSpecialBetPreviewPanel(divId, eventId) {
	return _toggleSpecialBetPanel(divId, eventId, true, '');	
}

function toggleSpecialBetPanelUpcoming(divId, eventId, excludedResultSets) {
	return _toggleSpecialBetPanel(divId, eventId, false, excludedResultSets);	
}

function _toggleSpecialBetPanel(divId, eventId, preview, excludedResultSets) {
	var div = jQuery("#"+divId);
	var link = jQuery("#"+divId+"_link");
	/*sw-layer in lwk ist in einer tabelle platziert, dafuer braucht man die unterscheidung*/
	var upcomingEvents = /^upcoming_/.test(divId);
	function toggle() {
		if (upcomingEvents)
			div = div.parents('tr:first');
		if (link.toggleClass("m_on").hasClass("m_on")) {
			div.show();
		} else {
			div.hide();
		}
	}

	if (div.is(":empty")) {
		div.load(
			'/program/specialBetPanel.faces', {
				eventId: eventId,
				preview: preview,
				excludedResultSets: excludedResultSets
			}, function() {
				markResults();
				toggle();
			}
		);
	} else {
		toggle();
	}
	
	return false;
}

var tipCancelled;
function loadEventStakeRatio(eventId, imgNode) {
	tipCancelled = false;
	var url = RefreshHandler.urlUpdate+"?"+RefreshHandler.encodeUrl("_", "/program/eventstakeratio?eventId="+eventId);
	var n = jQuery('#ratioHolder' + eventId);
	n.load(url,{
	}, function() {
		if (!tipCancelled) 
			tip_2(n.html(), jQuery(imgNode));
	}
	);
}

//anzeige von statistics-layer bei icons. statistics-layer als rollover auf event-zeilen in der 
//lwk wird in eBet.eventRow gemanaged
function loadEventStatistics2(element, eventId, sportId, eventType) {
	var url = "@betradar_tip/program/eventstatslayer?eventId="+eventId + "&sportId=" + sportId;
	if (eventType) {
		url += "&eventType=" + eventType;
	}
	var jq_el = jQuery(element);
	loadTooltipDelayed(jq_el, url, false);
}

var eventTVTimer = null;
/**deprecated, bitte loadEventTVInfos2 verwenden*/
function loadEventTVInfos(eventId, show, flag) {
	clearTimeout(eventTVTimer);
	if(!flag){
		if(show)
			eventTVTimer = setTimeout(function() {
				loadEventTVInfos(eventId, 1, 1);
			},300);
		else
			untip();
	}else
		RefreshHandler.nav('@eventTV'+eventId+'/program/eventTV?eventId='+eventId);
}

function loadEventTVInfos2(element) {
	var jq_el = jQuery(element);
	//jq-tv-button-content ist nur bei comp-ref (!comp) gesetzt
	var url = jq_el.find('.jq-tv-button-content').attr('e:url');
	if (url) {
		loadTooltipDelayed(jq_el, url);
	} else {
		var node = jq_el.find('.e_active');
		//in der onupdate von eventTV.jsp ist die Tooltip-Funktion drin
		var onupdate = node.attr('e:onupdate');
		if (onupdate) {
			(function() {
				eval(onupdate);
			}).apply(node, []);
		}
	}
}

/**Allgemeine Funktion, die von anderen Funktionen verwendet werden sollen, die ein 
 * verz�gertes Laden eines Refresh-Framework-Komponenten ben�tigen*/
var tooltipTimerKey = 'tooltipTimer';
function loadTooltipDelayed(jq_el, url) {
	if (!jq_el || !url) {
		return;
	}
	if (jq_el.data(tooltipTimerKey)) {
		RefreshHandler.nav(url);
		removeDataAndTimer(jq_el);
	} else {
		var tooltipTimer = setTimeout(function() {
			loadTooltipDelayed(jq_el, url);
		}, 300);
		jq_el.data(tooltipTimerKey, tooltipTimer);
	}
}

function untooltipDelayed(element) {
	removeDataAndTimer(element);
	untip();
}

function removeDataAndTimer(element) {
	var jq_el = jQuery(element);
	clearTimeout(jq_el.data(tooltipTimerKey));
	jq_el.removeData(tooltipTimerKey);
}

function sound_play(name) {
	if (typeof window['swfobject'] != 'undefined') {
		var obj = swfobject.getObjectById('sound');
		if(obj && typeof obj.setVar != 'undefined'){
			try {
		 		obj.setVar({sound_name: name});
		 		obj.Play();
			} catch(err) {
				log(err);
			}
		}
	}
}

function hideSpecialBetPanel(where) {
	var cur = jQuery(where);
	
	// lookup our "ajax parent"
	for(; cur.length && !cur.hasClass("ajax_target"); cur = cur.parent());

	// use this node's id
	toggleSpecialBetPanel(cur.prop("id"));
}

//Zeitzonen Layer
function mini_teaser(e, what){
	if(e.preventDefault) 
		e.preventDefault();
	else
		e.returnValue = false;
	var this_obj = jQuery("#"+what+"_mini_layer");
	if(this_obj.is(':hidden')){
		jQuery("#zone_mini_layer").hide();
		jQuery("#lang_mini_layer").hide();
		jQuery(this_obj).show();
	}
	else
		jQuery(this_obj).hide();
}

//Rollover Flaggen
function change_flag(this_obj, flag, casino){
	var sub_path = '';
	var path_split = this_obj.prop('src').split('/');
	if(casino)
		var sub_path = 'casino/';
		
	if(flag == 1)
		this_obj.prop('src', '/img/' + sub_path + 'flags/' + path_split[path_split.length-1]);
	else
		this_obj.prop('src', '/img/' + sub_path + 'flags_grey/' + path_split[path_split.length-1]);
}

//Rollover Payment
function change_img(this_obj, flag, casino){
	var sub_path = '';
	var path_split = this_obj.prop('src').split('/');
	var file = path_split[path_split.length-1];
	var path = path_split[path_split.length-2];
	if(casino)
		var sub_path = 'casino/';
	
	if(flag == 1)
		this_obj.prop('src', '/img/' + sub_path + path + '_on/' + file);
	else
		this_obj.prop('src', '/img/' + sub_path + path.substr(0, path.length - 3) + '/' + file);
}

//Zeitzonen Layer
function change_zone(){
	var this_obj = jQuery("#zone_mini_layer");
	if(this_obj.is(':hidden'))
		jQuery(this_obj).show();
	else
		jQuery(this_obj).hide();
}

function popMobileOffering(){
	var this_obj = jQuery("#mobile_offering");
	if(this_obj.is(':hidden'))
		jQuery(this_obj).show();
	else
		jQuery(this_obj).hide();
}

//Online-Casino Active Game
function active_game(this_obj){
	var offset = this_obj.find("img").offset();
	var active_game = jQuery('#casino_active_game');
	active_game.css("left", offset.left).css("top", offset.top).css("cursor","pointer");
	active_game.unbind('click').click(open_game_handler);
}

function open_game_handler() {
	var gameId = jQuery(this).attr('gameid');
	browseGame(gameId, true);
	return false;
}

function slide_game_handler() {
	var gameId = jQuery(this).attr('gameid');
	select_game(gameId);
	return false;
}

function select_game(gameId) {
	var span = jQuery('#small_picts .small_pict_' + gameId);
	var img = span.find('img');
	var gameName = img.prop('alt');
	var imageId = img.attr('imageSrcBig');
	jQuery('#explore_pict').prop('src', imageId);
	var link = span.find('a');
	var exploreLink = jQuery('#explore_pict_link');
	var exploreLinkSmall = jQuery('#explore_pict_link2');
	exploreLink.prop('href', link.prop('href'));
	exploreLink.attr('gameid', gameId);
	exploreLink.unbind('click', open_game_handler);
	exploreLink.bind('click', open_game_handler);
	exploreLinkSmall.html(gameName);
	exploreLinkSmall.prop('href', link.prop('href'));
	exploreLinkSmall.attr('gameid', gameId);
	exploreLinkSmall.unbind('click', open_game_handler);
	exploreLinkSmall.bind('click', open_game_handler);
	jQuery('#casino_active_game').attr('gameid', gameId);
	jQuery('#loginForm').find("input[name^='saveGameId']").prop('value', gameId);
	active_game(link);
	set_game_prev_next(gameId);
}

function show_login_layer(id, saveGameId) {
	show_layer(id);
	jQuery('#casinoLoginForm').find("input[name^='saveGameId']").prop('value', saveGameId);
	// cleanen wird mit set_popup_game bei Bedarf gesetzt
	jQuery('#casinoLoginForm').find("input[name^='openPopUpGame']").prop('value', false);
	jQuery('#casinoLoginForm').find("input[name^='openPopUpGameName']").prop('value', '');
	// Sonderbehandlung f�r den Safari, weil der das mit dem Passwort speichern nicht richtig hinbekommt
	if (safari){
		var user = jQuery('#loginForm').find("input[name^='login']").val();
		var pass = jQuery('#loginForm').find("input[name^='password']").val();
		jQuery('#casinoLoginForm').find("input[name^='login']").prop('value', user);				
		jQuery('#casinoLoginForm').find("input[name^='password']").prop('value', pass );	
	}	
}

function set_popup_game(popUpGameName) {
	jQuery('#casinoLoginForm').find("input[name^='openPopUpGame']").prop('value', true);
	jQuery('#casinoLoginForm').find("input[name^='openPopUpGameName']").prop('value', popUpGameName);
}

function reregister() {
	RefreshHandler.update('/system/reregister');
}

function submitHostMessageLayer(cookie) {
	var confirm = jQuery('#hostConfirm').prop('checked') ? true : false;
	var save = jQuery('#save').prop('checked') ? true : false;
	RefreshHandler.update('@layer/layer/submit/'+cookie+'/'+confirm+'/'+save);
}

function submitHostMessageLayer2(cookie, confirm) {
	var save = jQuery('#save').prop('checked') ? true : false;
	RefreshHandler.update('@layer/layer/submit/'+cookie+'/'+confirm+'/'+save);
}

function divOnUpdate(div) {
/* funktioniert im Firefox nur so, sonst erscheint dim_layer nicht */
	jQuery('#dim_layer').fadeIn('slow');
	var obj = jQuery(div);
	layer_offset(obj);
	jQuery(div).fadeIn('slow');
	jQuery('#dim_layer').fadeIn('slow');
}


function trackRegLeftReason(option) {
	RefreshHandler.nav('/controller/leftRegistration/' + option);
	jQuery('#register_cancel_layer, #dim_layer').fadeOut('slow');
}

function set_game_prev_next(gameId) {
	var active = jQuery('#small_picts .small_pict_' + gameId);
	var prev = active.prev();
	if(prev.length == 0) {
		prev = jQuery('#small_picts').children(':last-child');
	}
	var next = active.next();
	if(next.length == 0) {
		next = jQuery('#small_picts').children(':first-child');
	}
	var slideLink = jQuery('#explore_pict_link_previous');
	slideLink.prop('href', prev.find('a').prop('href'));
	slideLink.attr('gameid', prev.attr('gameid'));
	slideLink.unbind('click', slide_game_handler);
	slideLink.bind('click', slide_game_handler);
	slideLink = jQuery('#explore_pict_link_next');
	slideLink.prop('href', next.find('a').prop('href'));
	slideLink.attr('gameid', next.attr('gameid'));
	slideLink.unbind('click', slide_game_handler);
	slideLink.bind('click', slide_game_handler);
}

function select_category(categoryId) {
	set_explore_box_height(categoryId);
	jQuery('#small_picts').html(jQuery('#small_picts_' + categoryId).html());
	jQuery('.flex_tab_bg').removeClass('on');
	jQuery("span[id^='small_picts_link_" + categoryId + "']").parent().addClass('on');
	var gameId = jQuery('#small_picts').children(':first-child').attr('gameid');
	select_game(gameId);
	jQuery('#casinoLoginForm').find("input[name^='saveCatId']").prop('value', categoryId);
	jQuery('#loginForm').find("input[name^='saveCatId']").prop('value', categoryId);
}

function set_explore_box_height(categoryId) {
	var addPx = 47;
	var maxPerRow = 10;
	var baseHeight = 405;
	var count = jQuery('#small_picts_' + categoryId).children().length;
	count = Math.floor(count / maxPerRow);
	count = baseHeight + addPx * count; // + 'px';
	jQuery('#explore_box').css('height', count);
}

//Tooltip
var mytip = null;

function update_tip(e){
	if(mytip){
		var x = (document.all) ? window.event.clientX : e.pageX; 
// Wenn ein Tip kaputt ist, koennten es an diesen auskommentierten Code Zeilen liegen ;-)
//        if(mytip.offsetParent == null) 
        var y = (document.all) ? window.event.clientY + document.documentElement.scrollTop : e.pageY; 
//        else 
//                var y = (document.all) ? window.event.y + mytip.offsetParent.scrollTop : e.pageY; 
//		
		
		
		var dx = mytip.offsetWidth;
		
		if(document.body.clientWidth < dx + x + 15)
			mytip.style.left = (x - dx - 15) + 'px';
		else
			mytip.style.left = (x + 15) + 'px';
		mytip.style.top = (y + 15) + 'px';
	}
}

// Checkt die ben�tigte L�nge, falls gegeben
function checkShowTooltip(text,len){
	if(len == undefined)
		return true;
	if(!text)
		return false;
	
	// Hack, falls HTML �bergeben wird:
	// 	jQuery("<b>test</b>").text() liefert "test"
	// 	jQuery("test<br />test").text() allerdings ""
	// daher umschlie�ende Tags hinzuf�gen, die wieder rausgeparsed werden !

	var max = 0;
	var lines = jQuery("<div>"+text+"</div>").html().split(/<br[^>]*>/gi);
	for(i=0;i<lines.length;i++)
		max = Math.max(max,jQuery.trim(lines[i]).replace(/\s+/g, ' ').length);
	
	return max >= len;
}


// Tooltip f�r TEXT und HTML
function tip(text, len, filterOutHtml){
	if(!checkShowTooltip(text,len))
		return;
	if (filterOutHtml) {
		// #17593/ Metacab v303: HTML-Elemente herausfiltern (hier: Statistikbutton im Gruppenheader auf der Gruppenauswahlseite: subGroupHeader.tag) 
		text = text.replace(/<\/?([a-z][a-z0-9]*)\b[^>]*>?/gi, '');
	}
	mytip = document.getElementById('tooltip');
	mytip.style.display = 'block';
	mytip.innerHTML = text;
	document.onmousemove = update_tip;
}


/* Untip f�r die Tooltips, die per Ajax nachgeladen werden. Zum Beispiel User-Wetten
 * Sorgt daf�r, das noch nicht geladener Tooltip nicht angzeit wird, wenn der Maus-Cursor
 * schon weg ist. Die Variable tipCancelled soll man in der implementierenden Funtion selber abfragen.*/
function cancelTip(tipId) {
	tipCancelled = true;
	untip();
}

function untipElement(el) {
	var t = jQuery(el);
	var visible = t.length && t.is(":visible");
	if (visible) {
		t.hide();
	}
	return visible;
}

function untip(){
	var tips = [mytip, mytip_2, mytip_2, mytip_3, mytip_4, mytip_5, mytip_7, mytip_8, mytip_9, mytip_10];
	for (var i = 0; i < tips.length; i++) {
		if (untipElement(tips[i])) {
			break;
		}
	}
}

function tip_check(this_obj, text, len){
	var this_obj_2 = this_obj.parent().parent();
	if(this_obj_2.find(".hide").is(":hidden")){
		tip(text, len);
	}
}

//Tooltip 2
var mytip_2 = null;

function tip_2(text, this_obj, flip) {
    mytip_2 = document.getElementById('tooltip_2');
    mytip_2.style.display = 'block';
    document.getElementById('tooltip_2_inner').innerHTML = text;
    var offset = this_obj.offset();
    if(flip)
    	mytip_2.style.left = (offset.left - mytip_2.offsetWidth - 2) + 'px';
	else
	    mytip_2.style.left = (offset.left + this_obj.width() + 2) + 'px';
    mytip_2.style.top = (offset.top - 8) + 'px';
}

//Tooltip 3
var mytip_3 = null;
function tip_3(width, dx, dy, head_1, head_2, text, this_obj, pos){
	mytip_3 = document.getElementById('tooltip_3');
	mytip_3.style.display = 'block';
	mytip_3.style.width = width + 'px';
	var tl = document.getElementById('tooltip_3_tl');
	var bl = document.getElementById('tooltip_3_bl');
	if(this_obj.hasClass("redborder")){
		document.getElementById('tooltip_3_tr').innerHTML = head_1;
		document.getElementById('tooltip_3_br').innerHTML = head_2 + text;
		if(pos){
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl_red_bot.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl_bot.png)';
		}
		else{
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl_red.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl.png)';
		}
		document.getElementById('tooltip_3_tr').style.backgroundImage = 'url(/img/tooltip_3_tr_red.png)';
	}
	else{
		document.getElementById('tooltip_3_tr').innerHTML = head_2;
		document.getElementById('tooltip_3_br').innerHTML = text;
		if(pos){
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl_bot.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl_bot.png)';
		}
		else{
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl.png)';
		}
		document.getElementById('tooltip_3_tr').style.backgroundImage = 'url(/img/tooltip_3_tr.png)';
	}
	var offset = this_obj.offset();
	mytip_3.style.marginLeft = dx + 'px';
	pos ? mytip_3.style.top = (offset.top + dy - mytip_3.offsetHeight) + 'px' : mytip_3.style.top = (offset.top - dy) + 'px';
} 

function tip_3Secret(width, dx, dy, head_1, head_2, text, this_obj, styleObj, form, pos){
	mytip_3 = document.getElementById('tooltip_3');
	mytip_3.style.display = 'block';
	mytip_3.style.width = width + 'px';
	var tl = document.getElementById('tooltip_3_tl');
	var bl = document.getElementById('tooltip_3_bl');
	if(jQuery("#"+form+"\\:"+styleObj).hasClass("redborder")){
		document.getElementById('tooltip_3_tr').innerHTML = head_1;
		document.getElementById('tooltip_3_br').innerHTML = head_2 + text;
		if(pos){
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl_red_bot.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl_bot.png)';
		}
		else{
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl_red.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl.png)';
		}
		document.getElementById('tooltip_3_tr').style.backgroundImage = 'url(/img/tooltip_3_tr_red.png)';
	}
	else{
		document.getElementById('tooltip_3_tr').innerHTML = head_2;
		document.getElementById('tooltip_3_br').innerHTML = text;
		if(pos){
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl_bot.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl_bot.png)';
		}
		else{
			tl.style.backgroundImage = 'url(/img/tooltip_3_tl.png)';
			bl.style.backgroundImage = 'url(/img/tooltip_3_bl.png)';
		}
		document.getElementById('tooltip_3_tr').style.backgroundImage = 'url(/img/tooltip_3_tr.png)';
	}
	var offset = this_obj.offset();
	mytip_3.style.marginLeft = dx + 'px';
	pos ? mytip_3.style.top = (offset.top + dy - mytip_3.offsetHeight) + 'px' : mytip_3.style.top = (offset.top - dy) + 'px';
}

//Tooltip 4 (Top Spiel Zeitleiste)
var mytip_4 = null;

function tip_4(this_obj, a, b, c){
	mytip_4 = document.getElementById('tooltip_4');
	document.getElementById('tooltip_4_m').innerHTML = '<span>' + a + '</span><br />' + b + '<br />' + c;
	if(a < 6)
		document.getElementById('tooltip_4_m').style.background = 'url(/img/conference/timeline/tl_ttm2.png) no-repeat left top';
	else
		document.getElementById('tooltip_4_m').style.background = 'url(/img/conference/timeline/tl_ttm.png) no-repeat center top';
	document.getElementById('tooltip_4').style.display = 'block';
	var offset = this_obj.offset();
	var dx = mytip_4.offsetWidth / 2 - 4;
	if(a < 6)
		mytip_4.style.left = (offset.left - 20) + 'px';
	else
		mytip_4.style.left = (offset.left - dx) + 'px';
	mytip_4.style.top = (offset.top - 84) + 'px';
} 

//Tooltip 5 (Casino Gamepreview Floorplan)
var mytip_5 = null;

function tip_5(this_obj, pict, tip){
	mytip_5 = document.getElementById('tooltip_' + tip);
	mytip_5.style.display = 'block';
	mytip_5.firstChild.src = pict;
	var offset = this_obj.offset();
	mytip_5.style.left = (offset.left - 40) + 'px';
	mytip_5.style.top = (offset.top - 140) + 'px';
}

function tip_3Close(){
	mytip_3 = document.getElementById('tooltip_3');
	mytip_3.style.display = 'none';
}

var mytip_7 = null;

function tip_7(text, this_obj){
    mytip_7 = document.getElementById('tooltip_7');
    mytip_7.style.display = 'block';
    document.getElementById('tooltip_7_inner').innerHTML = text;
    var offset = this_obj.offset();
    mytip_7.style.left = (offset.left - 17) + 'px';
    mytip_7.style.top = (offset.top - mytip_7.offsetHeight) + 'px';
    }

//Tooltip 8 (Registration Footer)
var mytip_8 = null;

function tip_8(text, this_obj, newFooter){
	mytip_8 = document.getElementById('tooltip_8');
	mytip_8.style.display = 'block';
	document.getElementById('tooltip_8_inner').innerHTML = text;
	var offset = this_obj.offset();
	mytip_8.style.left = (offset.left - 23) + 'px';
	mytip_8.style.top = '-'+ (mytip_8.offsetHeight - (newFooter ? 49 : 14)) + 'px';
	}

// Tooltip 9 (BR Stats Layer)
var mytip_9 = null;

function tip_9(eventId, eventType, type){
	//einen m�glichen anderen schon aufgeklappten Layer zur�cksetzen
	untipElement(mytip_9);
	var tipId = 'betradar_layer_'+eventId;
	mytip_9 = document.getElementById(tipId);
	if (mytip_9 == null) {
		return;
	}
	var jq_mytip_9 = jQuery(mytip_9);
	//weil die Layers per asynchron geladen werden, zusätzlich �berpr�fen, 
	//ob der Mauszeiger nicht schon weg ist
	if (type == 'liveConference') {
		var eventRow = jq_mytip_9.closest('#jq-event-id-'+eventId);
		if (eventRow.length && !eventRow.is(':hover')) {
			return;
		}
	}
	jq_mytip_9.show();		
	var height = mytip_9.offsetHeight;
	var pBottom;
	var offset;
	function pointerBottom() {
		return offset.top - jQuery(window).scrollTop() > height;
	}
	//StatsLayer auf der LWK
	if (type == 'liveConference') {
		//vor der Berechnung erstmal style.top auf standardwert setzen
		//damit die Berechungn pointerBottom() immer stabile Werte zurückliefert
		mytip_9.style.top = '-4px';
		offset = jq_mytip_9.offset();
		if(pBottom = pointerBottom()){
			mytip_9.style.top = (-height + 36) + 'px';
		} 
	} 
	// StatsLayer bei Icons (auf der Startseite)
	else {
		var eventTypeText = '';
		if(eventType) {
			eventTypeText = '-' + eventType;
		}
		offset = jQuery('#statistics-'+eventId+eventTypeText).offset();
		mytip_9.style.left = (offset.left + 24) + 'px';			
		if(pBottom = pointerBottom()){
			mytip_9.style.top = (offset.top - height + 15) + 'px';
		} else{
			mytip_9.style.top = (offset.top - 10) + 'px';
		}
	}
	var layer = jQuery("#betradar_layer_"+eventId);
	//die Styles: bubble b_left b_shad nicht in template setzen, weil f�r IE deren Reihenfolge im
	//Zusammenhang mit top10/bot10 wichtig ist
	var upClasses = "top10 bubble b_left b_shad";
	var downClasses = "bot10 bubble b_left b_shad";
	if (pBottom) {
		layer.removeClass(upClasses).addClass(downClasses);
		layer.find(".betradar_top").hide();
		layer.find(".betradar_bot").show();
	} else {
		layer.removeClass(downClasses).addClass(upClasses);
		layer.find(".betradar_top").show();
		layer.find(".betradar_bot").hide();
	}
}

//Tooltip 10 (New Timeline)
var mytip_10 = null;

function tip_10(this_obj, a, b){
	mytip_10 = document.getElementById('tooltip_10');
	document.getElementById('tooltip_10_a').innerHTML = a;
	document.getElementById('tooltip_10_b').innerHTML = b;
	mytip_10.style.display = 'block';
	var offset = this_obj.offset();
	var pos_parent = jQuery(this_obj).parent().position();
	if((offset.left - pos_parent.left) > 595){
		mytip_10.style.left = (offset.left - 154) + 'px';
		jQuery('#tooltip_10').addClass('flip');
		}
	else{
		mytip_10.style.left = (offset.left - 14) + 'px';
		jQuery('#tooltip_10').removeClass('flip');
		}
	mytip_10.style.top = (offset.top - 90) + 'px';
}

//Feature Layer
function feature_pop(this_obj){
	if(IE_vers != 6){
		if(this_obj == 'close')
			jQuery("#feature_pop, #feature_pop_img, #dim_layer").fadeOut('slow');
		else if(this_obj.get(0).tagName.toLowerCase() == 'img'){
			jQuery("#feature_pop_img").find("img").prop("src", this_obj.prop("src"));
			jQuery(".pop_close").css('left', '794px');
			jQuery("#feature_pop_img").css('top', (60 + jQuery(window).scrollTop()) + 'px');
			jQuery("#feature_pop_img, #dim_layer").fadeIn('slow');
			}
		else{
			jQuery("#feature_pop .content").html(this_obj.parent().find('.hide').html());
			jQuery(".pop_close").css('left', '494px');
			jQuery("#feature_pop").css('top', (60 + jQuery(window).scrollTop()) + 'px');
			jQuery("#feature_pop, #dim_layer").fadeIn('slow');
			}
		}
	}

function popupMobileDemo(wrongBrowser){
	if (chrome != -1 || safari != -1 || fireFox != -1 || (msie != -1 && IE_vers >= 8)){
		popwin = window.open('/site/mobileDemo.faces',"popup","toolbar=no,scrollbars=no,menubar=no,resizable=yes,status=no,width=400,height=769,dependent=yes,top=0");
    	popwin.focus();
	} else
		showAlertLayer(wrongBrowser);
}

function pop_next(num){
	jQuery("#feature_pop .content").html(jQuery("#pop_" + num).html());
	}



//Subnav Layer mit versch. Verz�gerungen bei
//Tabbing, erstmaliges aufklappen, schlie�en
var t = null, sub_layer = null;
var tabbing = false;
function subnav(this_obj, type, flag){
	clearTimeout(t);
	if(!flag){
		sub_layer = jQuery(this_obj);
		if (type>0){
			if (!tabbing){ 
				t = setInterval("subnav('', '"+type+"', '1')", 300);
			}
			else{ 
				t = setInterval("subnav('', '"+type+"', '1')", 150);
			}
		}
		else t = setInterval("subnav('', '"+type+"', '1')", 100);
	}
	else{
		jQuery("#sub_nav").find(".sub_nav_drop, .peak").hide();
		if(type > 0){
			tabbing = true;
			sub_layer.find('.sub_nav_drop, .peak').show();
		}
		else{
			tabbing = false;
			sub_layer.find('.sub_nav_drop, .peak').hide();
		}
	}
}

//Nachladen der Navigation.
function loadSubnav(telekomLandingPage){
	jQuery("#sub_nav_2").load('/template/subNav.faces?withLayer'+(telekomLandingPage?'&telekom':''));
}

//Alle Drop-Down Layer der Navigation verstecken
//F�r Onclick bei den Casino spielen.
function hideSubnavLayers(){
	jQuery("#sub_nav .sub_nav_drop").hide();
}

function tog_messages(this_obj){
	jQuery('#messages_layer').find('.content.hide').slideUp("slow");
	jQuery('#messages_layer').find('.cf').removeClass('on');
	if(jQuery(this_obj).parent().parent().find('.content.hide').is(":hidden")){
		jQuery(this_obj).parent().addClass('on');
		jQuery(this_obj).parent().parent().find('.content.hide').slideDown("slow");
		}
	jQuery('.scrollbar_3').tinyscrollbar({ sizethumb: 55 });
	}

var selectedMessage = null;
function saveSelectedMessage(id){
	selectedMessage = id;
}

function removeSelectedMessage(){
	selectedMessage = null;
}

function restoreSelectedMessage(){
	if (selectedMessage != null && selectedMessage != undefined){
		tog_messages(jQuery("#"+selectedMessage));
	}
}

/*---------------------------------------------------------------------------------------------------------*/
/* -------------------------- ebet js -------------------------------------------------------------------- */
/* ------------------------------------------------------------------------------------------------------- */

function doClear(theText) {
	if (theText.value == theText.defaultValue) {
		theText.value = ""
	}
}

function doClearPassword(passwordField) {
	if (passwordField.value == '--------') {
		passwordField.value = "";
	}
}

function doResetPassword(passwordField) {
	if (passwordField.value == '') {
		passwordField.value = '--------';
	}
}


/** =============== BEGIN: Verwaltung von Buttons =============== **/

// >>> Marker for odds
var markedResults = [];
var markedLiveResults = [];

function getMarkedResults() {
	return markedResults.join(",");
}
function markResults() {
	for (i = 0; i < markedResults.length; i++) {
		markResultButton(markedResults[i], false);
		markLiveResultButton(markedResults[i]);
	}
}

/**
 * wird beim Klick auf dem Button aufgerufen (optimistisches Toggln von Buttons)
 * Es gibt momentan 3 Button-Typen:
 * Typ 1: nicht live JSF-Buttons - die w�rden auf lange Sicht durch Spring-MVC Buttons ersetzt
 * Typ 2: Spring-MVC Buttons aus der LWK und altem LWK-Gadget (Tabellen) - sollte auch durch das neue Design ersetzt werden
 * Typ 3: Spring-MVC Buttons - neues Design, soll alle Typen ersetzen
 * */
function tr(resultId,region) {
	//Button Typ 1
	var button = jQuery(":button[name=q"+resultId+"]");
	if(button.hasClass('roll_red')) {
		addMarkStyle(button);
	} else {
		removeMarkStyle(button);
	}
	//Button Typ 2
	getResultButtons(false, resultId).each(
		function() {
			var button = jQuery(this);
			if (button.hasClass('c_but') || button.hasClass('c_but_up') 
					|| button.hasClass('c_but_down')) {
				addLiveMarkStyle(button, false);
			} else {
				removeLiveMarkStyle(button, false);
			}
		}
	);
	//Button Typ 3
	getResultButtons(true, resultId).each(
		function() {
			var button = jQuery(this);
			if(!button.hasClass('on')) {
				addLiveMarkStyle(button, true);
			} else {
				removeLiveMarkStyle(button, true);
			}
		}
	);
	var delay = 0;
	if(eBetFeatures['webDelayToggleResults'] == 1)
		delay = 1000;
	setTimeout(function(){		
		toggleResult(resultId,region);// a4j:jsFunction in editor.xhtml
	},delay);
}

function getResultButtons(newLiveButton, resultId, parent) {
	var selector;
	//nach kompletter Umstellung auf Spring MVC kann die 2.Bedingung entfernt werden
	if (newLiveButton) {
		selector = resultId ? '.qbut-'+resultId : '.qbut';
	} else {
		selector = resultId ? ":button[name=ql"+resultId+"]" : ':button[name^=ql]';
	}
	var button = parent ? jQuery(parent).find(selector) : jQuery(selector);
	return button;
}

function openSelectedQuote(resultId){
	var button = jQuery(":button[name=q"+resultId+"]");
	if (!button.length) {
		button = getResultButtons(true, resultId);
	}
	var btnText = button.html().replace( /,/,"." );
	var linkString = "https://" + window.location.hostname;
	linkString += "/entry/quoteBanner.faces?language=de";	
	linkString += "&result0=" + resultId;
	linkString += "&quote0=" + btnText; 
	linkString += "&cs=true&forward=true&gaCS=true";
	window.open(linkString);
}

function markResult(resultId) {
	markedResults.push(resultId);
	function _markResults() {
		var lBtn = jQuery(this);
		markedLiveResults[resultId] = lBtn.text();
	}		

	getResultButtons(false, resultId).each(_markResults);
	getResultButtons(true, resultId).each(_markResults);
	markResultButton(resultId);
	markLiveResultButton(resultId);
}
/**wird vom Ticket-Editor aufgerufen nachdem es neu geladen wurde (zB beim Entfernen der Quote)*/
function clearMarks() {
	markedResults = [];
	markedLiveResults = [];
	var btns = jQuery('.red_quote');
	for (var i = 0; i < btns.length; i++) {
		var b = jQuery(btns[i]);
		removeMarkStyle(b);
	}

	getResultButtons(false, null).each(
		function() {
			removeLiveMarkStyle(jQuery(this), false);
		}		
	);
	
	getResultButtons(true, null).each(
		function() {
			removeLiveMarkStyle(jQuery(this), true);
		}
	);
}

function markResultButton(resultId) {
	var btns = document.getElementsByName("q"+resultId);
	for (var i = 0; i < btns.length; i++) {
		var b = jQuery(btns[i]);
		addMarkStyle(b);
	}
}

function addMarkStyle(elem) {
	elem.removeClass('roll_red');
	elem.addClass('white');
	elem.addClass('red_quote');
	elem.blur();
}
function removeMarkStyle(elem) {
	elem.addClass('roll_red');
	elem.removeClass('white');
	elem.removeClass('red_quote');
	elem.blur();
}

/**wird bei jedem Update von der Event-Zeile (LWK-Gadget) aufgerufen, 
 * um die vorher selektierte Results zu wiederherstellen*/
var refreshEditorTimer; 
function markLiveResults(parent) {
	var shouldUpdateEditor = false;
	for (i = 0; i < markedResults.length; i++) {
		if(markLiveResultButton(markedResults[i], parent)) {
			shouldUpdateEditor = true;
		}
	}
	if (shouldUpdateEditor) {
		log('updating editor');
		if (refreshEditorTimer) {
			clearTimeout(refreshEditorTimer);
		}
        //markLiveResults() wird bei einzelnen Event-Zeilen. Es wird versucht, falls die selektierten Quoten, 
		//aktualisiert wurden, den Wettschein nicht sofort f�r jede Zeile aufzurufen, sondern per Timeout
		//eine Aktualisierung des Wettschein f�r alle �nderungen zu feuern
		refreshEditorTimer = setTimeout(function() { refresh(); }, 1000);
	}
}

//parent - kleine Optimierung: suchen nicht in dem ganzen Dom-Baum, sonder nur
//unter dem ausgew�hlten Knoten
function markLiveResultButton(resultId, parent) {
	var shouldUpdateEditor = false;
	function _mrb(newLiveButton, button) {
		addLiveMarkStyle(button, newLiveButton)
		//�berpr�fen, ob die markierten Quoten sich ge�ndert haben
		//um zu entscheiden, ob der TicketEditor refresht werden kann.
		var quote = markedLiveResults[resultId];
		if (quote !== undefined && button.text() != quote) {
			shouldUpdateEditor = true;
		}
	}

	//Typ 2
	getResultButtons(false, resultId, parent).each(
		function() {
			_mrb(false, jQuery(this))
		}
	);

	//Typ 3
	getResultButtons(true, resultId, parent).each(
		function() {
			_mrb(true, jQuery(this))
		}
	);
	return shouldUpdateEditor;
}

function addLiveMarkStyle(elem, newLiveButton) {
	if (newLiveButton) {
		elem.addClass('on');
	} else {
		if (elem.hasClass('c_but_up')) {
			elem.removeClass('c_but_up');
			elem.addClass('c_but_on_up');
		} else if (elem.hasClass('c_but_down')) {
			elem.removeClass('c_but_down');
			elem.addClass('c_but_on_down');
		} else if (elem.hasClass('c_but_paused')) {
			elem.removeClass('c_but_paused');
			elem.addClass('c_but_on_paused');
		} 
		//"c_but" - letzte Bedingung
		else if (elem.hasClass('c_but')) {
			elem.removeClass('c_but');
			elem.addClass('c_but_on');
		}
	}
}

function removeLiveMarkStyle(elem, newLiveButton) {
	if (newLiveButton) {
		elem.removeClass('on');		
	} else {
		if (elem.hasClass('c_but_on_up')) {
			elem.removeClass('c_but_on_up');
			elem.addClass('c_but_up');
		} else if (elem.hasClass('c_but_on_down')) {
			elem.removeClass('c_but_on_down');
			elem.addClass('c_but_down');
		} else if (elem.hasClass('c_but_on_paused')) {
			elem.removeClass('c_but_on_paused');
			elem.addClass('c_but_paused');
		} 
		//"c_but" - letzte Bedingung
		else if (elem.hasClass('c_but_on')) {
			elem.removeClass('c_but_on');
			elem.addClass('c_but');
		} 
	}
}

/** =============== END: Verwaltung von Buttons =============== **/

// <<< Marker for odds

// >>> Marker for groups

var markedGroups = new Array();

function markGroup(groupId) {
	markedGroups.push(groupId);
	markNavTreeNode(groupId);
}

function markNavTreeNode(groupId) {
	jQuery('#t'+groupId).addClass('on');
	// Dummy-Klasse hinzuf�gen f�r den jQuery-Selektor in clearNavTreeMarks()
	jQuery('#t'+groupId).addClass('groupSelected'); 
	var obj = jQuery('#t'+groupId).parent().parent().parent();
	if (jQuery(obj).is(':hidden')) {
		jQuery(obj).show();
		jQuery(obj).parent().addClass("bold down");
		if (jQuery(obj).parent().parent().is(':hidden')) {
			jQuery(obj).parent().parent().show();
			jQuery(obj).parent().parent().parent().find(".moreGroups").hide();
			jQuery(obj).parent().parent().parent().find(".lessGroups").show();
		}
	}
}

function showGroups(groupId) {
	jQuery("#nav_more"+groupId).hide();
	jQuery("#nav_corner_off"+groupId).show();
	jQuery("#nav #ul"+groupId+".hide").slideDown("fast");
}

function hideGroups(groupId) {
	jQuery("#nav_corner_off"+groupId).hide();
	jQuery("#nav #ul"+groupId+".hide").slideUp("fast");
	jQuery("#nav_more"+groupId).show();
}

function clearNavTreeMarks() {
	var icons = jQuery('.groupSelected');
	for(var i = 0; i < icons.length; i++) {
		var icon = icons[i];
		jQuery(icon).removeClass('on');
	}
}

function groupSelectAll(groupId) {
	var navEntry = jQuery('#navEntry' + groupId);
	jQuery(navEntry).parent().parent().find("ul").toggle();
	if(jQuery(navEntry).parent().parent().find("ul").is(':hidden')){
		jQuery(navEntry).find('.left').removeClass("bold down");
		jQuery(navEntry).find('.check_r').removeClass("bold");
		jQuery(navEntry).parent().removeClass("pointline");
	} else{
		jQuery(navEntry).find('.left').addClass("bold down");
		jQuery(navEntry).find('.check_r').addClass("bold");
		jQuery(navEntry).parent().addClass("pointline");
	}
}

// <<< Marker for groups


// >>> Popup functions
var popwin = null;

function popupScrollbars(url, name, width, height) {
	name = name.replace(/ /g,"");
	name = name.replace(/./g,"");
	popwin = window.open(url,name,"toolbar=no,scrollbars=yes,menubar=no,resizable=yes,status=no,width="+width+",height="+height+",dependent=yes");
    popwin.focus();
}

function popupNoScrollbars(url, name, width, height) {
	name = name.replace(/ /g,"");
	name = name.replace(/./g,"");
    popwin = window.open(url,name,"toolbar=no,scrollbars=no,menubar=no,resizable=yes,status=no,width="+width+",height="+height+",dependent=yes");
    popwin.focus();
}

// aus einem Popup das Elternfenster delegieren eine neue Seite aufzurufen und das Popup schliessen 
function openPage(url){
	document.location.href = url;
	popwin.close();
}

function openPageWithLogin(url,loggedIn,paymentLogin){
	if (popwin)
		popwin.close();
	if (loggedIn)
		document.location.href = url;
	else{
		jQuery('#casinoLoginForm').find("input[name^='viewId']").prop('value', url);
		if(paymentLogin != undefined)
			jQuery('#casinoLoginForm').find("input[name^='paymentLogin']").prop('value', paymentLogin);
		else
			jQuery('#casinoLoginForm').find("input[name^='paymentLogin']").prop('value', 'true');
		show_layer('dim'); 
		show_login_layer('login','');
	}
}
// <<< Popup functions


// >>> Ticket-Editor
function enterCode(message) {
	with (document.editorForm) {
		var enteredCode = prompt(message);
		if (enteredCode) {
			action = 'TODO' + enteredCode;
			// url value="/ticket/editor.do?method=loadBonus UND code="
			submit();
		}
	}
}

function stakeClick() {
	document.getElementById('editorForm\:amountDisplay').select();
	document.getElementById('editorForm\:selected').value = 'stake';
	return true;
}
function totalStakeClick() {
	document.getElementById('editorForm\:totalStakeDisplay').select();	
	document.getElementById('editorForm\:selected').value = 'totalStake';
	return true;
}
function winClick() {
	document.getElementById('editorForm\:winDisplay').select();
	document.getElementById('editorForm\:selected').value = 'win';
	return true;
}

// >>> Selection Handling
// ts: toggle special bets
function ts(component) {
	//var component = jQuery(component).parent();
	if (jQuery(component).parent().parent().find(".hide").is(':hidden')) {
		jQuery(component).addClass("m_on");
		jQuery(component).parent().parent().find(".hide").show();
		jQuery(component).parent().parent().find(".sheet_slider_body_off").addClass("sheet_slider_body");
		jQuery(component).parent().parent().find(".sheet_slider_body_off").removeClass("sheet_slider_body_off");
	} else {
		jQuery(component).removeClass("m_on");
		jQuery(component).parent().parent().find(".hide").hide();
		jQuery(component).parent().parent().find(".sheet_slider_body").addClass("sheet_slider_body_off");
		jQuery(component).parent().parent().find(".sheet_slider_body").removeClass("sheet_slider_body");
	}
	jQuery("#help_resultType_layer").hide();
}

// <<< Selection Handling

// >>> Ajax4JSF error/expired handling
var A4J_ERROR_MESSAGE = '';
if (typeof A4J != 'undefined') { // some pages made error "A4J not defined"
	A4J.AJAX.onError = function(req,status,message) {
		if (!A4J_ERROR_MESSAGE == '') {
			showAlertLayer(A4J_ERROR_MESSAGE);
			A4J_ERROR_MESSAGE = "";
		}
	}
	A4J.AJAX.onExpired = function(loc,expiredMsg) {
		// Auf jsp leiten, damit Sprache richtig codiert wird
		location.href = CONTEXT_PATH + "/index.jsp";
	}
}

// >>> common functions
function showHintLayer(id, top, left) {
	jQuery("#"+id).show();
	jQuery("#"+id).css({'top' : top+'px', 'left' : left+'px'});
}
// <<< common functions


// Backup aus popupUtil.js
// wird wahrscheinlich gar nicht mehr benutzt...

/* 
Version: 2004-10-25

default values:
    name=<random value>
    width=760
    height=540
    top=10
    left=50
    status=yes
*/
function popup(url, name, width, height, top, left, status) {
	if ((typeof name) == "undefined") {
	    name = "popup_" +
	        createRandomString("abcdefghiklmnopqrstuvwxyz", 8);
	}
	if ((typeof width) == "undefined")
	    width = 760;
	if ((typeof height) == "undefined")
	    height = 540;
	if ((typeof top) == "undefined")
	    top = 10;
	if ((typeof left) == "undefined")
	    left = 50;
	if ((typeof status) == "undefined")
	    status = "yes";
	var params = "dependent=yes,location=no,menubar=no,toolbar=no,resizable=yes,scrollbars=yes"
	    + ",width=" + width
	    + ",height=" + height
	    + ",top=" + top
	    + ",left=" + left
	    + ",status=" + status;
	var popwin = window.open(url, name, params);
	popwin.focus();
}

function popupStatistics(lang, param, clientId) {
	var url;
	// Betradar benutzt als Laenderkuerzel fuer Serbien "sr" statt "rs"
	if (lang == 'language=rs') {
		lang = 'language=sr';
	}
	if(param == null)
		url = 'http://www.stats.betradar.com/s4/?clientid='+clientId+'&' + lang;
	else 
		url = 'http://www.stats.betradar.com/s4/?clientid='+clientId+'&' + lang + '&' + param;
	popup(url, 'stats', 1089, 778);
}

function createRandomString(chars, string_length) {
	var randomstring = '';
	for (var i=0; i<string_length; i++) {
	    var rnum = Math.floor(Math.random() * chars.length);
	    randomstring += chars.substring(rnum, rnum+1);
	}
	return randomstring;
}

// >>> payment
function radioClick(component, field, other) {
	window.setTimeout(function() {
		document.getElementById(field).value = component.value;
		if (other)
			document.getElementById(other).value = '';
		}, 0);
}
function selectTemplate(selectedDataPk) {
	document.getElementById('form:selectedDataPk').value = selectedDataPk;
	var elm = document.getElementById("securityCode" + selectedDataPk);
	if (elm != null) {
		document.getElementById('form:selectedSecurityCode').value = elm.value;
	}
}
function submitForm(submitButton, text) {
	submitButton.value=text;
	submitButton.onclick=doNothing;
}
function doNothing() {
	return false;
}
// <<< payment

// help_layer

function getScrollPosition() {
	var scrollPosition;
	if (window.pageYOffset) {
		scrollPosition = window.pageYOffset;
	} else if (document.documentElement.scrollTop) {
		scrollPosition = document.documentElement.scrollTop;
	} else if ( document.body.scrollTop ) {
		scrollPosition = document.body.scrollTop;
	} else {
		scrollPosition = 0;
	}
	return scrollPosition;
}

function getScreenWidth() {
	var screenWidth;
	if (document.body.offsetWidth) {
		screenWidth = document.body.offsetWidth;
    } else {
  		screenWidth = window.innerWidth;
    }
	return screenWidth;
}

var clickX;
var clickY;
function showHelp (text) {
	var scrollPosition = getScrollPosition();
	var screenWidth = getScreenWidth();
	jQuery('#help_resultType_layer').css("left", (clickX + 280) > screenWidth ? screenWidth - 280 : clickX);
	jQuery('#help_resultType_layer').css("top", clickY + scrollPosition);
	document.getElementById('help_resultType_layer_inner').innerHTML = text;
	jQuery('#help_resultType_layer').show();	  
}

function closeHelp() {
	jQuery('#help_resultType_layer').hide();	  
}

function showArchiveLayer() {
	var scrollPosition = getScrollPosition();
	var screenWidth = getScreenWidth();
	jQuery('#archive_layer').css("left", (clickX + 280) > screenWidth ? screenWidth - 280 : clickX);
	jQuery('#archive_layer').css("top", clickY + scrollPosition);
	jQuery('#archive_layer').show();	  
}

function closeArchiveLayer() {
	jQuery('#archive_layer').hide();	  
}

function showAlertLayer(text) {
	hideCasinoGame(function() {
		var text_split = text.split('#*#');
		document.getElementById('alert_layer_inner').innerHTML = text_split[0];
		if(text_split[1])
			document.getElementById('alert_layer_inner_headline').innerHTML = text_split[1];
		jQuery('#layer_bg').show();
		jQuery('#alert_layer').show();
	});
}

function showSessionTimeoutLayer() {
	hideCasinoGame(function() {
		if(eBetFeatures['extendedSessionLifetime'] == 1) {
			jQuery('#layer_bg').show();
			var layers = jQuery('.sessionTimeout_layer_new');
			layers.hide();
			var layer = jQuery(layers[Math.floor((Math.random()*2))]);
			var reg = layer.find('.buttonWrap .reg');
			if (jQuery.cookie('ru') != null)
				reg.hide();
			else
				reg.show();
			jQuery(layer).show();
		} else {
			jQuery.cookie('st', 'on', {path: '/', expires: 0});
			window.location.href = '/index.jsp';
		}
	});
}

function hideSessionTimeoutLayer() {
	window.location = window.location;
}

function closeAlertLayer() {
	jQuery('#layer_bg').hide();
	jQuery('#alert_layer,#sessionTimeout_layer').hide();
	showCasinoGame();
}

var allowRedirMain = true;
function closeCasinoLayer(casino) {
	allowRedirMain = false;
	RefreshHandler.update('@layer/layer/submitCasino/' + casino);
	hide_layer('germany_casino_closed');
	hide_layer('dim');
}

function redirMain(url) {
	if (allowRedirMain) {
		hide_layer('germany_casino_closed');
		hide_layer('dim');
		url();
	}
}

function transferCasinoChips(url, login) {
	allowRedirMain = false;
	if(login) {
		hide_layer('germany_casino_closed');
		hide_layer('dim');
		openPageWithLogin(url, false, false);
	} else {
		window.location.href = url;
	}
}

var toCasinoMain;

function setToCasinoMain(toCasinoMainJS) {
	toCasinoMain = toCasinoMainJS;
}

function trackClick (clickEvent) {
	var obj;
	var insideLayer = false;
	if (clickEvent != null) {
		obj = clickEvent.target;
	} else {
		clickEvent = window.event;
		obj = clickEvent.srcElement;
	}
	if (document.getElementById) {
		clickX  = clickEvent.clientX;
		clickY = clickEvent.clientY;
	} else if (document.all) {
		clickX = clickEvent.clientX;
		clickY = clickEvent.clientY;
	}
	
	if (jQuery(obj).hasClass("layerClass")) {
		jQuery(obj).find(".layerClass").hide();
		insideLayer = true;
	} else {
		insideLayer = jQuery(obj).parents('div').hasClass("layerClass");
	}
		
	if(!insideLayer 
		&& jQuery(obj).prop('id') == "dim_layer" 
		&& (jQuery('#welcome_layer').length == 0 
			|| jQuery('#welcome_layer').css('display') == 'none')
		&& (jQuery('#sh_existing_customer_layer').length == 0 
			|| jQuery('#sh_existing_customer_layer').css('display') == 'none')
		&& (jQuery('#test_result_layer').length == 0 
			|| jQuery('#test_result_layer').css('display') == 'none')){
		hideDimLayer();
		if(toCasinoMain != undefined)
			toCasinoMain();
	}
	if(!insideLayer){ 
		jQuery('#lang_layer').hide();
	}
	var archLayer = jQuery('#cal_archive_layer');
	if (archLayer.is(':visible') && jQuery(obj).parents('#cal_archive_layer').length == 0){
		archLayer.hide();
	}
}

document.onmousedown = trackClick;

// editor

var lock = false;
var delayTimeout;

function refresh() {
	if (!lock) {
		lock = true;
		refreshServer();
	}
	refreshEditorTimer = null;
}

function onReactionComplete() {
	A4J_ERROR_MESSAGE = ''; 
	lock = false; 
	reRenderHead(); 
	hideDelayLayer();
}

function delay(delayInput) {
	if (delayInput > 0) {
		jQuery('#counter').html(delayInput);
		delayTimeout = window.setTimeout('delay('+(delayInput - 1)+')', 1000);
	}
}

function hideDelayLayer() {
	jQuery('#delay_layer,#delay_layer_live, #delay_layer_inner').hide();
	clearTimeout(delayTimeout);
	showCasinoGame();
	hide_layer('dim');
	jQuery('#layer_bg').hide();
}

function showDelayLayer() {
	hideCasinoGame(function() {
		var popup = jQuery('#delay_layer_inner');
		popup.css('left', '50%');
		popup.css('top', '200px');
		popup.css('margin-left', '-150px');
		jQuery('#delay_layer, #delay_layer_inner').show();
		jQuery('#layer_bg').show();
	}, true);
}

function showDelayLayerLive() {
	if (jQuery("#identityLayerText").text().length > 75) { 
		jQuery("#delay_layer_spacer").removeClass("space_80").addClass("space_60");
	}
	jQuery('#delay_layer_live').show();
	jQuery('#layer_bg').show();
}

function hideDimLayer() {
	jQuery('#dim_layer').fadeOut('slow'); 
	jQuery('#layer_bg').hide();
	jQuery(".layerClass").hide();
	removeVideoContent();
	showCasinoGame();
}

// Deletes all elements with the class videoContent. 
// This is used for stopping videos, because on a fade out 
// videos in an iframe are still playing in the background.
function removeVideoContent() {
	jQuery(".videoContent").remove();
}

function showCenteredDelayLayer(showDelayLayer) {
	var size = new Array();
	size = getWindowSize();
	var popup = jQuery('#delay_layer_inner');
	var left = (size[0] - popup.width()) / 2;
	var top = (size[1] - popup.height()) / 2;
	top -= 20;
	popup.css('left', left);
	popup.css('top', top);
	popup.css('margin-left', '0px');	
	jQuery('#delay_layer, #delay_layer_inner').show();
	jQuery('#layer_bg').show();
}

function iFrameHeight(iframeId) {
	if(document.getElementById && !(document.all)) {
		h = document.getElementById(iframeId).contentDocument.body.scrollHeight;
	} else if(document.all) {
		h = document.frames(iframeId).document.body.scrollHeight;
	}
	if(h > 0) {
		h += 10;
		jQuery('#' + iframeId).css('height', h + 'px');
	}
}

//Button 'Daten �ndern' und 'Cancel' (Kontaktdaten)
function but_change_data(what, suffix, bg){
	this_obj = jQuery("#change" + suffix).parent();
	if(what == 1){
		this_obj.find(".off" + suffix).hide();
		this_obj.find(".hide" + suffix).show();
		if (!bg)
			this_obj.find(".bg_grey").css('backgroundColor','#ffffff');
	}
	else{
		this_obj.find(".off" + suffix).show();
		this_obj.find(".hide" + suffix).hide();
		this_obj.find(".bg_grey").css('backgroundColor','#ebebeb');
	}
}

// affiliate
function toggleOverview(field) {
	if (field == 'overviewInformation') {
		jQuery('.overviewInformation').show();
		jQuery('.overviewSportsbook').hide();
		jQuery('.overviewCasino').hide();
		jQuery('.overviewBilling').hide();
	}
	if (field == 'overviewSportsbook') {
		jQuery('.overviewInformation').hide();
		jQuery('.overviewSportsbook').show();
		jQuery('.overviewCasino').hide();
		jQuery('.overviewBilling').hide();
	}
	if (field == 'overviewCasino') {
		jQuery('.overviewInformation').hide();
		jQuery('.overviewSportsbook').hide();
		jQuery('.overviewCasino').show();
		jQuery('.overviewBilling').hide();
	}
	if (field == 'overviewBilling') {
		jQuery('.overviewInformation').hide();
		jQuery('.overviewSportsbook').hide();
		jQuery('.overviewCasino').hide();
		jQuery('.overviewBilling').show();
	}
}

function startWaitAjax() {
	jQuery('body').css('cursor','wait');
}
function stopWaitAjax() {
	jQuery('body').css('cursor','default');
}

// search
function encodeRedirectLocation(redirect, url, input) {
	if (redirect) {
		var redirectUrl = CONTEXT_PATH + url + '?input=' + input;
		location.href = redirectUrl;
	}
}

// verhindert im Editor die Eingabe von Zeichen
// erlaubt die Eingabe von Ziffern und einem bestimmten Trennzeichen
function ensureValidInput(event, separator, disallowSeparator) {
	var keycode;
	var type = event.type;
	if (window.event) {
		keycode = window.event.keyCode;
	} else if (event) {
		keycode = event.which;
	} else {
		return true;
	}
	
	// Zahlen erlauben f�r 0-9 oder bei onkeydown �ber numblock
	if (keycode > 47 && keycode < 58) {
		return true;
	}
	
	// Trennzeichen erlauben
	if (keycode == separator)
		return !disallowSeparator;
	
	// bei onkeydown numblock zahlen abfangen komma und punkt abfangen wenn dies der seperator ist
	if (type=="keydown"){
		if (keycode > 95 && keycode < 106){
			return true;
		}
		//Komma oder Komma �ber Numblock
		if (separator == 44 && (keycode == 108 || keycode == 188)){
			return true;
		}
		
		//Punkt oder punkt �ber numblock
		if (separator == 46 && (keycode == 110 || keycode == 190)){
			return true;
		}
	}
	
	// Steuerzeichen erlauben
	// backspace, line feed, carriage return, left arrow, right arrow
	var kcok = new Array(8, 10, 13);

	while (kcok.length > 0) {
		if (keycode == kcok.pop()) {							
			return true;
		}
	}

	return false;
}

function deleteInvalidInput(event){
	var node = jQuery((event.currentTarget) ? event.currentTarget : event.srcElement);
	var text = node.val();
	if (text != ''){
		node.val(text.replace(/[^0-9,.]/g,''));
	}
}

var lastMouseMove = new Date().getTime();
var lastPollingTime = 0;
var sessionTimeout = 15 * 60 * 1000 + eBetFeatures['noopUpdateDurationSec'] * 1000;
var refreshFrameworkTimeout = 5 * 60 * 1000;
var stDisplayed = false;
var stTimeout = undefined;


function handleMove(moveEvent) {
	var newLastMouseMove = new Date().getTime();
		
	if (stDisplayed && stTimeout == undefined) {
		jQuery.cookie('st',null,{path : '/'});
		RefreshHandler.start();
		window.setTimeout(function() {
			jQuery('#sessionTimeoutArea').remove();
		}, 3000);
	}
	
    if (newLastMouseMove - Math.max(lastMouseMove,lastPollingTime) < sessionTimeout) {
		lastMouseMove = newLastMouseMove;
	} else {
		document.onmouseover = null;	
		//Es gibt keinen Sinn den Inaktivit�tsdialog auf dieser Seite anzuzeigen.
		if (window.location.href.indexOf('unauthorized.faces') == -1) {
			showSessionTimeoutLayer();
			window.setTimeout(function() {
				hideSessionTimeoutLayer();
			}, 6000);
		}
	}
}

//wird von GWT und JS-Ajax-Funktionen aufgerufen 
//um die Zeit vom letzten Response sich zu merken
function updateLastPollingTime() {
	lastPollingTime = new Date().getTime();
}

function getLastMouseMove() {
	return lastMouseMove;
}

document.onmouseover = handleMove;

var pollTimer;
function doPoll(_url, loggedIn, pollInterval, pollDuration, noopInterval, noopDuration) {
		var compareValue = new Date().getTime() - getLastMouseMove();
		var pollingTimeExpired = compareValue > pollDuration;
		var noopTimeExpired = compareValue > noopDuration + pollDuration;
		if (loggedIn && pollingTimeExpired) {
			if (pollingTimeExpired && !noopTimeExpired && eBetFeatures['extendedSessionLifetime'] == 1) {
				jQuery.ajax({
					type: "GET",
					url: "/noop",
					cache: false,
					dataType: "text",
					complete: function(XMLHttpRequest, textStatus) {
						schedulePollTimer(_url, loggedIn, pollInterval, pollDuration, noopInterval, noopDuration, false);
					}
				});
			} else {
				schedulePollTimer(_url, loggedIn, pollInterval, pollDuration, noopInterval, noopDuration, true);
			}
	    } else if (loggedIn) {
			jQuery.ajax({
				type: "GET",
				url: _url,
				cache: false,
				dataType: "text",
				success: function(data, textStatus) {
					if (data != '') {
						poll(data);
					}
					updateLastPollingTime();
				},
				error: function(XMLHttpRequest, textStatus, errorThrown) {
					updateLastPollingTime();
				},
				complete: function(XMLHttpRequest, textStatus) {
					schedulePollTimer(_url, loggedIn, pollInterval, pollDuration, noopInterval, noopDuration, true);
				}
			});
		}		
}

function schedulePollTimer(_url, loggedIn, pollInterval, pollDuration, noopInterval, noopDuration, poll) {
	var interval = poll ? pollInterval : noopInterval;
	pollTimer = window.setTimeout(function() {
		doPoll(_url, loggedIn, pollInterval, pollDuration, noopInterval, noopDuration);
	}, interval);
}

RefreshHandler.onUpdatesRequested = updateLastPollingTime;
window.setInterval(function() {
	if (new Date().getTime() - getLastMouseMove() > refreshFrameworkTimeout) {
		RefreshHandler.stop();
    } else {
		RefreshHandler.start();
    }
}, 1000);

// F�gt der Affiliate Links&Banner Struktur nachtr�glich Formatierungen hinzu
// Es setzt landesspezifische Bilder neben die Sprachheader
// Und berechnet die Alternierung der Zeilenfarben. Dabei muss je nach Anzahl der Banner
// eine unterschiedliche 'line-height' gesetzt werden.
function markBannerButtons() {
	var topics = jQuery('#affiliateBanner ul.level5 > li.open > a');
	for (i = 0; i < topics.length; i++) {
		var content = jQuery(topics[i]).text().trim();
		// Es kann nur die Textinformation des entsprechenden Knotens verwendet werden
		// Dadurch wird es jedoch Reihenfolge-unabh�ngig
		var img = '';
		if (content == 'Deutsch' || content == 'German' || content == 'Tedesco') {
			img = 'german';
		} else if (content == 'Englisch' || content == 'English' || content == 'Inglese') {
			img = 'uk';
		} else if (content == 'Italienisch' || content == 'Italian' || content == 'Italiano' || content == 'Italy') {
			img = 'italy';
		} else if (/Franz.sisch/.test(content) || content == 'French') {
			img = 'fr';
		} else if (content == 'Dutch') {
			img = 'netherlands';	
		} else if (content == 'Schweiz' || content == 'Swiss' || content == 'Svizzero') {
			img = 'swiss';
		} else if (/T.rkisch/.test(content) || content == 'Turkey') {
			img = 'turkey';
		} else if (content == 'Denmark') {
			img = 'denmark';
		}
		if (img != '') {
			jQuery(topics[i]).html('<img align="absmiddle" src="' + STATIC_SERVER + '/img/affiliate/flag-' + img + '.png"> ' + content);
		}
	}
	
	var height = 30;
	var links = jQuery('#affiliateBanner ul.level6 > li.open > a');
	for (i = 0; i < links.length; i++) {
		if (i % 2 == 0) {
			var leafs = jQuery(links[i]).parent().find('li.leaf');
			var rows = Math.ceil(leafs.length / 9);
			var element = jQuery(links[i]);
			element.css('height', rows * height + 'px');
			element.css('background-color', '#EBEBEB');
			element.parent().find('ul.level7').css('background-color', '#EBEBEB');
		}
	}
}

/*
 * Scrollt zu einer uebergebenen Position.
 * Anwendung beim Pageing in der Gruppenansicht, LWK und Meine Wetten.
 * Wenn animate == true, dann wird das Scrollen animiert dargestellt.
 */
function scrollToPosition(position, animate){
	if(animate){
		jQuery("html, body").animate({scrollTop:position}, 'slow');
	}else{
		jQuery("html, body").scrollTop(position);
	}
}

function getWindowSize() {
	var w = 0, h = 0;
	 
    if (typeof(window.innerWidth) == 'number') {
        w = window.innerWidth;
        h = window.innerHeight;
    } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
        w = document.documentElement.clientWidth;
        h = document.documentElement.clientHeight;
    } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
        w = document.body.clientWidth;
        h = document.body.clientHeight;
    }
    return [w, h];
}

function relocateCondLayer(id) {
	var offset = jQuery('#' + id).offset();
	jQuery('#' + id + '_layer').css('top', offset.top - 55); 
	jQuery('#' + id + '_layer').css('left', offset.left - 45); 
}

function relocateCondLayer2(id) {
	var offset = jQuery('#' + id).offset();
	jQuery('#' + id + '_layer').css('top', offset.top - 230); 
	jQuery('#' + id + '_layer').css('left', offset.left - 300); 
}

function check_drop(this_obj, what, element){
	if(what == 1){
		this_obj.find(element).slideToggle('fast');
	}
	else{
		this_obj.mouseleave(function() {
			this_obj.find(element).hide();
			});
		}
}
/*
 * Vorraussetzung ist, dass das �u�erste Element der timeline.jsp �bergeben wird. (Hat die Klasse .timeline)
 * Hier wird der aktuelle Status der Filterbuttons gespeichert, damit dieser wiederhergestellt werden kann nach einem update.
 * Dann wird �ber pressEventButton der aktuelle Status getogglt
 */
var buttonStates = {};
var possibleStates = new Array('point','yellow-card','red-card','corner-kick', 'offside');
function saveStateAndPress(obj, classname){
	var divId = jQuery(obj).parent().prop('id');
	var boolMap = buttonStates[divId];
	if (boolMap == undefined){
		boolMap = {};
		for (var i=0;i<possibleStates.length;i++){
			var val = possibleStates[i];
			boolMap[val] = classname!=val ? false : true;
		}
		buttonStates[divId] = boolMap;
	}else
		boolMap[classname] = !boolMap[classname];
	pressEventButton(obj, classname);
}

/*
 * Status der Zeitleiste wiederherstellen. (Eingedr�ckte Filterbuttons)
 */
function restoreTimelineState(divId, thisObj){
	var boolMap = buttonStates[divId];
	if (boolMap!==undefined){
		var obj = jQuery(thisObj).find('.timeline');
		for (var i=0;i<possibleStates.length;i++){
			var val = possibleStates[i];
			if (boolMap[val])
				pressEventButton(obj, val);
		}
	}
}

/*
 * Ein Filterbutton der Zeitleiste wird eingedr�ckt.
 */
function pressEventButton(obj, classname){
	jQuery(obj).find('.' + classname).toggleClass('select');
}

//Toggled eine Zeitleiste unter "Meine Wetten"
function tog_timeline(this_obj, eventId, betId){
	var url = '/conferencetimeline?eventId=' + eventId + '&betId=' + betId + "&betRelated=true";
	var tl = jQuery('#_conferencetimeline_eventId_' + eventId + '_betId_' + betId + "_betRelated_true");
	toggle_timeline(this_obj, url, tl);
}

function togglePref(this_obj, pointType){
	var node=jQuery(this_obj);
	var alt= node.prop('alt');
	node.prop('alt',node.prop('src'));
	node.prop('src',alt);
	node.parent().parent().find('.event_tl').find('.' + pointType).toggle();
}

//Toggled eine Zeitleiste unter "Ergebnisse" des Popups "Ergebnisse und Livescore"
function tog_timeline_results(this_obj, eventId){
	jQuery(this_obj).toggleClass('on');
	var url = '/conferencetimeline?eventId=' + eventId + "&betRelated=true";
	var tl = jQuery(this_obj).parent().parent().find('#_conferencetimeline_eventId_' + eventId + '_betRelated_true');
	toggle_timeline(this_obj, url, tl);
}

//Toggled eine Zeitleiste unter "Ergebnisse" des Popups "Ergebnisse und Livescore"
function tog_timeline_results2(this_obj, eventId){
	var url = '@timeline-' + eventId + '/conferencetimeline?eventId=' + eventId + '&amp;betRelated=true'
	var tl = jQuery('#comp-timeline-' + eventId);
	jQuery(this_obj).toggleClass('on');
	jQuery("#jq-toggle-"  + eventId).toggleClass('hide');
	//When not loaded load timeline, else just toggle visbiliity status
	if (tl.find('.stat_soccer').length == 0){
		toggle_timeline(this_obj, url, tl);
	}
}

function toggle_timeline(this_obj, url, tl){
	if(tl.is(':hidden')){
		RefreshHandler.nav(url);
		jQuery(this_obj).parent().addClass("on");
		tl.show();
	}
	else{
		RefreshHandler.stopRefresh(tl);
		jQuery(this_obj).parent().removeClass("on");
		tl.hide();
	}
}

//�ber die Spielst�nde/Rote Karten in LWK-Gadget/LWK iterieren und die blinken lassen
function blink() {
	jQuery(".blink").each(function() {
		jQuery(this).toggleClass("invisible");
	});
	blinkTimer = window.setTimeout('blink();', 1000);
}
blink();

//Auf/Zuklappt die ausgeblendeten Ausg�nge bei Wettkampf-Sportarten
function toggleFurtherBets(element) {
	var el = jQuery(element);
	el.parent().parent().children('.extra').toggleClass('hide');
	el.children().each(function(){jQuery(this).toggleClass('hide');});
}

function getCompetitionKey(eventId) {
	var key = 'TOP_COMPETITION_OPEN_' + eventId;
	if (window[key] === undefined) {
		window[key] = false;
	}
	return key;
}

//stellt den State bei Top-Competition wieder her (das Bild, Zeilen (>3), unteren Bereich)
function restoreCompetitionFurtherBets(eventId, element) {
	var key = getCompetitionKey(eventId);
	var hideFnc = function(node, hide) {
		if (window[key] || !hide) {
			node.removeClass('hide');
		} else if (hide){
			node.addClass('hide');
		}
	};
	//Das Bild mit Sport-Image
	jQuery(element).find('.sheet_te.extra').each(function(){
		hideFnc(jQuery(this),true);
	});
	jQuery(element).filter('.competition').find('.extra').each(function(i){
		hideFnc(jQuery(this),i >= 3);
	});
	//child-Elemente toggln, falls parent nicht mit dem Style 'extra' versehen ist
	jQuery(element).filter('.headToHead').find('.extra').each(function(i){
		hideFnc(jQuery(this),i >= 3);
	});
	//sich selbst auf die css-Klasse 'extra' pr�fen
	jQuery(element).filter('.yesNo.extra').each(function(){
		hideFnc(jQuery(this),true)
	});
	//Text: wenige Wetten/mehr Wetten toggln (beim JS-Update oder Klicken auf dem Text)
	//#close_eventId wird nur dann gefunden, 
	//wenn die Funktion die Referenz auf Top-Rennen (nicht die Unterbereiche) bekommt
	jQuery(element).find('#close_'+eventId).find('div').each(function() {
		if (window[key]) {
			jQuery(this).toggleClass('hide');
		}
	});
}

//Auf-/Zuklappt die Zeilen bei Top-Competition (das Bild, Zeilen (>3), unteren Bereich)
function toggleCompetitionFurtherBets(eventId, element) {
	var key = getCompetitionKey(eventId);
	window[key] = !window[key];
	//Das Bild mit Sport-Image
	jQuery(element).find('.sheet_te.extra').each(function(){
		jQuery(this).toggleClass('hide');
	});
	jQuery(element).find('.competition').find('.extra').each(function(i){
		if(i >= 3) {
			jQuery(this).toggleClass('hide');
		}
	});
	//child-Elemente toggln, falls parent nicht mit dem Style 'extra' versehen ist
	jQuery(element).find('.headToHead').find('.extra').each(function(i){
		if(i >= 3) {
			jQuery(this).toggleClass('hide');
		}
	});
	//sich selbst auf die css-Klasse 'extra' pr�fen
	jQuery(element).find('.yesNo.extra').each(function(){
		jQuery(this).toggleClass('hide');
	});

	jQuery(element).find('#close_'+eventId).find('div').each(function() {
		jQuery(this).toggleClass('hide');
	});	
}

//Suche: Merkt den Zustand von Ergebnisse und Livescore Bereich und togglt es auch, falls notwendig
function toggleLivescoreSection(restoreState) {
	var isHidden = jQuery('#box_rl .box_rl_cont').is(':hidden');
	var open;
	if (restoreState) { 
		open = isHidden && window.livescoreSectionOpen !== undefined && window.livescoreSectionOpen;
	} else {
		open = isHidden;
	}
	if(open){
		jQuery('#box_rl .box_rl_cont,  #box_rl_pag').show();
		jQuery('#box_rl .hilights_head').removeClass('close');
		window.livescoreSectionOpen = true;
	}else{
		jQuery('#box_rl .box_rl_cont, #box_rl_pag').hide();
		jQuery('#box_rl .hilights_head').addClass('close');
		window.livescoreSectionOpen = false;
	};
}

function alternate(selector, oddStyle, evenStyle, switchClass, withParent) {
	if (!oddStyle) {
		oddStyle = '';
	};
	if (!evenStyle) {
		evenStyle = 'even';
	};
	var odd = true;
	jQuery(selector).each(function() {
		if (switchClass) {
			var e = withParent ? jQuery(this).parent() : jQuery(this);
			if(e.prev().hasClass(switchClass)) {
				odd = true;
			}
		}
		if (odd) {
			jQuery(this).removeClass(evenStyle);
			jQuery(this).addClass(oddStyle);
		} else {
			jQuery(this).removeClass(oddStyle);
			jQuery(this).addClass(evenStyle);
		}
		odd = !odd;
	})
}

//Wird beim Click auf Suche-Suggestion ausgef�hrt
function selSuggestion(text,type) {
	jQuery("#searchField").val(text);
	jQuery("#searchResultType").val(type);
	jQuery("#searchSuggestion").val("true");
	jQuery("#searchForm").submit();
}

function paginationClick(url, scrollToTop) {
	RefreshHandler.nav(url);
	if (scrollToTop) {
		scrollToPosition('0', false);
	}
}

function bindSuggestionBehavior(forContentSite) {
	var searchTab = jQuery("#box_00"); //die ganze Form + Suggestions
	var searchField = jQuery('#searchField'); //das Eingabefeld
	/*verz�gertes Aufrufen von Suchstring*/
	var callSearch = function() {
		var searchString = jQuery("#searchField").val();
		RefreshHandler.nav('@suggestion/search/suggestion?s='+encodeURIComponent(searchString)+'&cs='+forContentSite);
	}
	var searchTimeout;
	searchField.keyup(
		function(event) {
	        if (searchTimeout !== undefined){
	        	clearTimeout(searchTimeout);
	        }
	        searchTimeout = setTimeout(callSearch, 200);
		}
	);
	var mouseOverSuggestion = false;
	//Suggestion-Box verschwinden lassen, 
	//nur wenn man den gesamten Bereich (li id="box_00") verl�sst
	searchTab.focusout(function (e) {
		if(!mouseOverSuggestion) {
			jQuery('#box_00 .box_00').hide();
			searchTab.removeClass('on_3');
		} else {
			//focus zur�ck setzen, damit focusout wieder funktioniert
			//setTimeout - hack, funktioniert in firefox ansonsten nicht
			setTimeout(function() {searchField.focus();}, 100);
		}
	});
	searchTab.mouseover(function() {
		mouseOverSuggestion = true;
		jQuery(this).addClass('on_3');
	});
	searchTab.mouseout(function (e) {
		if(jQuery.contains(this, e.relatedTarget) == false) {
			mouseOverSuggestion = false;
			//nur den Style an Suchfeld �ndern, wenn Suggestions 
			//nicht angezeigt werden. zB beim Ziehen von Maus nach oben
			if (!jQuery('#box_00 .box_00').is(':visible')) {
				searchTab.removeClass('on_3');
			}
		}
	});
}


function toggleSmallConferenceSection(restoreState) {
	var isOpen = !jQuery('#box_lwk').is(':hidden');
	var open;
	if (restoreState) { 
		open = window.conferenceGroupSelectionOpen === undefined || window.conferenceGroupSelectionOpen;
	} else {
		open = !isOpen;
	}
	if(open){
		jQuery('#box_lwk').show();
		jQuery('#box_lwk_foot').show();
		jQuery('#box_lwk_closed_header').hide();
		window.conferenceGroupSelectionOpen = true;
	}else{
		jQuery('#box_lwk').hide();
		jQuery('#box_lwk_foot').hide();
		jQuery('#box_lwk_closed_header').show();
		window.conferenceGroupSelectionOpen = false;
	};
}

/**
 * Funktion zur Steuerung von LWK-Gadget Gruppenauswahl (Anzeige von Events f�r ausgew�hlte Gruppen) 
 * @param init - ob es der allererste Request ist oder nur Nav-Aufruf (Gruppen wurden hinzugef�gt/entfernt)
 * @param selectedGroupIds - in Gruppen-Navigation selektierte Gruppen-Ids
 * */
function smallConferenceGroupSelection(init, selectedGroupIds) {
	if (init) {
		//RefreshHandler.nav() soll nicht aufgerufen werden, wenn direkt nachdem das Gadget geladen wurde
		window.firstSmallConferenceGroupSelection = true;
		//setTimeout - ist eine L�sung daf�r, damit RefreshHandler.init()/nav() erst dann aufgerufen werden
		//nachdem das per Ajax gelieferte Dokument tats�chlich in DOM-Baum hinzugef�gt wurde. 
		setTimeout(function() {
			RefreshHandler.init(32);
		}, 0);
	} else {
		if (window.firstSmallConferenceGroupSelection) {
			window.firstSmallConferenceGroupSelection = false;
		} else {
			var url = '@smallconferencegroup/program/smallconferencegroupselection?selectedGroupIds='+selectedGroupIds;
			setTimeout(function() {
				RefreshHandler.nav(url);
			},0);
		}
	}
}

function viewResults(sportId, timefilter){
	//Alle offenen Zeitleisten auf hide setzen damit sie beim wechsel der Results Standardm��ig zu sind.
	var timeline = jQuery(".timeline");
	timeline.hide();
	timeline.parent().hide()
	//Results wechseln
	RefreshHandler.nav("@content" + "/results/" + sportId + "/" + timefilter);
	rebuildScorePopupNavigation(sportId);
}

function viewLivescore(){
	RefreshHandler.nav("@content/livescore");
	rebuildScorePopupNavigation("AllLive");
}

function viewMyLeagues(){
	RefreshHandler.nav("@content/myleagues");
	rebuildScorePopupNavigation("MyLeagues");
}

function viewMyEvents(){
	RefreshHandler.nav("@content/myevents");
	rebuildScorePopupNavigation("MyEvents");
}

//Casino2-Spezifische JS Funktionen
var casinoLobby;
function setCasinoLobby(url) {
	casinoLobby = url;
}

function changeCasinoFilter(this_obj, selectedItem, searchString, gameRules){
	if (selectedItem == undefined)
		selectedItem = jQuery(this_obj).children(':selected').prop('value');
	var url = "";
	if (gameRules != undefined){
		url = "@filteredGames/casino2/allGameRules?chosenFilter=" + selectedItem;
	}else{
		url = "@filteredGames/casino2/allGames?chosenFilter=" + selectedItem;
	}
	if (searchString == undefined)
		RefreshHandler.nav(url);
	else
		RefreshHandler.nav(url + "&searchString=" +  encodeURI(searchString));
}

function changeTournamentFilter(day){
	if (day != undefined)
		RefreshHandler.nav("@tournamentTable/casino2/tournamentTable?chosenDay=" + day);
}

var casinoBonusTimer;
var amountDisplayGlobal;
/**
var casinoRefreshTimer;
var shouldRefresh = false;
**/

function hideBox(id) {
	var box = jQuery('#' + id);
	if(box != undefined)
		box.hide();
}

function showBox(id) {
	var box = jQuery('#' + id);
	if(box != undefined)
		box.show();
}

function updateCasinoBonus(){
	window.setTimeout(function() {
		var disabled = jQuery('#amountDisplayCasino').prop('disabled');
		if(disabled)
			return;
		window.clearTimeout(casinoBonusTimer);
		hideBox('messageBox');
		showBox('messageBoxSpacer');
		var amountDisplay = document.getElementById("amountDisplayCasino").value;
		if(amountDisplay != '' && matchAmount(amountDisplay)) {
			if(amountDisplay != amountDisplayGlobal) {
				casinoBonusTimer = window.setTimeout(function() {
					RefreshHandler.nav("@bonus/casino2/bonus?amountDisplay=" + amountDisplay);
					amountDisplayGlobal = amountDisplay;
				}, 500);
			}
		} else {
			hideBox('bonusBox');
		}
	}, 0);
}

function changePlayForFun(playForFun, selectionId, popup){
	if (playForFun != undefined && selectionId != undefined){
		var paramString = playForFun ? "FUN" : "MONEY";
		RefreshHandler.nav("@game/casino2/game?selectionId=" + selectionId + '&wantsToPlayFor=' + paramString + '&popup=' + popup);
	}
}

function toggleCasinoCookie(this_obj) {
	var node = jQuery(this_obj);
	var cookie = getCasinoCookie();
	if (cookie != undefined && cookie == "on") {
		node.removeClass('check_on');
		node.addClass('check');
		jQuery.cookie("casinoPref", "off");
	}else{
		node.removeClass('check');
		node.addClass('check_on');
		jQuery.cookie("casinoPref", "on", {expires: 365});
	}	
}

function getCasinoCookie() {
	var cookie = jQuery.cookie("casinoPref");
	return cookie;
}

function checkCasinoCookie(){
	var node = jQuery(document.getElementById('casino_pref'));
	var cookie = getCasinoCookie();
	if (cookie != undefined && cookie == "on") {
		node.removeClass('check');
		node.addClass('check_on');
	}
	else{
		node.removeClass('check_on');
		node.addClass('check');
	}
}

function gamePopupOrMain(link, popupLink){
	var cookie = getCasinoCookie();
	if (cookie != undefined && cookie == "on") {
		popupNoScrollbars(popupLink, 'CasinoGame', 830, 720);
	}
	else{
		window.location.href = link;
	}
}

var isGameIdle = true;
var isGameLoaded = true;

function loadFlash(playUrl, vars, lang, fullscreen, baseVar, loadUrl, src, width, height, version, wmode, showEI){
	var fn = undefined;
	if(swfobject.hasFlashPlayerVersion(version)) {
		fn = function() {
			var flashvars = {};
			var params = {
					allowscriptaccess: "always",
					quality: "high",
					loop: "false",
					bgcolor: "#000000",
					flashvars: "server=" + playUrl + "/&" + vars + "&lang=" + lang + fullscreen,
					base: baseVar,
					scale: "exactfit",
					wmode: wmode
			};
			var attributes = {
					id: "gameContent",
					name: "gameContent"
			};
			swfobject.embedSWF(loadUrl + src, "gameContent", 
					width, height, version, null, flashvars, params, attributes);
		}
	} else if (showEI) {
		fn = function() {
			var attributes = {
					data: "/js/expressInstall.swf",
					width: width,
					height: height
			};
            var params = {
            		menu: false
            };
            swfobject.showExpressInstall(attributes, params, "gameContent");
		}
	}
	if(fn != undefined)
		swfobject.addDomLoadEvent(fn);
}

function loadLayer(url, popup, game) {
	url += '?width=' + getWindowSize()[0];
	if(popup != undefined) {
		if(popup)
			url += '&height=' + getWindowSize()[1] + '&popup=true';
		else
			url += '&popup=false';
	} 
	if(game != undefined && game != '')
		url += '&selectionId=' + game;
	RefreshHandler.nav("@layer" + url);
}

function changeCashierAmount(amount) {
	var disabled = jQuery('#amountDisplayCasino').prop('disabled');
	if(disabled)
		return;
	document.getElementById('amountDisplayCasino').value = amount;
	updateCasinoBonus();
}

function forfeitBonusQuestion (p1, p2, p3, p4){
	if (jQuery('#form\\:redeemBonus').val()=='false') {
		showConfLayer('/img/warn_grey.png', 
			'66px', '56px', 
			p1, 
			p2,
			'flex_button_new_grey_bg inline',
			'flex_button_new_grey',
			p3,
			'flex_button_new_green_arr_bg margin_l_12 inline',
			'flex_button_new_green',
			p4)
			.then(
				function(val){
					if(val){
						showDelayLayer();
						jQuery('#form\\:payinAction').click();
					} return val;
			});
	} else {
		showDelayLayer();
		jQuery('#form\\:payinAction').click();
	}
}

function executePayment(payParam, width, height, game, popup){
	var displayAmount = document.getElementById("amountDisplayCasino").value;
	if(displayAmount == '')
		return;
	jQuery('#button_buy').prop("onclick", null).attr("onclick", null).unbind("click").click(function() {return false;});
	jQuery('#button_sell').prop("onclick", null).attr("onclick", null).unbind("click").click(function() {return false;});
	jQuery('#refresh_buy').prop("onclick", null).attr("onclick", null).unbind("click").click(function() {return false;});
	jQuery('#refresh_sell').prop("onclick", null).attr("onclick", null).unbind("click").click(function() {return false;});
	jQuery('#sell').addClass("lightgrey");
	jQuery('#buy').addClass("lightgrey");
	document.getElementById("amountDisplayCasino").value = '';
	jQuery('#amountDisplayCasino').prop('disabled', true);
	var acceptBonus = document.getElementById("acceptBonus");
	if(acceptBonus != undefined)
		acceptBonus = acceptBonus.checked;
	else 
		acceptBonus = false;
	/**
	shouldRefresh = true;
	window.clearTimeout(casinoRefreshTimer);
	casinoRefreshTimer = window.setTimeout(function() {
		if(shouldRefresh)
			RefreshHandler.nav('@layer/casino2/cashierLayer');
	}, 15000);
	**/
	amountDisplayGlobal = '';
	if (!matchAmount(displayAmount)){
		displayAmount='';
	}else{
		displayAmount = displayAmount.replace(/\s/g, '');
	}
	var url = "@layer/casino2/cashierLayer?executePayment=" + payParam + "&amountDisplay=" + displayAmount + "&acceptBonus=" + acceptBonus;
	if (width != undefined && width !='')
		url = url + "&width=" + width;
	if (height != undefined && height !='')
		url = url +  "&height=" + height;
	if(game != undefined && game != '')
		url += "&selectionId=" + game;
	if(popup != undefined)
		url += "&popup=" + popup;
	RefreshHandler.nav(url);
}

function matchAmount(amountString){
	return amountString.match(/^([0-9]+((,|\.)[0-9]+)?)$/g);
}

var isFav;
function toggleFavorite(firstFav, onImg, offImg, gameId){
	if (isFav == undefined)
		isFav = !firstFav;
	else
		isFav = !isFav;
	
	var imgNode = jQuery(document.getElementById("favImg"));
	
	if (isFav){
		imgNode.prop("src", onImg);
		RefreshHandler.nav("@fav/casino2/fav?ctr=ADD&gameId=" + gameId);
	}else{
		imgNode.prop("src", offImg);
		RefreshHandler.nav("@fav/casino2/fav?ctr=DELETE&gameId=" + gameId);
	}
}

function getCurrentUrlWithParam(param){
	var loc = document.location.pathname + document.location.search;
	if (loc.indexOf("?") == -1){
		loc = loc + "?" + param;
	}
	else{
		loc = loc + "&" + param;
	}
	return loc
}

var rulesWindow;
function openRules(url) {
	var windowArgs = 'dependent=no, width=600, height=500, hotkeys=no, menubar=no, resizable=yes, scrollbars=yes, status=no, toolbar=no';
	if(!rulesWindow||rulesWindow.closed) {
		rulesWindow = window.open(url, 'Rules', windowArgs);
	}
	else {						
		rulesWindow = window.open(url, 'Rules', windowArgs);
		rulesWindow.focus();
	}
}

function reloadBalance() {
	var replacement = getCasinoReplacement();
	if (replacement == undefined
			|| (replacement != undefined
					&& replacement['div'] != undefined
					&& replacement['div'].css('display') != 'block')) {
						window.setTimeout(function() {
							var flashContent = getCasinoFlashObject()['object'];
							if (flashContent != undefined && flashContent.reloadbalance) {
								flashContent.reloadbalance();
							}
						}, 1000); 		
	}
}

function getCasinoFlashObject() {
	var flashContainer = jQuery('#gameContent');
	var flashObject;
	if (document.all)
		flashObject = document.all("gameContent");
	else
		flashObject =  document.getElementById("gameContent");
	return {
		'container': flashContainer,
		'object': flashObject
	}	
}

function getCasinoReplacement() {
	var img = jQuery('#gameImageReplacement');
	var div = jQuery('#gameImageContainer');
	return {
		'img': img,
		'div': div
	}
}

function closeCasinoGame() {
	var casinoPref = document.getElementById('casino_pref');
	if (casinoPref != undefined){
		window.location.href = casinoLobby;
	}
	else{
		window.close();
	}
}

var isFullscreen = false;

var width  = 780;	
var height = 560;	
var popWidth = 830;
var popHeight = 720;

var left = 0;	
var top = 0;	

function toggleFullscreen(fullscreen, popup) {
	if (isFullscreen) {
		setCasinoFlashContentSize(width, height);
		
	    self.moveTo(left, top);
	    self.resizeTo(popWidth, popHeight);

		document.getElementById("windowmode").innerHTML = fullscreen;
		isFullscreen = false;
	}
	else {
		setCasinoFlashContentSize(getWindowSize()[0], screen.height - 190);
		
		self.moveTo(0,0);
		self.resizeTo(screen.width, screen.height);

		document.getElementById("windowmode").innerHTML = popup;
		isFullscreen = true;
	}
}	

function setCasinoFlashContentSize(width, height) {
	var flashContent = getCasinoFlashObject()['object'];
	if (flashContent != undefined) {
		flashContent.width = width;
		flashContent.height = height;	
	}
}

function rules(url) {
    var features="directories=no,location=no,menubar=no,resizable=no,scrollbars=yes,status=no,toolbar=no,width=600,height=500";
    var remote = open(url, "rules", features);
    remote.focus();
}

var layerQueue = new Queue();
var hiding = false;

function gameRoundStarted() {
	isGameIdle = false;
}

function gameRoundEnded() {
	isGameIdle = true;
	if (!isGameLoaded)
		isGameLoaded = true;
	jQuery('#gameNotIdle').fadeOut('slow');
}

function initFlash11() {
	isGameIdle = false;
	isGameLoaded = false;
	height = 585;
}

function displayFlashScreenshot(img) {
	window.setTimeout(function() {
		var replacement = getCasinoReplacement();
		var flash = getCasinoFlashObject()['container'];
		if (flash != undefined
				&& replacement['div'] != undefined
				&& replacement['div'].css('display') != 'block'
				&& replacement['img'] != undefined) {
			flash.css('width', '0px');
			flash.css('height', '0px');
			replacement['img'].prop('src', 'data:image/jpeg;base64,' + img);
			replacement['div'].css('display', 'block');
			dequeueLayers();
		}
	}, 0); 	
}

function hideCasinoGame(slf, override) {
	if (hiding)
		layerQueue.enqueue(slf);
	else {
		hiding = true;
		
		var show = function(or) {
			if (isGameIdle || or)
				takeScreenshot();
			else
				window.setTimeout(function() {
					show();
				}, 100);
		}
		
		var hide = function() {
				window.setTimeout(function() {
					if (!isGameLoaded) {
						jQuery('#gameNotIdle').show();
						window.setTimeout(function() {
							hide();
						}, 1000);
					} else {
						layerQueue.enqueue(slf);
						var or = override != undefined && override;
						var flash = getCasinoFlashObject()['object'];
						var replacement = getCasinoReplacement()['div'];
						if (flash != undefined
								&& flash.stopAutoplay
								&& flash.captureScreenshot
								&& replacement != undefined
								&& replacement.css('display') != 'block') {
							flash.stopAutoplay();
							// laufende Spiele sollen nicht unterbrochen werden
							// Warnung anzeigen und abbrechen
							if (!isGameIdle && !or) 
								jQuery('#gameNotIdle').show();
							 else 
								jQuery('#gameNotIdle').fadeOut('slow');
							show(or);
						} else 
							dequeueLayers();
					}
				}, 0); 	
		};
		
		hide();
	}
}

function dequeueLayers() {
	hiding = false;
	while (true) {
		var f = layerQueue.dequeue();
		if (f == undefined){
			return;
		}
		f();
	}
}

function takeScreenshot() {
	var flash = getCasinoFlashObject()['object'];
	if (flash != undefined
			&& flash.captureScreenshot) {
		var scale = 1;
		var quality = 100;
		if (IE_vers != undefined && IE_vers <= 8) {
			scale = 0.5;
			quality = 22;
		}
		flash.captureScreenshot(scale, 0, quality, false);
	}
}

function showCasinoGame() {
	hiding = false;
	window.setTimeout(function() {
		var replacement = getCasinoReplacement()['div'];
		var flash = getCasinoFlashObject()['container'];
		if (replacement != undefined
				&& replacement != undefined
				&& replacement.css('display') == 'block'
				&& flash != undefined) {
			replacement.css('display', 'none');
			flash.css('width', width + 'px');
			flash.css('height', height + 'px');
			reloadBalance();
		}
	}, 0); 	
}

var refreshed = false;
function refreshFromCasinoPopup() {
	if(!refreshed) {
		opener.location.reload();
		refreshed = true;
	}
}

function confirmPaymentAction(question, ok, cancel) {
	document.getElementById('alert_layer_inner_casino').innerHTML = question;
	jQuery('#alert_layer_bg_casino').show();
	jQuery('#alert_layer_casino').show();
	jQuery('#alert_layer_casino_button_ok').prop("onclick", null).attr("onclick", null).unbind("click").click(function() {return false;});
	jQuery('#alert_layer_casino_button_ok').click(ok);
	jQuery('#alert_layer_casino_button_cancel').prop("onclick", null).attr("onclick", null).unbind("click").click(function() {return false;});
	jQuery('#alert_layer_casino_button_cancel').click(cancel);
}

function closePaymentActionLayer() {
	jQuery('#alert_layer_bg_casino').hide();
	jQuery('#alert_layer_casino').hide();
}

//Casino2 End
jQuery(function() {
	var pp = jQuery("#password_placeholder");
	var p = jQuery("#password");
	var l = jQuery("#login");

	pp.click(function() {
	    p.focus();
	});
	p.focus(function() {
		pp.hide();
	});
	function update_pp() {
	    if (p.prop("value")) {
	    	pp.hide();
	    } else {
	        pp.show();
	    }
	}
	p.blur(update_pp);
	l.blur(update_pp);
	l.click(update_pp);
	l.focus(update_pp);
	l.change(update_pp);
});

jQuery(function() {
	var pp = jQuery("#password_placeholder_2");
	var p = jQuery("#password_2");
	var l = jQuery("#casinoLogin");
	
	pp.click(function() {
		p.focus();
	});
	p.focus(function() {
		pp.hide();
	});
	function update_pp() {
		if (p.prop("value")) {
			pp.hide();
		} else {
			pp.show();
		}
	}
	p.blur(update_pp);
	l.blur(update_pp);
	l.click(update_pp);
	l.focus(update_pp);
	l.change(update_pp);
});

/****************************
 * Suchfeldverhalten Anfang
 ****************************/
function bindSearchField(mapping){
	jQuery('#searchField').keypress(function(e){
		if(e.which == 13){
			e.preventDefault();
			viewSearchResults(mapping);
		}
	});
	
	jQuery('#searchButton').click(function(e){
		viewSearchResults(mapping);
	});
}

function viewSearchResults(mapping,searchQuery){
	showDelayLayer();
	if(searchQuery != undefined)
		jQuery('#searchField').val(searchQuery);
	RefreshHandler.nav(mapping+"?s="+encodeURIComponent(jQuery('#searchField').val()));
	if(mapping.indexOf('searchResultsLivescore') != -1)
		rebuildScorePopupNavigation("Search");
}

/****************************
 * Suchfeldverhalten Ende
 ****************************/
function updateSearchFilter(element, type, newStatus){
	jQuery(element).toggleClass('on');
	var query = "?type="+encodeURIComponent(type);
	query += "&newStatus="+encodeURIComponent(newStatus);
	RefreshHandler.nav("/search2/updateFilter/"+query);
}

function isFunctionDefined(functionName){
	return eval("typeof " + functionName + " == 'function'");
}


/****************************
 * Confirmation Layer Anfang
 ****************************/
function showConfLayer( imageSrc, imageWidth, imageHeight,layerTitle, layerTxt,
		cancelBtnClass, cancelBtnLinkClass, cancelBTnTxt,
		confBtnClass,confBtnLinkClass,confBtnTxt) {
	var deferred = jQuery.Deferred();
	
	jQuery('#c_LayerImage').attr({
			src: imageSrc,
				width: imageWidth,
					height: imageHeight
	});	

	jQuery('#c_LayerTitle').text(layerTitle);
	jQuery('#c_LayerText').text(layerTxt);
	
	jQuery('#c_CancelBtn').addClass(cancelBtnClass).click(function() {
		deferred.resolve(false);
	});
	jQuery('#c_CancelBtn a').addClass(cancelBtnLinkClass);
	jQuery('#c_CancelBtn a span').text(cancelBTnTxt);
	
	jQuery('#c_ConfBtn').addClass(confBtnClass).click(function() {
		deferred.resolve(true);
	});
	jQuery('#c_ConfBtn a').addClass(confBtnLinkClass);
	jQuery('#c_ConfBtn a span').text(confBtnTxt);
	
	var layer = jQuery("#c_Layer").show();
	show_layer('dim');
	return deferred.promise().then(function(val) {
		layer.hide();
		hide_layer('dim');
		return val;
	});
}

/****************************
 * Confirmation Layer Ende
 ****************************/

/****************************
 * Information Layer Anfang
 ****************************/
function showInfLayer( imageSrc, imageWidth, imageHeight,layerTitle, layerTxt,
		btnClass, btnLinkClass, btnTxt) {
	
	jQuery('#c_LayerImage').attr({
			src: imageSrc,
				width: imageWidth,
					height: imageHeight
	});	

	jQuery('#i_LayerTitle').text(layerTitle);
	jQuery('#i_LayerText').text(layerTxt);
	
	jQuery('#i_Btn').addClass(btnClass).click(function() {
		deferred.resolve(false);
	});
	jQuery('#i_Btn a').addClass(btnLinkClass);
	jQuery('#i_Btn a span').text(btnTxt);

	var layer = jQuery("#i_Layer").show();
	show_layer('dim');
	return deferred.promise().then(function(val) {
		layer.hide();
		hide_layer('dim');
		return val;
	});
}
/****************************
 * Information Layer Ende
 ****************************/

/****************************
 * Alert Layer Anfang
 ****************************/
function showNewAlertLayer( layerTitle, layerTxt, btnLink, btnTxt ) {

	jQuery('#newAlertLayerTitle').text(layerTitle);
	jQuery('#newAlertLayerText').html(layerTxt);	
	jQuery('#newAlertLayerBtn a').attr("href", btnLink);
	jQuery('#newAlertLayerBtn a span').text(btnTxt);
	
	jQuery('#layer_bg').show();
	jQuery("#newAlertLayer").show();
}
/****************************
 * Alert Layer Ende
 ****************************/

/****************************
 * Navigation Anfang
 ****************************/

/**
 * 
 */
function showWaitCursor(){
	jQuery('body').addClass('wait');
}

function hideWaitCursor(){
	jQuery('body').removeClass('wait');
}

/**
 * Liefert den passenden Knoten
 * @param type - Typ des Blocks (siehe dazu NavType.java)
 * @param groupId - Gruppen ID
 */
function getNavBlockNode(type, groupId){
	return jQuery('#jq-navBlock-'+type+'-'+groupId);
}

/**
 * Liefert das Komponenten-Element
 * @param type - Typ des Blocks (siehe dazu NavType.java)
 * @param groupId - Gruppen ID
 */
function getNavBlockComp(type, groupId){
	return jQuery('#comp-'+type+'_'+groupId);
}

/**
 * Funktion f�r die 1. und 2. Gruppenebene
 * <br />
 * Navigiert zur entsprechenden Seite oder f�hrt eine RefreshHandler-Request aus
 * @param e - Mausevent
 * @param type - Seitentyp (start,sports)
 * @param groupId - Gruppen ID
 * @param navType - Typ des Blocks (siehe dazu NavType.java)
 * @param url - Url der Komponente
 * @param level - Gruppenlevel
 * @param selectedSubGroupCount - Anzahl der selektierten Untergruppen
 * @param link - SEO-Url
 */
function toggleOrLoad(event, type, groupId, navType, url, level, selectedSubGroupCount, link){
	// Linkausf�hrung verhindern
	preventDefault(event);
	// Falls auf ein Bild geklickt wurde, muss der Aufruf durchgereicht werden
	// Siehe toggleAllAndUpdate(..)
	
	// IE Fix
	var target = event.target || window.event.srcElement;
	if(target.nodeName == "IMG")
		return;
	
	var node = getNavBlockNode(navType, groupId);
	var subnode = getNavBlockComp(navType, groupId);
	
	// Falls bereits Untergruppen selektiert wurden zur Auswahlseite wechseln
	if (type == "start" && level == 2 && (selectedSubGroupCount > 0 || eBetFeatures['springSportbetAdaptations'] == 1)){
		window.location.href=link;
	}else{
		// Ansonsten:
		// - Togglestatus aktualisieren
		// - Knoten markieren
		node.toggleClass('on');
		var subnode = getNavBlockComp(navType, groupId);
		var callback;
		// - Untergruppenliste ggf. laden
		if (subnode.contents().length == 0){
			callback = function(){RefreshHandler.nav(url);};
		}else{
			if(!subnode.is(':visible')){
				// Sicherstellen, dass die Unterknoten noch den richtigen Status haben
				subnode.find('ul:first').show();
				subnode.find('div[id^="jq-further"]').show();
			} else {
				node.parent().attr("itemprop","").attr("itemtype","");
			}
			subnode.slideToggle();
		}
		
		if(type == "sports")
			RefreshHandler.nav('/sports/selection/toggle?groupId=' + groupId + '&type='+navType, null, callback);
		else if(callback)
			callback();
	}
}

function showNavGroupList(element,slide, type, toggleStatus){
	var jqElement = jQuery(element);
	if(type == "sports" && !toggleStatus){
		jqElement.closest('.e_active').hide();
		return;
	}
	if(slide)
		jqElement.slideToggle();
	else{
		jqElement.prev('a').addClass('on');
		jqElement.show();
	}
}

function toggleGroupOptions(element,numClass,show){
	var jElement = jQuery(element);
	
	if(show)
		jElement.removeClass('num_'+numClass);
	else
		jElement.addClass('num_'+numClass);
	jElement.find('span.num_r').eq(show?1:0).show();
	jElement.find('span.num_r').eq(show?0:1).hide();
}

/**
 * Funktion f�r die Schaltfl�chen beim Mouseover
 * @param event - MouseEvent
 * @param groupId - Gruppen ID
 * @param linkType - Typ des Blocks (siehe dazu NavType.java)
 * @param action - Auszuf�hrende Aktion (addAll/removeAll)
 * @param type - Startseite oder Gruppenauswahl
 */
function toggleAllAndUpdate(event, groupId, linkType, action, type){
	preventDefault(event);
	showWaitCursor();
	var node = getNavBlockNode(linkType, groupId);
	var url = encodeURI('/sports/selection/'+action+'?groupId='+groupId+'&type='+linkType);
	if(type == "start") {
		RefreshHandler.nav(url, null, function(){
			window.location.href = node.attr("href");
		});
		return;
	}
	// Toggeln ausf�hren und Gruppenbaum aktualisieren
	updateNav(url, node);
}

/**
 * Funktion f�r die dritte Ebene
 * @param event - Mausevent
 * @param groupId - Gruppen ID
 * @param type - Typ des Blocks (siehe dazu NavType.java)
 */
function toggleAndUpdate(event, groupId, type){
	showWaitCursor();
	// Linkausf�hrung verhindern
	preventDefault(event);
	// Element bestimmen
	var node = getNavBlockNode(type, groupId);
	// Optimistisch Toggeln
	node.toggleClass('on');
	var url = encodeURI('/sports/selection/toggle?groupId='+groupId+'&type='+type);
	// Toggeln ausf�hren und Gruppenbaum aktualisieren
	updateNav(url, node.closest('.e_active'));
}

/**
 * Setzt den Zeitfilter und aktualisiert den Gruppenbaum
 * @param periodIndex
 */
function setTimePeriod(periodIndex, url){
	showWaitCursor();
	var url = encodeURI(url + '?periodIndex='+periodIndex);
	var node = jQuery('#comp-leftSide');
	updateNav(url, node, true);
}

/**
 * F�hrt einen RefreshHandler-Request f�r die �bergebene url durch.
 * <br />
 * Ermittelt alle betroffenen Elemente im Navigationsbaum und aktualisiert diese ebenfalls.
 * @param url
 * @param node
 */
function updateNav(url, node, onlyChildElements){
	var updateUrls = new Array();
	if(node.attr('e:url'))
		updateUrls[0] = node.attr('e:url');
	
	var updateElements = jQuery();
	// Navigationsbaum nach oben gehen
	if(!onlyChildElements)
		updateElements = node.parents('.e_active');
	// Navigationsbaum nach unten gehen
	var childElements = node.find('.e_active');
	childElements.each(function(index, value){
		if(jQuery(value).contents().length > 0)
			updateElements = updateElements.add(jQuery(value));
	});
	// Parallele Elemente finden
	var nextElement = node.next('.e_active');
	if(nextElement.contents().length > 0)
		updateElements = updateElements.add(nextElement);
	
	updateElements.each(function(index, value){
		updateUrls[updateUrls.length] = jQuery(value).attr('e:url').replace('&slide=true','');
	});	
	
	// Update des Gruppenbaums erst im Callback, da bei gleichzeitiger Ausf�hrung nicht
	// sichergestellt ist, dass die Selection bereits manipuliert wurde.
	RefreshHandler.nav(url, null, function(){
		RefreshHandler.completeOrUpdate(RefreshHandler.urlComplete, updateUrls);
		hideWaitCursor();
	});
}

function toggleFurther(groupId){
	var nodeSelector = '#jq-further-'+groupId;
	var node = jQuery(nodeSelector);
	jQuery(node).prev('ul').slideToggle();
	jQuery(node).find('span').toggle();

	RefreshHandler.nav('sports/selection/toggle?groupId='+groupId+'&type=FURTHER');
}

function redirectToStart(delay){
	window.setTimeout(function(){
		window.location.href = "/";
	},delay);
}

function preventDefault(eventObj){
	eventObj = eventObj || window.event;
	
	if (eventObj.preventDefault) {
		// W3C-DOM-Standard
		eventObj.preventDefault();
	} else {
		// Andernfalls setze returnValue
		// Microsoft-Alternative f�r Internet Explorer < 9
		eventObj.returnValue = false;
	}
}

/****************************
 * Navigation Ende
 ****************************/

/****************************
 * Wettartenfilter Anfang
 ****************************/

function selectResultTypeId(element, groupId, programType, resultTypeId){
	if(jQuery(element).hasClass('on'))
		return;
	jQuery(element).closest('#resultTypeFilter_'+groupId+'_'+programType).find('div.on').removeClass('on');
	jQuery(element).addClass('on');
	RefreshHandler.nav('/sports/selection/setResultTypeId?groupId='+groupId+'&programType='+programType+'&resultTypeId='+resultTypeId);
}

/****************************
 * Wettartenfilter Ende
 ****************************/
 
 // Funktion, um im Html �ber den Browser einen Breakpoint setzen zu k�nnen.
 // Dazu kann das Javascript im Browser z.B. um "breakpoint_hook();" erg�nzt werden und hier
 // der Breakpoint gesetzt werden.
 function breakpoint_hook(args) {
 	alert("breakpoint_hook");
 }
 
 /****************************
  * Mini Game laden
  ****************************/
 function loadMinigame(gameId){
	 if (gameId == undefined || gameId == "")
		 return;
	 RefreshHandler.nav('@minigame/casino2/miniGame?popup=false&selectionId=' + gameId);
 }
 
 /****************************
  * Register Titan Popup
  ****************************/
 var initTitan = true;
 function initTitanSlider(){
	 if (initTitan){
		 initTitan = false;
		 window.unoSlider = jQuery('#modal-odd').unoSlider({
			    bullets: true,
			    next: '.kahn-next',
			    prev: '.kahn-back',
			    auto: false,
			    easing: 'linear'
			  });
	 }
 }
 /****************************
  * Results Archiv Datepicker
  ****************************/
function initArchiveDatePicker(start, end, locale, accesableYearsAndWeeks) {
	var accesable = jQuery.parseJSON(accesableYearsAndWeeks);
    var startDate = new Date(start);
    var endDate = new Date(end);
    Date.prototype.getWeek = function(){
    	return jQuery.datepicker.iso8601Week(this);
    }
    
    var selectCurrentWeek = function() {
        window.setTimeout(function () {
            jQuery('.week-picker').find('.ui-datepicker-current-day a').addClass('ui-state-active');
        }, 1);
    }
    jQuery('.week-picker').datepicker( {
    	showWeek: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        maxDate: -1,
        yearRange: "-7:+0",
        defaultDate: startDate,
        onSelect: function(dateText, inst) {
            jQuery('#cal_archive_layer').hide();
            var date = jQuery(this).datepicker('getDate');
            var path = window.location.pathname;
            var splitUri = path.split('/');
            //path starts and ends with "/" thus array begins and ends with empty string
            splitUri[splitUri.length-2] = "kw" + date.getWeek();
            splitUri[splitUri.length-3] = date.getFullYear();
            window.location.href = window.location.origin + splitUri.join('/');
        },
        beforeShowDay: function(date) {
            var cssClass = '';
            if(date >= startDate && date <= endDate)
                cssClass = 'ui-datepicker-current-day';
            var selectable;
            if (accesable=="" || accesable ==undefined){
            	selectable=false
            }else{
            	var yearList = accesable[date.getFullYear()];
            	selectable = yearList != undefined && yearList.indexOf(date.getWeek()) != -1; 
            }
            return [selectable, cssClass];
        },
        onChangeMonthYear: function(year, month, inst) {
            selectCurrentWeek();
        }
    });
    jQuery(this).datepicker('setDate', startDate);
    jQuery.datepicker.setDefaults(jQuery.datepicker.regional[locale]);
    selectCurrentWeek();
    //Hack um locale im Datepicker zu aktivieren, sonst w�re schon vorhandene Kalenderwoche nciht lokalisiert
    jQuery('.ui-datepicker-prev.ui-corner-all').click();
    jQuery('.ui-datepicker-next.ui-corner-all').click();
}

var lockUnselect = false;

function selectRow(eventId, featureVal) {
	if (!featureVal || lockUnselect)
		return;
	jQuery('#jq-event-id-' + eventId).addClass('selected');
}

function unselectRow(eventId, featureVal) {
	if (featureVal && !lockUnselect)
		jQuery('#jq-event-id-' + eventId).removeClass('selected');
}

function scrollToEventRow(eventId, featureVal) {
	if (!featureVal)
		return;
	lockUnselect = true;
	jQuery(window).scrollTop(jQuery('#jq-event-id-' + eventId).offset().top);
	selectRow(eventId, featureVal);
	setTimeout(function(){
		lockUnselect = false;
		unselectRow(eventId, featureVal);
	},5000);
}
