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
        public void ParseHtml(HtmlDocument document)
        {
            Events = new List<SportEvent>();

            string currentSport = string.Empty;
            string currentSubtype = string.Empty;
            string currentLeague = string.Empty;
            string currentDate = string.Empty;

            foreach (HtmlElement tr in document.GetElementsByTagName("h1"))
            {
                if (!string.IsNullOrEmpty(tr.InnerText))
                {
                    currentSport = tr.InnerHtml.Trim().Replace(" Results", string.Empty);
                }
            }
            foreach (HtmlElement tr in document.GetElementsByTagName("div"))
            {
                string outerHTML = string.IsNullOrEmpty(tr.OuterHtml) ? string.Empty : tr.OuterHtml.TrimStart();
                if (outerHTML.StartsWith("<DIV class=\"col sheet_zoom_bg") || outerHTML.StartsWith("<DIV class=\"col bg_grey") || outerHTML.StartsWith("<DIV class=\"col over") || outerHTML.StartsWith("<DIV class=\"col clear"))
                {
                    List<string> elemets = new List<string>();
                    foreach (HtmlElement td in tr.GetElementsByTagName("div"))
                    {
                        if (td.GetElementsByTagName("div").Count == 0)
                        {
                            elemets.Add(td.InnerText != null ? td.InnerText.Trim() : null);
                        }
                    }

                    if (elemets.Count >= 4)
                    {
                        SportEvent sportEvent = new SportEvent();

                        sportEvent.Sport = currentSport;
                        sportEvent.Subtype = currentSubtype; 
                        sportEvent.League = currentLeague;
                        sportEvent.Date = currentDate;

                        sportEvent.Time = elemets[0];

                        string[] teams = Fix(elemets[1]).Split(new string[] { " - " }, StringSplitOptions.None);
                        if (teams.Length == 2)
                        {
                            sportEvent.Home = teams[0].Trim();
                            sportEvent.Away = teams[1].Trim();
                        }
                        else
                        {
                            continue;
                        }



                        sportEvent.Status = elemets[2];
                        sportEvent.WinningOdds = elemets[3];

                        Events.Add(sportEvent);
                    }
                    else if (elemets.Count == 1)
                    {
                        // currentLeague = elemets[0];
                    }
                    else
                    {
                        continue;
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
                            currentSport = string.IsNullOrEmpty(td.InnerText) ? string.Empty : Fix(td.InnerText.Trim());
                        }
                    }
                }
                else if (outerHTML.StartsWith("<DIV class=\"col_13"))
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
                        // currentLeague = string.IsNullOrEmpty(tr.InnerText) ? string.Empty : Fix(tr.InnerText.Trim());
                        string[] strings = Fix(string.IsNullOrEmpty(tr.InnerText) ? string.Empty : Fix(tr.InnerText.Trim())).Split(new string[] { " - " }, StringSplitOptions.None);
                        if (strings.Length == 3)
                        {
                            currentSport = strings[0];
                            currentSubtype = strings[1];
                            currentLeague = strings[2];
                        }

                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
