using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.parser
{
    public class SportEvent
    {
        public string Id { get; set; }

        public string Sport { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string League { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }

        public List<SportGame> Games { get; set; }

        public SportEvent()
        {
            Games = new List<SportGame>();
        }
    }
}
