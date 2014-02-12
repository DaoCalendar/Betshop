using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace premier.parser
{
    public class header
    {
        private List<string> data = new List<string>();

        public string Event { get; set; }

        public int HomeTeam { get; set; }
        public int AwayTeam { get; set; }

        public header(string e)
        {
            Event = e;
            data = new List<string>();
            // for (int i = 0; i < lenght; i++) data.Add(string.Empty);
        }

        public void Add(string s)
        {
            if (s == "Home team" || s == "home")
            {
                HomeTeam = data.Count;
            }
            if (s == "Away team" || s == "away")
            {
                AwayTeam = data.Count;
            }
            data.Add(s);
        }

        public int Lenght
        {
            get { return data.Count; }
        }

        internal List<string> Data { get { return data; } }

        public string this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public override int GetHashCode()
        {
            int hashCode = Event.GetHashCode();

            for (int i = 0; i < data.Count; i++)
            {
                hashCode += data[i].GetHashCode() << i;
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var h = obj as header;

            if (h != null && Event == h.Event && Lenght == h.Lenght)
            {
                for (int i = 0; i < Lenght; i++)
                {
                    if (this[i] != h[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }
    }
}
