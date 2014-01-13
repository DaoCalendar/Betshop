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

            Int32 length = data.Count;
            for (Int32 i = 0; i < length - 6; i++) data.Add(string.Empty);
        }

        public void Add(string s)
        {
            if (s == "Home team" || s == "home")
            {
                HomeTeam = data.Count;
            }
            else if (data.Count > Lenght)
            {

                HomeTeam = data.Count - 1;

            }
            if (s == "Away team" || s == "away")
            {
                AwayTeam = data.Count;
            }

            else if (data.Count > Lenght)
            {


                AwayTeam = data.Count - 1;
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
                try
                {
                    return data[index];
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                }
                finally
                {
                    index = 16;
                   
                }
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

            for (int i = 0; i < data.Count - 1; i++)
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
