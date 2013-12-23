using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eSSPNV10.SC
{
    public class ChannelData
    {
        public int Value;
        public byte Channel;
        public string Currency;
        public int Level;
        public bool Recycling;
        public ChannelData()
        {
            Value = 0;
            Channel = 0;
            Currency = "";
            Level = 0;
            Recycling = false;
        }
    };
}
