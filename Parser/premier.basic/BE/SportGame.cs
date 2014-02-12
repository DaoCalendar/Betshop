using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.parser
{
    public class SportGame
    {
        public string Name { get; set; }

        public Dictionary<string, double> Quotes { get; set; }

        [System.Web.Script.Serialization.ScriptIgnore]
        public string GameQuotes
        {
            get
            {
                string quotes = string.Empty;
                foreach (string s in Quotes.Keys)
                {
                    if (!string.IsNullOrEmpty(quotes)) quotes += ";  ";
                    quotes += "\"" + s.ToString() + "\" = " + Quotes[s].ToString("f2");
                }
                return quotes;
            }
        }

        public SportGame(string name)
        {
            Name = name;
            Quotes = new Dictionary<string, double>();
        }

        public SportGame() { }
    }
}
