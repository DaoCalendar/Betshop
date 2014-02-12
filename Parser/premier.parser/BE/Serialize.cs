using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.parser.serialization
{
    class Serialize
    {
        public static void f(List<SportEvent> oList)
        {
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(oList);
        }

        public static void f2(string json)
        {
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<SportEvent> list = oSerializer.Deserialize<List<SportEvent>>(json);
        }
    }
}
