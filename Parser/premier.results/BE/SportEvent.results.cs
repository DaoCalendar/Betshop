using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.parser
{
    public partial class SportEvent
    {
        public string Status { get; set; }
        public string WinningOdds { get; set; }

        public void Update(SportEvent se)
        {
            Time = se.Time;
            Status = se.Status;
            WinningOdds = se.WinningOdds;
        }
    }
}
