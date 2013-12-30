using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace premier.parser
{
    public class item
    {
        public string Id { get; set; }
        public header Header { get; set; }

        public item(header header)
        {
            this.Header = header;
        }

        public item FirstHalf { get; set; }

        private List<string> data = new List<string>();
        private List<string> oldData = new List<string>();

        public List<string> Data
        {
            get { return data; }
        }

        public List<string> OldData
        {
            get { return oldData; }
        }

        public void AddString(string value)
        {
            data.Add(value);
            oldData.Add(value);
        }

        public int Lenght
        {
            get { return data.Count; }
        }

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

        public string Removed(string token)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\"Id\":\"" + Id + "\"");

            sb.Append(", \"Removed\":\"true\"");
            sb.Append(", \"Token\":\"" + token + "\"");

            sb.Append("}");

            return sb.ToString().Replace('+', 'P');
        }

        public string JSON(string token)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\"Id\":\"" + Id + "\"");

            sb.Append(", \"TimeSent\":\"" + DateTime.Now.ToString("u") + "\"");
            sb.Append(", \"First\":\"true\"");

            if (!Header.Data.Contains("Sport"))
                sb.Append(", \"Sport\":\"" + Header.Event + "\"");

            sb.Append(", \"Token\":\"" + token + "\"");

            for (int i = 0; i < Lenght; i++)
            {
                sb.Append(",\"" + Header[i] + "\":\"" + data[i] + "\"");
            }
            if (FirstHalf != null)
            {
                sb.Append(",\"FirstHalf\":{");
                int counter = 0;
                while (!Header[counter].StartsWith("Match"))
                {
                    counter++;
                }

                for (int i = counter; i < FirstHalf.Lenght; i++)
                {
                    if (i != counter)
                    {
                        sb.Append(",");
                    }
                    sb.Append("\"" + FirstHalf.Header[i] + "\":\"" + FirstHalf[i] + "\"");
                }
                sb.Append("}");
            }

            sb.Append("}");
            return sb.ToString().Replace('+', 'P');
        }

        public string Update(item item, string token)
        {
            if (item.Lenght > 5 && item[5] != null && item[5].StartsWith("Penalty"))
            {
                return string.Empty;
            }

            bool somethingUpdated = false;

            StringBuilder sb = new StringBuilder();

            sb.Append("{\"Id\":\"" + Id + "\"");

            sb.Append(", \"Home team\":\"" + item[Header.HomeTeam] + "\"");
            sb.Append(", \"Away team\":\"" + item[Header.AwayTeam] + "\"");
            sb.Append(", \"TimeSent\":\"" + DateTime.Now.ToString("u") + "\"");
            sb.Append(", \"First\":\"false\"");

            sb.Append(", \"Token\":\"" + token + "\"");

            for (int i = 0; i < data.Count ; i++)
            {
                if (i != Header.HomeTeam && i != Header.AwayTeam)
                {
                    if (data[i] != item[i] || Header[i] == "Suspicious")
                    {
                        oldData[i] = data[i];
                        data[i] = item[i];

                        if (Header[i] != "Suspicious")
                            somethingUpdated = true;

                        sb.Append(",\"" + Header[i] + "\":\"" + data[i] + "\"");
                    }
                }
            }
            if (FirstHalf != null)
            {
                if (item.FirstHalf != null)
                {
                    bool fhUpdated = false;
                    bool fhHasTrueUpdate = false;
                    StringBuilder fhSB = new StringBuilder();

                    int counter = 0;
                    while (!Header[counter].StartsWith("Match"))
                    {
                        counter++;
                    }

                    for (int i = counter; i < FirstHalf.Lenght; i++)
                    {
                        if (FirstHalf[i] != item.FirstHalf[i] || FirstHalf.Header[i] == "Suspicious")
                        {
                            FirstHalf.OldData[i] = FirstHalf.Data[i];
                            FirstHalf.Data[i] = item.FirstHalf[i];

                            if (fhUpdated)
                            {
                                fhSB.Append(",");
                            }
                            else
                            {
                                fhSB.Append(",\"FirstHalf\":{");
                                fhUpdated = true;
                                if (FirstHalf.Header[i] != "Suspicious")
                                    fhHasTrueUpdate = somethingUpdated = true;
                            }
                            fhSB.Append("\"" + FirstHalf.Header[i] + "\":\"" + FirstHalf[i] + "\"");
                        }
                    }
                    if (fhUpdated)
                    {
                        fhSB.Append("}");
                    }
                    if (fhHasTrueUpdate)
                    {
                        sb.Append(fhSB.ToString());
                    }
                }
                else
                {
                    somethingUpdated = true;
                    FirstHalf = null;
                    sb.Append(",\"FirstHalf\":\"null\""); 
                }
            }
            else if (item.FirstHalf != null)
            {
                FirstHalf = item.FirstHalf;

                sb.Append(",\"FirstHalf\":{");
                for (int i = 0; i < FirstHalf.Lenght; i++)
                {
                    if (i != 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append("\"" + FirstHalf.Header[i] + "\":\"" + FirstHalf[i] + "\"");
                }
                sb.Append("}");
            }

            sb.Append("}");

            return somethingUpdated ? sb.ToString().Replace('+', 'P') : string.Empty;
        }

        public bool IsSameEvent(item i)
        {
            return (data[Header.HomeTeam] == i[Header.HomeTeam] && data[Header.AwayTeam] == i[Header.AwayTeam]);
        }

        public bool IsSameRow(item i)
        {
            return (Header.Equals(i.Header) && IsSameEvent(i));
        }

        public override string ToString()
        {
            return data[Header.HomeTeam] + " - " + data[Header.AwayTeam];
        }

        //public override int GetHashCode()
        //{
        //    return Header.GetHashCode() + data[1].GetHashCode() << 8 + data[4].GetHashCode() << 16;
        //}

        //public override bool Equals(object obj)
        //{
        //    var item = obj as item;

        //    if (item != null)
        //    {
        //        if (Header.Equals(item.Header) && data[1] == item[1] && data[4] == item[4])
        //        {
        //            return true;
        //        }
        //    }

        //    return base.Equals(obj);
        //}
    }
}
