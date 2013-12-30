using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier.parser
{
    public partial class DataParser
    {
        public List<SportEvent> Events { get; set; }


        // ispravljenaq pasing funkcija
        public void ParseHtml(HtmlDocument document)
        {
            Events = new List<SportEvent>();

            // div class="sheet_col_0 pad_l_12 bold left
            string currentSport = string.Empty;
            string currentSubtype = string.Empty;
            string currentLeague = string.Empty;
            string currentDate = string.Empty;
            
            SportEvent sportEvent = new SportEvent();
            
            sportEvent.Sport = currentSport;
            sportEvent.Subtype = currentSubtype;
            sportEvent.League = currentLeague;
            sportEvent.Date = currentDate;
            // sportEvent.Time = sTime;

            SportEvent currentEvent = null;

            //running events
            var divRunning = document.GetElementById("_program_conference_runningeventssection");
            var divComming = document.GetElementById("_program_conference_upcoming1eventssection");
            var divs = divRunning.GetElementsByTagName("div");
            foreach (HtmlElement div in divs)
            {
                
                if (div.GetAttribute("class").Contains("main_space border_ccc"))
                {
                    var eventDivs = div.GetElementsByTagName("div");
                    foreach (HtmlElement eventDiv in eventDivs)
                    {
                        if (eventDiv.GetAttribute("id").Contains("jq-event-id"))
                        {
                            var gameDivs = div.GetElementsByTagName("div");
                            foreach (HtmlElement gameDiv in gameDivs)
                            {
                                if (gameDiv.GetAttribute("class").Contains("c_2"))
                                {
                                    List<string> elemets = new List<string>();
                                    
                                    for (int i = 0; i <= 5; i++)
                                    {
                                        elemets[i] = sportEvent.Sport;
                                    }
                                        
                                     

                                     if (currentEvent != null && elemets.Count != 0)
                                        {
                                            SportGame sportGame = new SportGame(elemets[0]);

                                                string parametar = null;
                                                for (int i = 1; i < elemets.Count; i++)
                                                {
                                                    if (string.IsNullOrEmpty(parametar))
                                                    {
                                                        parametar = elemets[i];
                                                    }
                                                    else
                                                    {
                                                        if (!string.IsNullOrEmpty(elemets[i]))
                                                        {
                                                            double value = 0.0;
                                                            double.TryParse(elemets[i].Replace(',', '.'), out value);

                                                            sportGame.Quotes.Add(parametar, value);
                                                        }
                                                        else if (!string.IsNullOrEmpty(parametar))
                                                        {
                                                            double value = 0.0;
                                                            double.TryParse(parametar.Replace(',', '.'), out value);

                                                        sportGame.Quotes.Add(string.Empty, value);
                                                    }
                                                    parametar = null;
                                                }
                                            }

                                            currentEvent.Games.Add(sportGame);
                                    }   
               
                                    if (elemets.Count <= 5)
                                    {
                                        string sTime = elemets[0].Trim();
                                        string sTeam = elemets[1].Trim();
                                        string odds1 = elemets[3].Replace(',', '.');
                                        string oddsX = elemets[4].Replace(',', '.');
                                        string odds2 = elemets[5].Replace(',', '.');

                                        string[] teams = Fix(sTeam).Split(new string[] { " - " }, StringSplitOptions.None);
                                           
                                        if (teams.Length == 2)
                                            {
                                                sportEvent.Home = teams[0].Trim();
                                                sportEvent.Away = teams[1].Trim();
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        
                                        double matchOdds1 = 0.0;
                                        double.TryParse(odds1, out matchOdds1);
                                        double matchOddsX = 0.0;
                                        double.TryParse(oddsX, out matchOddsX);
                                        double matchOdds2 = 0.0;
                                        double.TryParse(odds2, out matchOdds2);

                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    

                                    //sportEvent.Sport = currentSport;
                                    //sportEvent.Subtype = currentSubtype;
                                    //sportEvent.League = currentLeague;
                                    //sportEvent.Date = currentDate;
                                    //sportEvent.Time = sTime;

                                    //string[] teams = Fix(sTeam).Split(new string[] { " - " }, StringSplitOptions.None);
                                    //if (teams.Length == 2)
                                    //{
                                    //    sportEvent.Home = teams[0].Trim();
                                    //    sportEvent.Away = teams[1].Trim();
                                    //}
                                    //else
                                    //{
                                    //    continue;
                                    //}

                                    //SportGame sportGame = new SportGame("3-Way");
                                    //sportGame.Quotes.Add("1", matchOdds1);
                                    //sportGame.Quotes.Add("X", matchOddsX);
                                    //sportGame.Quotes.Add("2", matchOdds2);

                                    //sportEvent.Games.Add(sportGame);

                                    //    Events.Add(sportEvent);

                                    //    currentEvent = sportEvent;
                                    //}
                                    //else
                                    //{
                                    //    continue;
                                    //}
                                }
                                            }
                                            //ovde parsiraj sadrzaj eventa
                                            //home team
                            
                                            //away team
                                            //score
                                            //quotas
                                        }
                                    }
                                }
                            }
                        }

        public void ParseHtml(HtmlDocument document)
        {
            Events = new List<SportEvent>();

            // div class="sheet_col_0 pad_l_12 bold left
            string currentSport = string.Empty;
            string currentSubtype = string.Empty;
            string currentLeague = string.Empty;
            string currentDate = string.Empty;

            SportEvent currentEvent = null;

            foreach (HtmlElement tr in document.Body.GetElementsByTagName("div"))
            {
                string outerHTML = string.IsNullOrEmpty(tr.OuterHtml) ? string.Empty : tr.OuterHtml.TrimStart();
                if (outerHTML.StartsWith("<DIV class=\"col ext "))
                {
                    List<string> elemets = new List<string>();
                    foreach (HtmlElement td in tr.GetElementsByTagName("div"))
                    {
                        if (td.GetElementsByTagName("div").Count == 0)
                        {
                            elemets.Add(td.InnerText != null ? td.InnerText.Trim() : null);
                        }
                    }

                    if (elemets.Count >= 11)
                    {
                        SportEvent sportEvent = new SportEvent();

                        string sTime = elemets[0].Trim();
                        string sTeam = elemets[1].Trim();
                        string odds1 = elemets[3].Replace(',', '.');
                        string oddsX = elemets[4].Replace(',', '.');
                        string odds2 = elemets[5].Replace(',', '.');

                        double matchOdds1 = 0.0;
                        double.TryParse(odds1, out matchOdds1);
                        double matchOddsX = 0.0;
                        double.TryParse(oddsX, out matchOddsX);
                        double matchOdds2 = 0.0;
                        double.TryParse(odds2, out matchOdds2);

                        sportEvent.Sport = currentSport;
                        sportEvent.Subtype = currentSubtype;
                        sportEvent.League = currentLeague;

                        sportEvent.Date = currentDate;
                        sportEvent.Time = sTime;

                        string[] teams = Fix(sTeam).Split(new string[] { " - " }, StringSplitOptions.None);
                        if (teams.Length == 2)
                        {
                            sportEvent.Home = teams[0].Trim();
                            sportEvent.Away = teams[1].Trim();
                        }
                        else
                        {
                            continue;
                        }

                        SportGame sportGame = new SportGame("3-Way");
                        sportGame.Quotes.Add("1", matchOdds1);
                        sportGame.Quotes.Add("X", matchOddsX);
                        sportGame.Quotes.Add("2", matchOdds2);

                        sportEvent.Games.Add(sportGame);

                        Events.Add(sportEvent);

                        currentEvent = sportEvent;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (outerHTML.StartsWith("<DIV class=\"bg_grey border")
                    || outerHTML.StartsWith("<DIV class=\"bg_othergrey border"))
                {
                    List<string> elemets = new List<string>();
                    foreach (HtmlElement td in tr.GetElementsByTagName("div"))
                    {
                        if (td.GetElementsByTagName("div").Count == 0)
                        {
                            elemets.Add(td.InnerText != null ? td.InnerText.Trim() : null);
                        }
                    }

                    if (elemets.Contains("Goal scorer"))
                    {
                        continue;
                    }

                    if (currentEvent != null && elemets.Count != 0)
                    {
                        SportGame sportGame = new SportGame(elemets[0]);

                            string parametar = null;
                            for (int i = 1; i < elemets.Count; i++)
                            {
                                if (string.IsNullOrEmpty(parametar))
                                {
                                    parametar = elemets[i];
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(elemets[i]))
                                    {
                                        double value = 0.0;
                                        double.TryParse(elemets[i].Replace(',', '.'), out value);

                                        sportGame.Quotes.Add(parametar, value);
                                    }
                                    else if (!string.IsNullOrEmpty(parametar))
                                    {
                                        double value = 0.0;
                                        double.TryParse(parametar.Replace(',', '.'), out value);

                                    sportGame.Quotes.Add(string.Empty, value);
                                }
                                parametar = null;
                            }
                        }

                        currentEvent.Games.Add(sportGame);
                    }
                }
                else if ((outerHTML.Contains("onmouseover=\"tip('To ") && tr.GetElementsByTagName("div").Count == 0)
                    || (outerHTML.Contains("onmouseover=\"tip('Back ")))
                {
                    List<string> elemets = new List<string>();
                    foreach (HtmlElement td in tr.GetElementsByTagName("a"))
                    {
                        if (!string.IsNullOrEmpty(td.InnerText))
                        {
                            // currentSport = string.IsNullOrEmpty(td.InnerText) ? string.Empty : Fix(td.InnerText.Trim());
                            string[] strings = Fix(string.IsNullOrEmpty(td.InnerText) ? string.Empty : Fix(td.InnerText.Trim())).Split(new string[] { " - " }, StringSplitOptions.None);
                            if (strings.Length == 2)
                            {
                                currentSport = strings[0];
                                currentSubtype = strings[1];
                            }
                        }
                    }
                }
                else if (outerHTML.StartsWith("<DIV class=\"col_10"))
                {
                    if (!string.IsNullOrEmpty(tr.InnerText))
                    {
                        currentDate = tr.InnerText;
                        currentDate = currentDate.TrimEnd();
                        if (currentDate.Length > 6)
                        {
                            currentDate = DateTime.Now.Year.ToString() + "-" + currentDate.Substring(currentDate.Length - 3, 2) + "-" + currentDate.Substring(currentDate.Length - 6, 2);
                        }
                    }
                }
                else if (outerHTML.StartsWith("<DIV class=\"sheet_col_0 pad_l_12 bold left"))
                {
                    if (!string.IsNullOrEmpty(tr.InnerText))
                    {
                        currentLeague = string.IsNullOrEmpty(tr.InnerText) ? string.Empty : Fix(tr.InnerText.Trim());
                    }
                }
                
                
                // dodato 
                else if (outerHTML.StartsWith("<DIV id=\"alert_layer"))
                {
                    if (!string.IsNullOrEmpty(tr.InnerText))
                    {
                        currentLeague = string.IsNullOrEmpty(tr.InnerText) ? string.Empty : Fix(tr.InnerText.Trim());
                    }
                }
            }
        }
    }
}
