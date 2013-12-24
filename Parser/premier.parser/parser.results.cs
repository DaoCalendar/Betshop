using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier.parser
{
    public class results
    {
        public void ParseHtml(HtmlDocument document, string sport, Dictionary<header, List<item>> itemCatalog, Dictionary<string, string> listOfIds, Update sendUpdate)
        {
            header mainHeader = new header(sport);
            mainHeader.Add("sport");
            mainHeader.Add("league");
            mainHeader.Add("time");
            mainHeader.Add("home");
            mainHeader.Add("away");
            mainHeader.Add("status");
            mainHeader.Add("odds");

            List<item> newItems = new List<item>();

            string currentLeague = string.Empty;
            foreach (HtmlElement tr in document.GetElementsByTagName("div"))
            {
                if (tr.OuterHtml.Trim().StartsWith("<DIV class=\"col "))
                {
                    List<string> elemets = new List<string>();
                    foreach (HtmlElement td in tr.GetElementsByTagName("div"))
                    {
                        if (td.GetElementsByTagName("div").Count == 0)
                        {
                            elemets.Add(td.InnerText != null ? td.InnerText.Trim() : null);
                        }
                    }

                    if (elemets.Count == 5 || elemets.Count == 6)
                    {
                        item i = new item(mainHeader);

                        i.AddString(sport);
                        i.AddString(currentLeague);
                        i.AddString(elemets[0]);
                        string[] teams = elemets[1].Split(new string[] { " - " }, StringSplitOptions.None);
                        if (teams.Length != 2)
                            continue;
                        i.AddString(teams[0].Trim());
                        i.AddString(teams[1].Trim());
                        i.AddString(elemets[2]);
                        i.AddString(elemets[3]);

                        newItems.Add(i);
                    }
                    else if (elemets.Count == 1)
                    {
                        currentLeague = elemets[0];
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            Dictionary<header, List<item>> data = new Dictionary<header,List<item>>();
            data.Add(mainHeader, newItems);
        //    CompareData(itemCatalog, data, listOfIds, sendUpdate);
        //}

        //public void CompareData(Dictionary<header, List<item>> itemCatalog, Dictionary<header, List<item>> data, Dictionary<string, string> listOfIds, Update sendUpdate)
        //{
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

                            string update = oldItem.Update(newItem, string.Empty);
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
                                    sendUpdate(newItem.JSON(string.Empty), newItem.Id);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //foreach (item i in allItems)
            //{
            //    if (itemCatalog.ContainsKey(i.Header) && itemCatalog[i.Header].Contains(i))
            //    {
            //        itemCatalog[i.Header].Remove(i);
            //    }
            //}
        }
    }
}
