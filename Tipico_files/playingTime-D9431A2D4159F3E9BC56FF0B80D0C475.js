(function() {
	//Sekundengenaue Anzeige von Spielzeit bei Top-Fußball. Wird ins Tablet kopiert und da auch verwendet 
	eBet.playingTime = {
		// Timer
		timer : {},
		// Letzter Synchronisierungszeitpunkt mit dem Server
		synchronizeTime : {},
		// Letzte synchronisierte Spielzeit auf dem Server
		synchronizePlayingTime : {},
		// Aktuelle Spielzeit
		playingTime : {},
		// Unterbrechung
		interrupted : {},
		startTimeCalculation : function (serverPlayingTime, serverInterrupted, id, el){
			// Timer immer unterbrechen
			this.stopTimeCalculation(id);
			if(serverPlayingTime != undefined && serverPlayingTime != 0){
				// Serverzeit übernehmen, falls...
				// - Noch keine Zeit gesetzt
				// - Differenz zwischen Client und Server > 5 Sekunden
				// - Unterbrechung / Fortsetzung
				if(this.synchronizePlayingTime[id] == undefined || Math.abs(serverPlayingTime-this.playingTime[id]) > 5 || serverInterrupted != this.interrupted[id]){
					this.synchronizeTime[id] = new Date().getTime();
					this.synchronizePlayingTime[id] = serverPlayingTime;
					this.playingTime[id] = this.synchronizePlayingTime[id];
				}
				this.interrupted[id] = serverInterrupted;
			}else{
				// Differenz zur letzten Serversynchronisierung berechnen
				var now = new Date().getTime();
				var diff = now-this.synchronizeTime[id];
				this.playingTime[id] = this.synchronizePlayingTime[id]+Math.floor(diff/1000);
			}
			
			
			// Minuten und Sekunden berechnen
			var minutes = Math.floor(this.playingTime[id] / 60);
			var seconds = this.playingTime[id] - minutes * 60;
			
			// Text setzen
			jQuery(el).find('.jq-playingTime-' + id).text(this.timeFormat(minutes)+'\''+this.timeFormat(seconds))+'\'\'';
			
			// Timer neustarten (falls keine Unterbrechung)
			if(!this.interrupted[id]) {
				function fncStartTimeCalculation() {
					eBet.playingTime.startTimeCalculation(0,false, id, el);
				}
				this.timer[id] = setTimeout(fncStartTimeCalculation,200);
			}
		},

		stopTimeCalculation : function(id){
			if(this.timer[id] != undefined) {
				clearTimeout(this.timer[id]);
				this.timer[id] = undefined;
			}
		},
		
		timeFormat : function(number) {
			return (number < 10) ? "0" + number : number;
		},

		resetTimeCalculation : function(id){
			this.stopTimeCalculation(id);
			this.synchronizeTime[id] = undefined;
			this.synchronizePlayingTime[id] = undefined;
			this.playingTime[id] = undefined;
			this.interrupted[id] = false;
		}
	};
})();