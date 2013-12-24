using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.parser
{
    public partial class SportEvent
    {
        public static readonly string Separator = " ## ";

        public string Id { get; set; }

        public string Sport { get; set; }
        public string Subtype { get; set; }
        public string League { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }

        public override bool Equals(object obj)
        {
            SportEvent se = obj as SportEvent;

            if (se != null)
            {
                return ToString() == obj.ToString();
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Sport + Separator + League + Separator + Home + Separator + Away + Separator + Date;
        }
    }
}
