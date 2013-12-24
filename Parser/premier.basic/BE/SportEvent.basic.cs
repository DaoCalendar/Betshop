using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.parser
{
    public partial class SportEvent
    {
        public List<SportGame> Games { get; set; }

        public SportEvent()
        {
            Games = new List<SportGame>();
        }

        public void Update(SportEvent se)
        {
            Time = se.Time;

            Games = new List<SportGame>();
            Games.AddRange(se.Games);
        }
    }
}
