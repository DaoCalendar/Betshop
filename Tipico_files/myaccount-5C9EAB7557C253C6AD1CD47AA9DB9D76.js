var ebetAccount = {
		getTicketListUrl: function(){
			this.openTicketSummaries = {};
			this.openTicketRows = {};
			return jQuery("#comp-myContent").attr("e:url");
		},
		
		getBonusOverviewUrl: function(){
			return jQuery("#comp-bonusOverviewList").attr("e:url");
		},
		
		getBonusFilterUrl: function(){
			return jQuery("#comp-bonusFilter").attr("e:url");
		},
		
		getBonusUpdateUrl: function(){
			return "accounting2/bonus/update";
		},
		
		
		getParamListFromUrl: function(url){
			if (url.indexOf("?") == -1){
				return "";
			}
			else{
				return url.substr(url.indexOf("?") + 1);
			}

		},
		
		getUrlWithoutParamList: function(url){
			if (url.indexOf("?") == -1){
				return url;
			}else{
				//incl question mark
				return url.substr(0, url.indexOf("?") + 1);
			}
		},
		
		changePage: function(currentPage, chosenPage) {
			showDelayLayer();
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url).replace("currentPage=" + currentPage, "currentPage=" + chosenPage);
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);
		},	
		
		changeBetsPerPage: function(currentPage, entriesPerPage, newEntriesPerPage) {
			showDelayLayer();
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url).replace("currentPage=" + currentPage, "currentPage=1").replace("entriesPerPage=" + entriesPerPage, "entriesPerPage=" + newEntriesPerPage);
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);
		},
		
		delSelection: function(daysBack, layerTitle, layerTxt, cancelBtnTxt, confBtnTxt) {
			var url = this.getTicketListUrl();
			var paramList = this.getParamListFromUrl(url);
			showConfLayer("/img/warn.gif", "66px", "56px", layerTitle, layerTxt,"flex_button_new_grey_bg inline", "flex_button_new_grey",cancelBtnTxt,"flex_button_new_red_arr_bg margin_l_12 inline","flex_button_new_red",confBtnTxt)
			.then(function(val) {
				if (val) {
					showDelayLayer();
					RefreshHandler.completeOrUpdate(RefreshHandler.urlComplete, ["/ticket2/delTickets?" + paramList + "&daysBack=" + daysBack, url])
				}
				return val;
			});
		},	

		cancelPayment: function(paymentId, layerTitle, layerTxt, cancelBtnTxt, confBtnTxt) {
			var url = this.getTicketListUrl();
			showConfLayer("/img/warn_grey.png", "66px", "56px", layerTitle, layerTxt,"flex_button_new_grey_bg inline", "flex_button_new_grey",cancelBtnTxt,"flex_button_new_green_arr_bg margin_l_12 inline","flex_button_new_green",confBtnTxt)
			.then(function(val) {
				if (val) {
					showDelayLayer();
					RefreshHandler.completeOrUpdate(RefreshHandler.urlComplete, ["/accounting2/cancelPayment?paymentId=" + paymentId, url])
				}
				return val;
			});			
		},
		
		buybackTicket: function(buybackValue, ticketId, layerTitle, layerTxt, cancelBtnTxt, confBtnTxt, url) {
			if(!url)
				url = this.getTicketListUrl();
			showConfLayer("/img/warn_grey.png", "66px", "56px", layerTitle, layerTxt,"flex_button_new_grey_bg inline", "flex_button_new_grey",cancelBtnTxt,"flex_button_new_red_arr_bg margin_l_12 inline","flex_button_new_red",confBtnTxt)
			.then(function(val) {
				if (val) {
					showDelayLayer();
					RefreshHandler.completeOrUpdate(RefreshHandler.urlComplete, ["/ticket2/buybackTicket?buybackValue=" + buybackValue + "&ticketId=" + ticketId, url])
				}
				return val;
			});
		},

		cancelTicket: function(ticketId, ticketStatus, layerTitle, layerTxt, cancelBtnTxt, confBtnTxt) {
			var url = this.getTicketListUrl();
			var paramList = this.getParamListFromUrl(url);
			showConfLayer("/img/warn_grey.png", "66px", "56px", layerTitle, layerTxt,"flex_button_new_grey_bg inline", "flex_button_new_grey",cancelBtnTxt,"flex_button_new_red_arr_bg margin_l_12 inline","flex_button_new_red",confBtnTxt)
			.then(function(val) {
				if (val) {
					showDelayLayer();
					RefreshHandler.completeOrUpdate(RefreshHandler.urlComplete, ["/ticket2/cancelTicket?" + paramList + "&ticketId=" + ticketId + "&ticketStatus=" + ticketStatus, url])
				}
				return val;
			});
		},
		
		changePeriod: function(time, newTime, currentPage) {
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url).replace("time=" + time, "time=" + newTime).replace("currentPage=" + currentPage, "currentPage=1");
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);
		},
		
		changeUser: function(user, newUser, currentPage) {
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url).replace("selectedLogin=" + user, "selectedLogin=" + newUser).replace("selectedLogin=null", "selectedLogin=" + newUser).replace("currentPage=" + currentPage, "currentPage=1");
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);
		},
		
		changeToTimechoose: function(time, paramList) {
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url).replace("time=" + time, "time=TIMECHOOSE");
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);
		},
		
		
		
		bonusUpdate : function (updateUrl, updateParams){
			var index = updateUrl.indexOf('?');
			var url = updateUrl;
			if (index == -1){
				url += '?';
			}else if (index != updateUrl.length-1){
				url += '&';
			}
			url = url + jQuery.param(updateParams);
			RefreshHandler.nav( url , null, function(){
				hideDelayLayer();
			});
			
			/**
			var urls = new Array();
			urls[0] = this.getUrlWithoutParamList(this.getBonusFilterUrl());
			urls[1] = this.getUrlWithoutParamList(this.getBonusOverviewUrl());
			
			RefreshHandler.nav(url, null);
			 */
		},
		
		changeBonusOverview  : function (updateParams){
			var url = this.getBonusUpdateUrl() + '?' + jQuery.param(updateParams);
			var urls = new Array();
			urls[0] = this.getUrlWithoutParamList(this.getBonusOverviewUrl());
			urls[1] = this.getUrlWithoutParamList(this.getBonusFilterUrl());
			
			RefreshHandler.nav(url, null, function(){
				RefreshHandler.completeOrUpdate(RefreshHandler.urlComplete, urls);
				hideDelayLayer();
			});
		},
	
		changeBonus: function( bonusId )
		{
			var url = this.getBonusOverviewUrl();
			var newParamList = this.getParamListFromUrl(url).replace("currentPage="+currentPage, "currentPage=1").replace("bonusId="+bonusId, "bonusId="+newBonusId).replace("bonusId=-1", "bonusId="+newBonusId);
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);	
			
			var filterUrl = this.getBonusFilterUrl();
			var newFilterParamList = this.getParamListFromUrl(filterUrl).replace("bonusId="+bonusId, "bonusId="+newBonusId).replace("bonusId=-1", "bonusId="+newBonusId);
			RefreshHandler.complete(this.getUrlWithoutParamList(filterUrl) + newFilterParamList);	
		},
		
		
		
		changeTimeChosen: function(paramList, url) {
			var datepickerTo = jQuery('#datepickerTo');
			var datepickerFrom = jQuery('#datepickerFrom');
			if (datepickerTo != undefined && datepickerFrom != undefined) {
				if (datepickerFrom.val().length > 0 && datepickerTo.val().length > 0) {
					showDelayLayer();
					var newParamList = paramList;
					var dateFromIndex = newParamList.indexOf("&dateFrom=");
					if (dateFromIndex !== -1){
						var endIndex = newParamList.indexOf("&",dateFromIndex+1)
						newParamList = newParamList.substring(0,dateFromIndex) + newParamList.substring(endIndex, newParamList.length);
					}
					newParamList = newParamList + "&dateFrom=" + datepickerFrom.val();
					var dateToIndex = newParamList.indexOf("&dateTo=");
					if (dateToIndex !== -1){
						var endIndex = newParamList.indexOf("&",dateToIndex+1)
						newParamList = newParamList.substring(0,dateToIndex) + newParamList.substring(endIndex, newParamList.length);
					}
					newParamList = newParamList + "&dateTo=" + datepickerTo.val();
					RefreshHandler.complete(url + newParamList);								
				}
			}
		},
		
		changeStatus: function(status, newStatus, currentPage) {
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url).replace("status=" + status, "status=" + newStatus).replace("currentPage=" + currentPage, "currentPage=1");
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);
		},
		
		updateOrder: function(newOrder, order, desc, currentPage) {
			// Falls newOrder = order -> desc/asc toggeln
			// sonst order ändern.
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url);
			if (newOrder == order) {
				newParamList = newParamList.replace("desc=" + desc, "desc=" + !desc).replace("currentPage=" + currentPage, "currentPage=1");
			}
			else {
				newParamList = newParamList.replace("order=" + order, "order=" + newOrder).replace("desc=" + desc, "desc=true").replace("currentPage=" + currentPage, "currentPage=1");
			}
			RefreshHandler.complete(this.getUrlWithoutParamList(url) + newParamList);
		},
		
		updateDelRequest: function(delRequest) {
//			newParamList = this.getParamListFromUrl(this.getTicketListUrl()).replace("delRequest=" + delRequest, "delRequest=" + !delRequest);
//			RefreshHandler.complete("@infoBlock/ticket2/infoBlock?" + newParamList);
			jQuery("#comp-myContent").find(".info_bar").show();
		},
		
		loadDatepicker: function(locale, oldTimeParam) {
			var datepickerTo = jQuery('#datepickerTo');
			var datepickerFrom = jQuery('#datepickerFrom');
			var url = this.getTicketListUrl();
			var newParamList = this.getParamListFromUrl(url).replace("time=" + oldTimeParam, "time=TIMECHOOSE");
			url = this.getUrlWithoutParamList(url);
			if (datepickerTo != undefined && datepickerFrom != undefined) {
				datepickerTo.datepicker(jQuery.extend({},
						jQuery.datepicker.regional[locale], {
			    	            onSelect: function(date) {
			    	            	ebetAccount.changeTimeChosen(newParamList, url);
			    	            }
			    	    }));
				datepickerFrom.datepicker(jQuery.extend({},
						jQuery.datepicker.regional[locale], {
			    	            onSelect: function(date) {
			    	            	ebetAccount.changeTimeChosen(newParamList, url);
			    	            }
			    	    }));
			}
		},
		rejectTicket: function(ticketId) {
			RefreshHandler.nav("/layer/rejectTicket?ticketId=" + ticketId)
		},
		
		acceptTicket: function(ticketId) {
			RefreshHandler.nav("/layer/acceptTicket?ticketId=" + ticketId)
		},
		openTicketRows : {},
		
		togLineStyle: function(this_obj, betId){
			jQuery(this_obj).parent().parent().find('.hr').toggle().parent().parent().find('.sheet_body').toggleClass('on').toggle().find('.sheet_body_sub').toggleClass('hide');
			
			var line = jQuery(this_obj).parent().parent().next().next();
			if (line.attr("style")){
				line.removeAttr("style");
			} else{
				line.css("margin-left", "-93px");
				line.css("width", "754px");
			}
			
			if (this.openTicketRows[betId] == undefined){
				this.openTicketRows[betId] = '#jq-' + betId;
			}else{
				this.openTicketRows[betId] = undefined;
			}
		},
		
		openTicketSummaries : {},
		togTicketSummary: function(this_obj, ticketId){
			jQuery(this_obj).toggleClass('on').parent().parent().parent().find('.sheet_body').toggleClass('on').find('.sheet_body_sub').toggle().parent().find('.ie_w100.hide').toggle();
			if (this.openTicketSummaries[ticketId] == undefined){
				this.openTicketSummaries[ticketId] = '#jq-' + ticketId;
			}else{
				this.openTicketSummaries[ticketId] = undefined;
			}
		},
		
		updateTogTicketSummaries: function(){
			for (var id in this.openTicketSummaries){
				if (id!=undefined && this.openTicketSummaries[id]!=undefined){
					jQuery(this.openTicketSummaries[id]).toggleClass('on').parent().parent().parent().find('.sheet_body').toggleClass('on').find('.sheet_body_sub').toggle().parent().find('.ie_w100.hide').toggle();
				}
			}
			
			for (var id in this.openTicketRows){
				if (id!=undefined && this.openTicketRows[id] !=undefined){
					var node = jQuery(this.openTicketRows[id]);
					node.toggleClass('on').parent().parent().find('.hr').toggle().parent().parent().find('.sheet_body').toggleClass('on').toggle().find('.sheet_body_sub').toggleClass('hide');
					var line = node.parent().parent().next().next();
					line.css("margin-left", "-93px");
					line.css("width", "754px");
				}
			}
		},
		
		toggleTicketRow: function(this_obj){
			jQuery(this_obj).toggleClass('on').parent().parent().find('.hr').toggle().parent().parent().find('.sheet_body').toggleClass('on').toggle().find('.sheet_body_sub').toggleClass('hide');
		}
}