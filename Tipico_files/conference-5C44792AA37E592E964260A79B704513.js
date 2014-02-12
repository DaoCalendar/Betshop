(function() {

	/*BEGIN: Cookie Buttons*/

    //überprüfen von "window.flashAvailable == true;" wie in SoundButton.java funktioniert hier nicht
	//weil flashAvailable noch undefined ist
	//workaround aus: http://stackoverflow.com/questions/5717062/how-to-detect-flash-using-swfobject
	function flashAv() {
		return swfobject.hasFlashPlayerVersion("1");
	}
	
	// Management von StatButton und SoundButton: Cookie und Aussehen
	function CookieButton(cookieName, component, enabled) {
		this.cookieName = cookieName;
		this.component = component;
		this.enabled = enabled != undefined ? enabled : true;
	};
	CookieButton.prototype.getExpiration = function() {
		var date = new Date();
		date.setTime(date.getTime() + (1000 * 60 * 60 * 24 * 365)); // 1 Jahr
		return date;
	};
	CookieButton.prototype.update = function(el) {
		var jq_el = jQuery(el);
		if (!this.enabled) {
			var tooltipText = jq_el.attr('data-tooltip-text');
			if (tooltipText) {
				jq_el.hover(
					function(){tip(tooltipText)},
					function(){untip()}
				);
			}
		}
		if (this.isOn()) {
			jq_el.children().addClass('on');
		} else {
			jq_el.children().removeClass('on')
		}
	};
	CookieButton.prototype.toggle = function(el) {
		if (!this.enabled) {
			return;
		}
		var on = this.isOn();
		if (on) {
			jQuery.cookie(this.cookieName, null);				
		} else {
			jQuery.cookie(this.cookieName, "dummy", {expires: this.getExpiration()});				
		}
		// tracken
		var action = !on ? "ENABLE" : "DISABLE";
		window._gaq.push(['_trackEvent', 'LWK', this.component, action]);
		this.update(el);
	};
	CookieButton.prototype.isOn = function() {
		return this.enabled ? jQuery.cookie(this.cookieName) !== null : false;
	};
	/*END: Cookie Buttons*/
	
	/*Begin: Running Events*/
	eBet.runningSection = {
		update : function(el) {
			eBet.util.alternate("runningSection", jQuery(el), ['jq-event-row'], ['jq-header-row']);			
		}
	}
	
	eBet.runningEventRow = {
		loadStat : function(eventId,sportId) {
			if (eBet.statButton.btn.isOn()) {
				var url = "@betradar_stat_"+eventId+"/program/eventstatslayer?eventId="+eventId+"&sportId=" + sportId + "&type=liveConference";
				RefreshHandler.loadIfEmpty(url, true);
			}
		},
		
		cnlTip : function(tipId) {
			if (eBet.statButton.btn.isOn()) {
				untip(tipId);
			}
		},

		update : function(el, eventId, tipId, sportId, newGoalTime, goalBlinking, newRedCardTime, redCardBlinking) {
			// jquery.hover-function, da events von inneren divs gebubblt werden
			// eventId && tipId -> nur f�r laufende Events
			if (eventId && tipId) {
				jQuery(el).find('.jq-event-row').hover(
						function(){eBet.runningEventRow.loadStat(eventId, sportId, tipId)},
						function(){eBet.runningEventRow.cnlTip(tipId)}
				);
			}
			eBet.soundButton.playSound(eventId, sportId, newGoalTime, goalBlinking, newRedCardTime, redCardBlinking);
			markLiveResults(el);			
			
			var parent = jQuery(el).closest(".jq-rows-cont");
			eBet.util.alternate("runningSectionEvent", parent, ['jq-event-row'], ['jq-header-row']);
		}
	};
	/*END: Running Events*/

	/*Begin: Upcoming Events*/	
	eBet.upcomingSection = {
		compPrefix : "upcoming-",
		isDefaultOn : function(interval) {
			return interval == 1;
		},
		getWorkerId : function() {
			return "upcoming-section";
		},
		
		updateAppearance : function(on, interval, withEffects) {
			var jq_arrow = jQuery("#upcoming-"+interval+"-arrow");
			var jq_cp_row = jQuery("#upcoming-"+interval+"-cp-row");
			var jq_cont = jQuery("#upcoming-"+interval+"-cont");
			if (on) {
				//Upcoming 24 werden nachgeladen
				if (interval == 2) {
					RefreshHandler.loadIfEmpty('/program/conference/upcoming2eventssection', false);
				}
				jq_arrow.addClass('on');
				if (withEffects) {
					jq_cp_row.slideDown('fast');
					jq_cont.slideDown('fast');
				} else {
					jq_cp_row.show();
					jq_cont.show();					
				}
			} else {
				jq_arrow.removeClass('on');
				if (withEffects) {
					jq_cp_row.slideUp('fast');
					jq_cont.slideUp('fast');
				} else {
					jq_cp_row.hide();
					jq_cont.hide();										
				}
			}
		},
		
		update : function(el) {
			for (var interval = 1; interval <3; interval++) {
				var on = eBet.util.isStateOn(this.compPrefix + interval, null, this.isDefaultOn(interval));
				this.updateAppearance(on, interval, false);
			}
			eBet.util.alternate(this.getWorkerId(), jQuery(el), ['jq-event-row'], ['jq-header-row']);
		},
		
		toggle : function(interval) {
			var on = eBet.util.toggleState(this.compPrefix + interval, null, this.isDefaultOn(interval));
			this.updateAppearance(on, interval, true);
		}
	};
	
	eBet.upcomingEventRow = {
		update : function(el) {
			var parent = jQuery(el).closest(".jq-rows-cont");
			eBet.util.alternate(eBet.upcomingSection.getWorkerId(), parent, ['jq-event-row'], ['jq-header-row']);
			markLiveResults(el);			
		}
	};
	/*End: Upcoming Events*/

	eBet.tabPanel = {
		update : function(el, sectionType) {
			var btn = jQuery(el).find("#stat-button");
			eBet.statButton.btn.update(btn);
			btn = jQuery(el).find("#sound-button");
			eBet.soundButton.update(btn);
		},
		navTabPanel : function(sectionType) {
			var updateUrls = new Array();
			updateUrls[updateUrls.length] = '@sectionsPanel/program/conference/sectionsselector?sectionType='+sectionType;
			updateUrls[updateUrls.length] = '@tabPanel/program/conference/tabpanel?sectionType='+sectionType;
			RefreshHandler.completeOrUpdate(RefreshHandler.urlComplete, updateUrls);
		}
	};
	
	eBet.topEventRow = {
		update : function(el, eventId) {
			markLiveResults(el);
			eBet.sbLayer.toggle(el, eventId, true);
		}
	};

	eBet.statButton = {
		btn : new CookieButton('statButton', 'STATISTICS_LAYER_BUTTON')
	};
	
	eBet.soundButton = {
	    // cookieName wie in SoundButton.java
		cookieName : "playSounds",
	    soundApplause : "'/sounds/applause'",
	    soundBuh : "'/sounds/buh'",
	    sportIds : ["soccer", "ice-hockey"],
	    times : [],
	    btn : new CookieButton('playSounds', 'SOUND_BUTTON', flashAv()),
	    update: function(el) {
	    	this.btn.update(el);
	    },
	    toggle : function(el) {
    		this.btn.toggle(el);    	
	    },
	    isEnabled : function() {
	    	return this.btn.enabled;
	    },
	    /* goalBlinking/redCardBlinking - damit beim neuladen oder beim aktivieren von sounds
	     * die alten punkte nicht gespielt werden*/
	    playSound : function(eventId, sportId, newGoalTime, goalBlinking, newRedCardTime, redCardBlinking) {
	    	if (!(eventId && sportId && newGoalTime && newRedCardTime)) {
	    		return;
	    	}
	    	if (!flashAv()) {
	    		return;
	    	}
	    	if (!this.btn.isOn()) {
	    		return;
	    	}
	    	if (jQuery.inArray(sportId, this.sportIds) === -1) {
	    		return;
	    	}
	    	//alle preconditions erfüllt
	    	if (this.shouldPlaySound(eventId, this.soundApplause, newGoalTime, goalBlinking)) {
				setTimeout('sound_play(' + this.soundApplause + ')', 1);	
	    	}
	    	if (this.shouldPlaySound(eventId, this.soundBuh, newRedCardTime, redCardBlinking)) {
	    		setTimeout('sound_play(' + this.soundBuh + ')', 1);	
	    	}
	    },
	    shouldPlaySound : function(eventId, soundType, newEventPointTime, blinking) {
	    	this.times[soundType] = this.times[soundType] || [];
	    	var prevEventToTime = this.times[soundType][eventId] || [0];
	    	this.times[soundType][eventId] = [newEventPointTime];
	    	return (newEventPointTime > 0 && prevEventToTime[0] < newEventPointTime && blinking);
	    }

	};
	
	eBet.goalBox = {
		fadeOutTime : undefined,
		fadeOutInterval : 10000,

		lastEventPointId : undefined,
		animationShown : false, 
		fadeOutCallback : function() {
			eBet.goalBox.animationShown = false;	
		},
		update : function(el) {
			var jq_el = jQuery(el);
			jq_el.hover(
				function() {jq_el.find('.jq-history-layer').show();},
				function() {jq_el.find('.jq-history-layer').hide()}
			);
			//Penalty-Zeile
			var jq_penalty = jq_el.find('.jq-goalbox-penalty');
			if (jq_penalty.length) {
				markLiveResults(el);
				var eventPointId = jq_penalty.attr('data-event-point-id');
				this.updateAnimation(jq_penalty, eventPointId, false);				
			}
			//Highlight-Spielpunkt
			var jq_highlight = jq_el.find('.jq-goalbox-highlight');
			if (jq_highlight.length) {
				var eventPointId = jq_highlight.attr('data-event-point-id');
				this.updateAnimation(jq_highlight, eventPointId, true);
	    	}
		},
		
		updateAnimation : function(jq_el, eventPointId, withFadeOut) {
			//da die Spielernamen werden erst später eingetragen und dadurch wird die Komponente nachträglich verändert
			//Falls die Animation schon läuft, soll sie Animation weitergeführt werden -> dafür der Else-Zweig
			if (this.lastEventPointId != eventPointId) {
				this.lastEventPointId = eventPointId;
				jq_el.fadeIn('slow', function() {
					eBet.goalBox.animationShown = true;
					eBet.goalBox.fadeOutTime = new Date().getMilliseconds() + eBet.goalBox.fadeOutInterval;
				});
				if (withFadeOut) {
					jq_el.delay(eBet.goalBox.fadeOutInterval).fadeOut('slow', this.fadeOutCallback);	
				}
			} else if (this.animationShown) {
				eBet.goalBox.fadeOutTime = eBet.goalBox.fadeOutTime - new Date().getMilliseconds();
				jq_el.show();
				if (withFadeOut) {
					jq_el.delay(eBet.goalBox.fadeOutTime).fadeOut('slow',this.fadeOutCallback);
				}
			}			
		}
	};
	
	eBet.topEvent = {
		compName : "topevent",
		defaultOn : false,
		
		updateTimeline : function(id, node) {
			if (eBet.util.isStateOn(this.compName, id, this.defaultOn)){
				var topNode = jQuery("#top-event-" + id);
				var buttons = topNode.find(".details_but");
				buttons.find(".hide").show();
				buttons.find(":not(.hide)").hide()
				topNode.find(".details_row").addClass('on');
				topNode.find(".stat_soccer").show();
			} else{
				RefreshHandler.stopRefresh(jQuery(node));
			}
			eBet.preference.updateNode(id, jQuery(node).find(".event_tl"));
		},
		
		updateButton : function(id, node){
			if (eBet.util.isStateOn(this.compName, id, this.defaultOn)){
				var topNode = jQuery("#top-event-" + id);
				var buttons = topNode.find(".details_but");
				topNode.find(".details_row").addClass('on');
				buttons.find(".hide").show();
				buttons.find(":not(.hide)").hide()
			}
		},
		
		toggle : function(id, node) {
			var on = eBet.util.toggleState(this.compName, id, this.defaultOn);
			if (on) {
				RefreshHandler.nav("@timeline-" + id + "/conferencetimeline?id=" + id);
				var topNode = jQuery("#top-event-" + id);
				var buttons = topNode.find(".details_but");
				buttons.find(".hide").show();
				buttons.find(":not(.hide)").hide()
				topNode.find(".details_row").addClass('on');
				topNode.find(".stat_soccer").slideToggle('fast');
			} else{
				RefreshHandler.stopRefresh(jQuery("#comp-timeline-" + id));
				var topNode = jQuery("#top-event-" + id);
				topNode.find(".details_but").children().toggle();
				topNode.find(".details_row").toggleClass('on');
				topNode.find(".stat_soccer").slideToggle('fast');
			}
			eBet.liveTicker.toggle(node, id);
		}
	};

	eBet.topTennis = {
		compName : "topTennis",
		defaultOn: false,
		isStateOn : function(eventId) {
			return eBet.util.isStateOn(this.compName + eventId, null, this.defaultOn);
		},
		toggleState : function(eventId) {
			return eBet.util.toggleState(this.compName + eventId, null, this.defaultOn);
		},
		update : function(el, eventId) {
			var on = this.isStateOn(eventId);
			var jq_el = jQuery(el);
			this.updateAppearance(jq_el, on, false);
		},
		
		toggle : function(el, eventId) {
			var on = this.toggleState(eventId);
			var tennisEvent = jQuery(el).closest('.event');
			this.updateAppearance(tennisEvent, on, true);
			eBet.liveTicker.toggle(el, eventId);
			var stat = tennisEvent.find('.stat_tennis');
			eBet.topTennisScoreTable.updateAppearance(stat, on, true);
		}, 
		
		updateAppearance : function(jq_el, on, withEffects) {
			var detailsBut = jq_el.find('.details_but').children();
			if (on) {
				jQuery(detailsBut[1]).show();
				jQuery(detailsBut[0]).hide();
			} else {
				jQuery(detailsBut[1]).hide();
				jQuery(detailsBut[0]).show();
			}
			var detailsRow = jq_el.find('.details_row');
			if (on) {
				detailsRow.addClass('on');	
			} else {
				detailsRow.removeClass('on');
			}
		}	
	};
	
	/**Bedient LiveTicker von Top-Fu�ball und Top-Tennis*/
	eBet.liveTicker = {
		compName : "liveTicker",
		defaultOn: false,
		updateFrame : function(el, eventId) {
			var jq_el = jQuery(el);
			jq_el.tinyscrollbar({ sizethumb: 55 });
			var on = eBet.util.isStateOn(this.compName, eventId, this.defaultOn);
			if (on) {
				this.updateAppearance(jq_el, on);
			}
		},
		updateContent : function(el) {
			var jq_el = jQuery(el);
			var liveTicker = jq_el.closest('.event').find('.jq-live-ticker-frame');
			liveTicker.tinyscrollbar_update('relative');
		},
		toggle : function(toggle_el, eventId) {
			var on = eBet.util.toggleState(this.compName, eventId, this.defaultOn);
			var liveTicker = jQuery(toggle_el).closest('.event').find('.jq-live-ticker-frame');
			this.updateAppearance(liveTicker, on);
		},
		updateAppearance : function(liveTicker, on) {
			var viewPort = liveTicker.find('.viewport');
			if (on) {
				viewPort.addClass('on');	
			} else {
				viewPort.removeClass('on');
			}	
			liveTicker.tinyscrollbar_update(); // wird nach oben gescrollt 
		}
	};
	
	eBet.topTennisScoreTable = {
		update : function(el, eventId) {
			var jq_el = jQuery(el);
			var on = eBet.topTennis.isStateOn(eventId);
			this.updateAppearance(jq_el, on, false);
		},
		updateAppearance : function(jq_el, on, withEffects) {
			if (on) {
				if (withEffects) {
					jq_el.slideDown('fast');
				} else {
					jq_el.show();
				}
			} else {
				if (withEffects) {
					jq_el.slideUp('fast');
				} else {
					jq_el.hide();
				}
			}
		}
	};
	
	eBet.topCompetition = {
		compName : "topCompetition",
		defaultOn : false,
		
		update : function(el, id) {
			if (eBet.util.isStateOn(this.compName, id, this.defaultOn)) {
				jQuery(el).find('.hide').show();
				var closeButton = jQuery(el).find("#close_" + id);
				closeButton.find(".hide").show();
				closeButton.find(":not(.hide)").hide()
				this.updateSection(el, id, 'yesNoSection');
			}
		},
		
		updateSection : function(el, id, sectionType){
			if (eBet.util.isStateOn(this.compName, id, this.defaultOn)) { 
				jQuery(el).find('.hide').show();
			}
			//bei updates von CompetitionSection soll auch markLiveResults aufgerufen werden
			//weil die Zeilen nicht eigenständige Komponenten sind
			markLiveResults(el);
			eBet.util.alternate(sectionType+"-"+id, jQuery(el), ['jq-result-set-row'], []);
		},
		
		updateRow : function(el, id, rowType) {
			if (eBet.util.isStateOn(this.compName, id, this.defaultOn)) {
				jQuery(el).find(".hide").show();
			}
			markLiveResults(el);
			var parent = jQuery(el).closest(".jq-rows-cont");
			eBet.util.alternate(rowType+'-'+id, parent, ['jq-result-set-row'], []);
		},
		
		toggle : function(id, topEventNode) {
			var on = eBet.util.toggleState(this.compName, id, this.defaultOn);
			//Toggle MoreLess
			var closeButton = jQuery("#close_" + id);
			if (on) {
				jQuery(topEventNode).find(".hide").slideDown('fast');
				closeButton.find(".hide").show();
				closeButton.find(":not(.hide)").hide()
			}  else {
				jQuery(topEventNode).find(".hide").slideUp('fast');
				closeButton.find(".hide").hide();
				closeButton.find(":not(.hide)").show();
			}
		}
	};
	
	eBet.preference = {
		hiddenPointTypes : {},
		
		update : function(id, el){
			var hidden = this.hiddenPointTypes[id] || {}; 
			for (var pointType in hidden){
				if (pointType != undefined && hidden[pointType] == true){
					jQuery(el).find(".option_c2." + pointType).toggleClass("on");
					jQuery(el).find(".option_c3." + pointType).children().toggleClass("hide");
				}
			}
		},
		
		toggle : function(id, pointType, el){
			this.hiddenPointTypes[id] = this.hiddenPointTypes[id] || {};
			this.hiddenPointTypes[id][pointType] = this.hiddenPointTypes[id][pointType] ? false : true;
			this.updateNode(id, jQuery("#comp-timeline-" + id).find(".event_tl"));
			this.updateNode(id, jQuery("#top-event-" + id).find('.jq-live-ticker-frame'));
			eBet.liveTicker.updateContent(el);
		},
		
		updateNode : function(id, node){
			var hiddenPointTypes = this.hiddenPointTypes[id] || {};
			for (var pointType in hiddenPointTypes){
				if (hiddenPointTypes.hasOwnProperty(pointType)) {
					var value = hiddenPointTypes[pointType]; 
					if (value) {
						node.find("." + pointType).hide();
					} else {
						node.find("." + pointType).show();
					}
				}
			}
		}
	};
})();