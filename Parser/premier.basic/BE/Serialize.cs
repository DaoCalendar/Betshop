using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.parser.serialization
{
    class Serialize
    {
        public static string CreateJSON(List<SportEvent> oList, bool markedOnly)
        {
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            IEnumerable<SportEvent> fList = markedOnly ? (from i in oList where i.Id != null select i) : oList;
            return oSerializer.Serialize(fList);
        }

        public static string CreateJSON(List<SportEvent> oList)
        {
            return CreateJSON(oList, false);
        }

        public static List<SportEvent> LoadJSON(string sJSON)
        {
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return oSerializer.Deserialize<List<SportEvent>>(sJSON);
        }
    }
}
