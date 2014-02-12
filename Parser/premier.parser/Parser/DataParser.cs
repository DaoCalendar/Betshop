using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier.parser
{
    public class DataParser
    {
        public List<SportEvent> Events { get; set; }
        public void ParseHtml(HtmlDocument document)
        {
            List<SportEvent> newItems = new List<SportEvent>();

            // div class="sheet_col_0 pad_l_12 bold left
            string currentSport = string.Empty;
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

                    if (elemets.Count == 11)
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
                        sportEvent.League = currentLeague;

                        sportEvent.Date = currentDate;
                        sportEvent.Time = sTime;

                        string[] teams = sTeam.Split(new string[] { " - " }, StringSplitOptions.None);
                        if (teams.Length == 2)
                        {
                            sportEvent.Home = teams[0].Trim();
                            sportEvent.Away = teams[1].Trim();
                        }
                        else
                        {
                            sportEvent.Home = null;
                        }

                        SportGame sportGame = new SportGame("3-Way");
                        sportGame.Quotes.Add("1", matchOdds1);
                        sportGame.Quotes.Add("X", matchOddsX);
                        sportGame.Quotes.Add("2", matchOdds2);

                        sportEvent.Games.Add(sportGame);

                        newItems.Add(sportEvent);

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
                                double value = 0.0;
                                double.TryParse(elemets[i].Replace(',', '.'), out value);

                                sportGame.Quotes.Add(parametar, value);
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
                            currentSport = string.IsNullOrEmpty(td.InnerText) ? string.Empty : td.InnerText.Trim();
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
                            // currentDate = currentDate.Substring(currentDate.Length - 6);
                        }
                    }
                }
                else if (outerHTML.StartsWith("<DIV class=\"sheet_col_0 pad_l_12 bold left"))
                {
                    if (!string.IsNullOrEmpty(tr.InnerText))
                    {
                        currentLeague = string.IsNullOrEmpty(tr.InnerText) ? string.Empty : tr.InnerText.Trim();
                    }
                }
            }

            premier.parser.serialization.Serialize.f(newItems);

            Events = newItems;
        }
    }
}
