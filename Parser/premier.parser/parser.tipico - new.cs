using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier.parser
{
    public delegate void Update(string message, string id);

    public class tipico
    {
        private List<List<string>> activeEvents;

        private bool htmlReadCompleted;

        Dictionary<header, List<item>> data;

        public bool ReadCompleted { get { return htmlReadCompleted; } }
        public Dictionary<header, List<item>> Data { get { return data; } }

        public tipico() 
        {
            activeEvents = new List<List<string>>();
            data = null;

            htmlReadCompleted = false;
        }

        public void PrepareHtml(HtmlDocument document)
        {
            string topMatchHomeScore = null;
            string topMatchAwayScore = null;

            foreach (HtmlElement div in document.GetElementsByTagName("div"))
            {
                if (!string.IsNullOrEmpty(div.InnerText))
                {
                    string check = div.OuterHtml.Trim().ToLower();

                    // if (check.StartsWith("<div class=\"row head_2"))
                    if (check.StartsWith("<div class=\"jq-header\">")
                        || check.StartsWith("<div class=jq-header>")
                        || check.StartsWith("<div class=\"row head_2")
                        || check.StartsWith("<div id=_program_conference_upcomingheader_interval")
                        || check.StartsWith("<div class=\"row cf event-row jq-event-row")
                        || check.StartsWith("<div id=_program_conference_runningevent_id")
                        || check.StartsWith("<div id=comp-head-")
                        || check.StartsWith("<div id=top-event-")
                        // || check.StartsWith("<div id=_program_conference_top")
                        || check.StartsWith("<div class=\"row cf event-row odd"))
                    {
                        string sport = null;

                        string redCardHome = null;
                        string redCardAway = null;

                        List<string> elemets = new List<string>();
                        List<string> elemetsCheck = new List<string>();

                        if (check.Contains("halftime - bet can only be"))
                            continue;

                        foreach (HtmlElement td in div.All)
                        {
                            if (!td.OuterHtml.Trim().ToLower().StartsWith("<span"))
                            {
                                elemets.Add(td.InnerText != null ? td.InnerText.Trim().Replace("Soccer", "Football") : null);
                                elemetsCheck.Add(td.OuterHtml);

                                if (string.IsNullOrEmpty(sport))
                                {
                                    if (td.OuterHtml.Contains("onmouseover=\"tip('"))
                                    {
                                        int startIndex = td.OuterHtml.IndexOf("onmouseover=\"tip('") + "onmouseover=\"tip('".Length;
                                        int endIndex = td.OuterHtml.IndexOfAny(new char[] { ' ', '\"', '\'', '>' }, startIndex);
                                        sport = td.OuterHtml.Substring(startIndex, endIndex - startIndex + 1);
                                        if (sport == "Soccer")
                                            sport = "Football";
                                    }
                                }
                                if (td.OuterHtml.TrimStart().StartsWith("<DIV class=\"team  redcard "))
                                {
                                    string redCard = td.OuterHtml.TrimStart().Substring(34, 1);
                                    if (string.IsNullOrEmpty(redCardHome)) redCardHome = redCard;
                                    else if (string.IsNullOrEmpty(redCardAway)) redCardAway = redCard;
                                }
                            }
                        }

                        if (check.StartsWith("<div id=top-event-"))
                        {
                            if (elemets.Count > 4 && elemets[4] != null && elemets[4].Contains("-"))
                            {
                                topMatchHomeScore = elemets[4].Split(new char[] { '-' })[0];
                                topMatchAwayScore = elemets[4].Split(new char[] { '-' })[1];
                            }
                            else
                                continue;
                        }
                        else
                        {
                            if (!check.StartsWith("<div id=comp-head-") && !string.IsNullOrEmpty(topMatchHomeScore) && !string.IsNullOrEmpty(topMatchAwayScore))
                            {
                                elemets.Add(topMatchHomeScore);
                                elemets.Add(topMatchAwayScore);

                                topMatchHomeScore = topMatchAwayScore = null;
                            }

                            if (!string.IsNullOrEmpty(sport))
                            {
                                elemets.Add(sport);
                            }

                            elemets.Add(redCardHome);
                            elemets.Add(redCardAway);

                            if (elemets.Count != 0)
                            {
                                if (string.IsNullOrEmpty(elemets[0]))
                                    for (int i = 0; i < 3; ++i)
                                        if (!elemets[0].StartsWith("Next"))
                                            elemets.RemoveAt(0);

                                activeEvents.Add(elemets);
                            }
                        }
                    }
                }
            }

            htmlReadCompleted = true;
        }

        public void ParseData()
        {
            //try
            {
                header activeHeader = null;
                data = new Dictionary<header, List<item>>();

                item currentItem = null;

                foreach (List<string> elemets in activeEvents)
                {
                    if (elemets.Count == 32 && elemets[0] != null && elemets[0].Contains("Match odds") && !string.IsNullOrEmpty(elemets[2]))
                    {
                        activeHeader = new header(elemets[2]);

                        activeHeader.Add("Time");
                        activeHeader.Add("Home team");
                        activeHeader.Add("Home team score");
                        activeHeader.Add("Away team score");
                        activeHeader.Add("Away team");

                        if (!string.IsNullOrEmpty(elemets[3]))
                        {
                            activeHeader.Add(elemets[3] + " " + elemets[15]);
                            activeHeader.Add(elemets[3] + " " + elemets[16]);
                            activeHeader.Add(elemets[3] + " " + elemets[17]);
                        }
                        if (!string.IsNullOrEmpty(elemets[5]))
                        {
                            activeHeader.Add(elemets[5] + " " + elemets[19]);
                            activeHeader.Add(elemets[5] + " " + elemets[20]);
                            activeHeader.Add(elemets[5] + " " + elemets[21]);
                        }
                        if (!string.IsNullOrEmpty(elemets[7]))
                        {
                            activeHeader.Add(elemets[7] + " " + elemets[23]);
                            activeHeader.Add(elemets[7] + " " + elemets[24]);
                            activeHeader.Add(elemets[7] + " " + elemets[25]);
                        }
                        if (!string.IsNullOrEmpty(elemets[9]))
                        {
                            activeHeader.Add(elemets[9] + " " + elemets[27]);
                            activeHeader.Add(elemets[9] + " " + elemets[28]);
                            activeHeader.Add(elemets[9] + " " + elemets[29]);
                        }

                        if (!data.ContainsKey(activeHeader))
                        {
                            activeHeader.Add("RedCardsHome");
                            activeHeader.Add("RedCardsAway");
                            activeHeader.Add("Started");
                            activeHeader.Add("Suspicious");
                            data.Add(activeHeader, new List<item>());
                        }
                    }
                    else if (elemets.Count == 29 && elemets[0] != null && elemets[0].Contains("Match odds") && !string.IsNullOrEmpty(elemets[2]))
                    {
                        activeHeader = new header(elemets[2]);

                        activeHeader.Add("Time");
                        activeHeader.Add("Home team");
                        activeHeader.Add("Home team score");
                        activeHeader.Add("Away team score");
                        activeHeader.Add("Away team");

                        if (!string.IsNullOrEmpty(elemets[3]))
                        {
                            activeHeader.Add(elemets[3] + " " + elemets[14]);
                            activeHeader.Add(elemets[3]);
                            activeHeader.Add(elemets[3] + " " + elemets[16]);
                        }
                        if (!string.IsNullOrEmpty(elemets[5]))
                        {
                            activeHeader.Add(elemets[5] + " " + elemets[18]);
                            activeHeader.Add(elemets[5]);
                            activeHeader.Add(elemets[5] + " " + elemets[20]);
                        }
                        if (!string.IsNullOrEmpty(elemets[7]))
                        {
                            activeHeader.Add(elemets[7] + " home");
                            activeHeader.Add(elemets[7]);
                            activeHeader.Add(elemets[7] + " away");
                        }
                        if (!string.IsNullOrEmpty(elemets[8]))
                        {
                            activeHeader.Add(elemets[8] + " " + elemets[24]);
                            activeHeader.Add(elemets[8] + " " + elemets[25]);
                            activeHeader.Add(elemets[8] + " " + elemets[26]);
                        }

                        if (!data.ContainsKey(activeHeader))
                        {
                            activeHeader.Add("RedCardsHome");
                            activeHeader.Add("RedCardsAway");
                            activeHeader.Add("Started");
                            activeHeader.Add("Suspicious");
                            data.Add(activeHeader, new List<item>());
                        }
                    }
                    else if (elemets[0] != null && elemets[0].StartsWith("Next"))
                    {
                        activeHeader = new header(elemets[0]);

                        activeHeader.Add("Time");
                        activeHeader.Add("Sport");
                        activeHeader.Add("Home team");
                        activeHeader.Add("Away team");

                        if (!string.IsNullOrEmpty(elemets[1]))
                        {
                            activeHeader.Add(elemets[1] + " 1");
                            activeHeader.Add(elemets[1] + " X");
                            activeHeader.Add(elemets[1] + " 2");
                        }
                        if (!string.IsNullOrEmpty(elemets[3]))
                        {
                            activeHeader.Add(elemets[3] + " 1");
                            activeHeader.Add(elemets[3] + " X");
                            activeHeader.Add(elemets[3] + " 2");
                        }
                        if (!string.IsNullOrEmpty(elemets[5]))
                        {
                            activeHeader.Add(elemets[5]);
                            activeHeader.Add(elemets[5] + " +");
                            activeHeader.Add(elemets[5] + " -");
                        }
                        else
                        {
                            int breakHere = 1;
                        }
                        if (!data.ContainsKey(activeHeader))
                        {
                            activeHeader.Add("RedCardsHome");
                            activeHeader.Add("RedCardsAway");
                            activeHeader.Add("Started");
                            activeHeader.Add("Suspicious");
                            data.Add(activeHeader, new List<item>());
                        }
                    }
                    else if (elemets.Count == 26 && elemets[0] != null && elemets[0].Contains("Match odds") && !string.IsNullOrEmpty(elemets[0]))
                    {
                        activeHeader = new header(elemets[2]);

                        activeHeader.Add("Time");
                        activeHeader.Add("Sport");
                        activeHeader.Add("Home team");
                        activeHeader.Add("Home team score");
                        activeHeader.Add("Away team score");
                        activeHeader.Add("Away team");

                        if (!string.IsNullOrEmpty(elemets[3]))
                        {
                            activeHeader.Add(elemets[3] + " " + elemets[13]);
                            activeHeader.Add(elemets[3] + " " + elemets[14]);
                            activeHeader.Add(elemets[3] + " " + elemets[15]);
                        }
                        if (!string.IsNullOrEmpty(elemets[5]))
                        {
                            activeHeader.Add(elemets[5] + " " + elemets[17]);
                            activeHeader.Add(elemets[5] + " " + elemets[18]);
                            activeHeader.Add(elemets[5] + " " + elemets[19]);
                        }
                        if (!string.IsNullOrEmpty(elemets[7]))
                        {
                            activeHeader.Add(elemets[7] + " " + elemets[21]);
                            activeHeader.Add(elemets[7] + " " + elemets[22]);
                            activeHeader.Add(elemets[7] + " " + elemets[23]);
                        }
                        else
                        {
                            int breakHere = 1;
                        }
                        if (!data.ContainsKey(activeHeader))
                        {
                            activeHeader.Add("RedCardsHome");
                            activeHeader.Add("RedCardsAway");
                            activeHeader.Add("Started");
                            activeHeader.Add("Suspicious");
                            data.Add(activeHeader, new List<item>());
                        }
                    }
                    else if (!string.IsNullOrEmpty(elemets[0]) && elemets[0].Contains("Match odds"))
                    {
                        activeHeader = new header(elemets[2]);

                        activeHeader.Add("Time");
                        activeHeader.Add("Sport");
                        activeHeader.Add("Home team");
                        activeHeader.Add("Away team");

                        if (!string.IsNullOrEmpty(elemets[3]))
                        {
                            activeHeader.Add(elemets[3] + " 1");
                            activeHeader.Add(elemets[3] + " X");
                            activeHeader.Add(elemets[3] + " 2");
                        }
                        if (!string.IsNullOrEmpty(elemets[5]))
                        {
                            activeHeader.Add(elemets[5] + " 1");
                            activeHeader.Add(elemets[5] + " X");
                            activeHeader.Add(elemets[5] + " 2");
                        }
                        if (!string.IsNullOrEmpty(elemets[7]))
                        {
                            activeHeader.Add(elemets[7]);
                            activeHeader.Add(elemets[7] + " +");
                            activeHeader.Add(elemets[7] + " -");
                        }
                        else
                        {
                            int breakHere = 1;
                        }
                        if (!data.ContainsKey(activeHeader))
                        {
                            activeHeader.Add("RedCardsHome");
                            activeHeader.Add("RedCardsAway");
                            activeHeader.Add("Started");
                            activeHeader.Add("Suspicious");
                            data.Add(activeHeader, new List<item>());
                        }
                    }
                    else if (elemets.Count > 19 && activeHeader != null && activeHeader.Event.StartsWith("Next"))
                    {
                        item item = new item(activeHeader);

                        while (string.IsNullOrEmpty(elemets[1]))
                            elemets.RemoveAt(1);

                        item.AddString(elemets[0]);
                        item.AddString(elemets[elemets.Count - 3]);
                        item.AddString(elemets[2]);
                        item.AddString(elemets[5]);
                        item.AddString(elemets[7]);
                        item.AddString(elemets[8]);
                        item.AddString(elemets[9]);
                        item.AddString(elemets[11]);
                        item.AddString(elemets[12]);
                        item.AddString(elemets[13]);
                        item.AddString(elemets[15]);
                        item.AddString(elemets[16]);
                        item.AddString(elemets[17]);
                        item.AddString(elemets[elemets.Count - 2]);
                        item.AddString(elemets[elemets.Count - 1]);
                        item.AddString("false");
                        item.AddString("false");

                        data[activeHeader].Add(item);

                        if (item.Data.Count != activeHeader.Data.Count)
                        {
                            int breakHere = 1;
                        }

                        currentItem = item;
                    }
                    else if (elemets.Count > 22 && elemets[3] != null && elemets[3].Contains(":"))
                    {
                        item item = new item(activeHeader);

                        while (string.IsNullOrEmpty(elemets[1]))
                            elemets.RemoveAt(1);

                        item.AddString(elemets[0]);
                        item.AddString(elemets[2]);
                        item.AddString(elemets[3].Split(new char[] { ':' })[0]);
                        item.AddString(elemets[3].Split(new char[] { ':' })[1]);
                        item.AddString(elemets[4]);
                        item.AddString(elemets[6]);
                        item.AddString(elemets[7]);
                        item.AddString(elemets[8]);
                        item.AddString(elemets[10]);
                        item.AddString(elemets[11]);
                        item.AddString(elemets[12]);
                        if (!string.IsNullOrEmpty(elemets[15]) && elemets[15].Contains(":") && string.IsNullOrEmpty(elemets[14]) && string.IsNullOrEmpty(elemets[16]))
                        {
                            elemets[14] = elemets[15].Split(new char[] { ':' })[0].Trim();
                            elemets[16] = elemets[15].Split(new char[] { ':' })[1].Trim();
                        }
                        item.AddString(elemets[14]);
                        item.AddString(elemets[15]);
                        item.AddString(elemets[16]);
                        item.AddString(elemets[18]);
                        item.AddString(elemets[19]);
                        item.AddString(elemets[20]);
                        item.AddString(elemets[elemets.Count - 2]);
                        item.AddString(elemets[elemets.Count - 1]);
                        item.AddString("true");
                        item.AddString("false");

                        if (elemets.Count > 39)
                        {
                            item firstHalf = new item(activeHeader);
                            item.FirstHalf = firstHalf;

                            int counter = 0;
                            while (!activeHeader[counter].StartsWith("Match"))
                            {
                                firstHalf.AddString(item[counter]);
                                counter++;
                            }

                            firstHalf.AddString(elemets[25]);
                            firstHalf.AddString(elemets[26]);
                            firstHalf.AddString(elemets[27]);
                            firstHalf.AddString(elemets[29]);
                            firstHalf.AddString(elemets[30]);
                            firstHalf.AddString(elemets[31]);
                            firstHalf.AddString(elemets[33]);
                            firstHalf.AddString(elemets[34]);
                            firstHalf.AddString(elemets[35]);
                            firstHalf.AddString(elemets[37]);
                            firstHalf.AddString(elemets[38]);
                            firstHalf.AddString(elemets[39]);
                            firstHalf.AddString(null);
                            firstHalf.AddString(null);
                            firstHalf.AddString("true");
                            firstHalf.AddString("false");

                            int breakHere = 1;
                        }

                        data[activeHeader].Add(item);

                        if (item.Data.Count != activeHeader.Data.Count)
                        {
                            int breakHere = 1;
                        }

                        currentItem = item;
                    }
                    else if (elemets.Count > 19 && activeHeader != null)
                    {
                        item item = new item(activeHeader);

                        string time = elemets[1];
                        if (!time.Contains("+") && time.Contains("'"))
                            time = time.Substring(0, time.IndexOf('\'') + 1);

                        item.AddString(time); // minutes.ToString());

                        item.AddString(activeHeader.Event);

                        item.AddString(elemets[3]);

                        item.AddString(elemets[elemets.Count - 5]);
                        item.AddString(elemets[elemets.Count - 4]);

                        item.AddString(elemets[5]);

                        item.AddString(elemets[7]);
                        item.AddString(elemets[8]);
                        item.AddString(elemets[9]);

                        item.AddString(elemets[11]);
                        item.AddString(elemets[12]);
                        item.AddString(elemets[13]);

                        item.AddString(elemets[15]);
                        item.AddString(elemets[16]);
                        item.AddString(elemets[17]);

                        item.AddString(elemets[elemets.Count - 2]);
                        item.AddString(elemets[elemets.Count - 1]);
                        item.AddString("true");
                        item.AddString("false");

                        if (elemets.Count > 33 && elemets[21] == "1st half")
                        {
                            item firstHalf = new item(activeHeader);
                            item.FirstHalf = firstHalf;

                            int counter = 0;
                            while (!activeHeader[counter].StartsWith("Match"))
                            {
                                firstHalf.AddString(item[counter]);
                                counter++;
                            }

                            firstHalf.AddString(elemets[23]);
                            firstHalf.AddString(elemets[24]);
                            firstHalf.AddString(elemets[25]);

                            firstHalf.AddString(elemets[27]);
                            firstHalf.AddString(elemets[28]);
                            firstHalf.AddString(elemets[29]);

                            firstHalf.AddString(elemets[31]);
                            firstHalf.AddString(elemets[32]);
                            firstHalf.AddString(elemets[33]);

                            firstHalf.AddString(null);
                            firstHalf.AddString(null);
                            firstHalf.AddString("true");
                            firstHalf.AddString("false");

                            int breakHere = 1;
                        }
                        currentItem = item;
                        data[activeHeader].Add(item);
                    }
                    else
                    {
                        Console.WriteLine("!(activeHeader != null && elemets.Count >= 7)");
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex.Message);
            //}
        }


        Dictionary<string, string> listOfIds;
        string token;
        Update sendUpdate;
        Dictionary<header, List<item>> itemCatalog; 

        public void InitAll(Dictionary<header, List<item>> itemCatalog, Update sendUpdate, string token, Dictionary<string, string> listOfIds)
        {
            this.itemCatalog = itemCatalog;
            this.sendUpdate = sendUpdate;
            this.token = token;
            this.listOfIds = listOfIds;
        }

        public void DoAll()
        {
            ParseData();
            CompareData();
        }

        public void CompareData()
        {
            List<item> allItems = new List<item>();

            foreach (header h in itemCatalog.Keys)
            {
                allItems.AddRange(itemCatalog[h]);
            }

            foreach (header h in data.Keys)
            {
                if (!itemCatalog.ContainsKey(h))
                {
                    itemCatalog.Add(h, new List<item>());
                }

                List<item> items = itemCatalog[h];

                foreach (item newItem in data[h])
                {
                    bool found = false;
                    foreach (item oldItem in items)
                    {
                        if (oldItem.IsSameEvent(newItem))
                        {
                            found = true;

                            string update = oldItem.Update(newItem, token);
                            if (!string.IsNullOrEmpty(oldItem.Id) && !string.IsNullOrEmpty(update))
                            {
                                sendUpdate(update, oldItem.Id);
                            }

                            if (allItems.Contains(oldItem))
                            {
                                allItems.Remove(oldItem);
                            }

                            break;
                        }
                    }

                    if (!found)
                    {
                        items.Add(newItem);

                        if (listOfIds.ContainsValue(newItem.ToString()))
                        {
                            foreach (string id in listOfIds.Keys)
                            {
                                if (newItem.ToString() == listOfIds[id])
                                {
                                    newItem.Id = id;
                                    sendUpdate(newItem.JSON(token), newItem.Id);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (item i in allItems)
            {
                if (itemCatalog.ContainsKey(i.Header) && itemCatalog[i.Header].Contains(i))
                {
                    itemCatalog[i.Header].Remove(i);
                    if (!string.IsNullOrEmpty(i.Id))
                        sendUpdate(i.Removed(token), i.Id);
                }
            }
        }
    }
}
