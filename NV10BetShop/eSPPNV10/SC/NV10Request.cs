using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace eSSPNV10.SC
{
    public class NV10Request
    {
        public static class NV10RequestTemplates 
        {
            public static NV10Request NV10ChannelValues
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0xE }
                    };
                }
            }

            public static NV10Request NV10SyncRequest
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x11 }
                    };
                }
            }

            public static NV10Request NV10SetGenerator 
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x4A }
                    };
                }
            }

            public static NV10Request NV10SetModulus
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x4B }
                    };
                }
            }

            public static NV10Request NV10SetExchangeKey
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x4C }
                    };
                }
            }

            public static NV10Request NV10GetSerial
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0xC }
                    };
                }
            }

            public static NV10Request NV10GetSetup
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x5 }
                    };
                }
            }

            public static NV10Request NV10Enable
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0xA }
                    };
                }
            }

            public static NV10Request NV10Poll
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x7 }
                    };
                }
            }

            public static NV10Request NV10SetChannelInhibits
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x2 }
                    };
                }
            }

            public static NV10Request NV10Reset
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x1 }
                    };
                }
            }

            public static NV10Request NV10Hold
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x18 }
                    };
                }
            }

            public static NV10Request NV10RejectNote
            {
                get
                {
                    return new NV10Request()
                    {
                        _length = 0x1,
                        _data = new Byte[] { 0x8 }
                    };
                }
            }

        }

        private Byte STX = 0x7f;
        private Byte _deviceID;
        private Byte _length;
        private Byte[] _data;
        private Byte _crcl;
        private Byte _crch;

        //public NV10Request(Byte deviceID)
        //{
        //    _deviceID = deviceID;
        //}
        
        public void setDeviceID(Byte deviceID)
        {
            _deviceID = deviceID;
        }
        
        public void setLength(Byte length)
        {
            _length = length;
        }

        public void AddDataBytes(Byte[] dataToAdd)
        {
            List<Byte> newData = new List<Byte>();
            newData.AddRange(_data);
            newData.AddRange(dataToAdd);
            _data = newData.ToArray();
        }

        public void AddSecurityBytes(Byte[] dataToAdd)
        {
            Byte[] value = new Byte[8];
            if (dataToAdd.Length <= 8)
            {
                Array.Copy(dataToAdd, 0, value, 0, dataToAdd.Length);
            }
            else
            {
                Array.Copy(dataToAdd, dataToAdd.Length - 8, value, 0, 8);
            }
            AddDataBytes(value);
        }


        public Byte getDeviceIDAndSeq(Byte deviceID, Boolean InSeq)
        {
            Byte resSeqDevID = 0;
            if (deviceID > 0x7F)
            {
                throw new ArgumentOutOfRangeException("deviceID");
            }
            if (InSeq)
            {
                resSeqDevID |= 0x80;
            }
            else
            {
                resSeqDevID &= 0xFF - 0x80;
            }
            resSeqDevID &= (Byte)(0x80 + deviceID);
            return resSeqDevID;
        }

        public void getMsgCrc(bool seq)
        {
            List<Byte> dataToCrc = new List<Byte>();
            dataToCrc.Add(getDeviceIDAndSeq(_deviceID, seq));
            dataToCrc.Add((Byte)_data.Length);
            dataToCrc.AddRange(_data);
            Byte[] crclh = NV10CRCTools.GetCRC(dataToCrc.ToArray());
            _crcl = crclh[0];
            _crch = crclh[1];
        }

        public Byte[] getSendBytes(bool seq)
        {
            getMsgCrc(seq);
            List<Byte> res = new List<Byte>();
            res.Add(STX);
            res.Add(getDeviceIDAndSeq(_deviceID, seq));
            res.Add((Byte)_data.Length);
            res.AddRange(_data);
            res.Add(_crcl);
            res.Add(_crch);
            return res.ToArray();
        }

        public String getDataSendString(bool seq)
        {
            Byte[] _data = getSendBytes(seq);
            if (_data != null)
            {
                string res = "{";
                for (int i = 0; i < _data.Length; i++)
                {
                    string num = String.Format("{0:X}", _data[i]);
                    if (i == 0)
                    {
                        res += " " + num;
                    }
                    else
                    {
                        res += ", " + num;
                    }
                }
                res += " }";
                return res;
            }
            else
            {
                return "{}";
            }
        }

        public Byte[] getBytesForCrypting()
        {
            List<Byte> res = new List<Byte>();
            res.AddRange(_data);
            return res.ToArray();
        }

        public void setNewDataBytes(Byte[] newData)
        {
            _data = newData;
        }
    }
}
