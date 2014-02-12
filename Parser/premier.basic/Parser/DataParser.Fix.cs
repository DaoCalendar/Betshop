using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier.parser
{
    public partial class DataParser
    {
        private static string Fix(string entry)
        {
            string fixedEntry = entry;

            int startFrom = 0;
            int startIndex = fixedEntry.IndexOf('(', startFrom);
            int endIndex = fixedEntry.IndexOf(')', startFrom);

            while (startIndex != -1)
            {
                string check = fixedEntry.Substring(startIndex, endIndex - startIndex + 1);
                if (check.Contains(" "))
                {
                    fixedEntry = fixedEntry.Remove(startIndex, endIndex - startIndex + 1);
                }
                else
                {
                    startFrom = endIndex + 1;
                }
                startIndex = fixedEntry.IndexOf('(', startFrom);
                endIndex = fixedEntry.IndexOf(')', startFrom);
            }

            return fixedEntry.Replace("Soccer", "Football");
        }
    }
}
