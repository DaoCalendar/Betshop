using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier.parser
{
    public class basic
    {
        public void ParseHtml(HtmlDocument document)
        {
            string sport = "testiranje";

            header mainHeader = new header(sport);
            mainHeader.Add("time");
            mainHeader.Add("team");
            mainHeader.Add("blank");
            mainHeader.Add("1");
            mainHeader.Add("X");
            mainHeader.Add("2");
            mainHeader.Add("league");
            mainHeader.Add("date");
            mainHeader.Add("sport");

            List<item> newItems = new List<item>();

            // div class="sheet_col_0 pad_l_12 bold left
            string currentSport = string.Empty;
            string currentLeague = string.Empty;
            string currentDate = string.Empty;

            foreach (HtmlElement tr in document.GetElementsByTagName("div"))
            {
                if (tr.OuterHtml.Trim().StartsWith("<DIV class=\"col ext "))
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
                        item i = new item(mainHeader);

                        i.AddString(elemets[0]);
                        i.AddString(elemets[1]);
                        i.AddString(elemets[2]);
                        i.AddString(elemets[3]);
                        i.AddString(elemets[4]);
                        i.AddString(elemets[5]);
                        i.AddString(currentLeague);
                        i.AddString(currentDate);
                        i.AddString(currentSport);

                        newItems.Add(i);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (tr.OuterHtml.Trim().StartsWith("<DIV class=\"bg_grey border\">")
                    || tr.OuterHtml.Trim().StartsWith("<DIV class=\"bg_othergrey border\">"))
                {
                    List<string> elemets = new List<string>();
                    foreach (HtmlElement td in tr.GetElementsByTagName("div"))
                    {
                        if (td.GetElementsByTagName("div").Count == 0)
                        {
                            elemets.Add(td.InnerText != null ? td.InnerText.Trim() : null);
                        }
                    }

                    {
                        continue;
                    }
                }
                else if (tr.OuterHtml.Trim().Contains("onmouseover=\"tip('To ") && tr.GetElementsByTagName("div").Count == 0)
                {
                    List<string> elemets = new List<string>();
                    foreach (HtmlElement td in tr.GetElementsByTagName("a"))
                    {
                        if (!string.IsNullOrEmpty(td.InnerText))
                        {
                            currentSport = td.InnerText;
                        }
                    }
                }
                else if (tr.OuterHtml.Trim().StartsWith("<DIV class=\"col_10"))
                {
                    if (!string.IsNullOrEmpty(tr.InnerText))
                    {
                        currentDate = tr.InnerText;
                    }
                }
                else if (tr.OuterHtml.Trim().StartsWith("<DIV class=\"sheet_col_0 pad_l_12 bold left"))
                {
                    if (!string.IsNullOrEmpty(tr.InnerText))
                    {
                        currentLeague = tr.InnerText;
                    }
                }
            }

            int returnTrue = 0;
        }
    }
}
