using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NV10Connector
{
    public class BetShopPushEvent
    {
        public enum PushStatus { FAILED, OK };
        public double amountPushed { get; set; }
        public PushStatus status { get; set; }
        public string messageReceived { get; set; }
    }
}
