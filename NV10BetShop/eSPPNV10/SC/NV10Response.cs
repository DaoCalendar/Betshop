using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eSSPNV10.SC
{
    public class NV10Response
    {
        public enum NV10ResponseType
                    { 
                        NOT_SET = 0x00,
                        CHANNEL_DISABLE = 0xB5,
                        STACKING = 0xCC,
                        NOTE_PATH_OPEN = 0xE0,
                        NOTE_CLEARED_FROM_FRONT = 0xE1,
                        NOTE_CLEARED_INTO_CASHBOX = 0xE2,
                        FRAUD_ATTEMPT = 0xE6,
                        STACKER_FULL = 0xE7,
                        DISABLED = 0xE8,
                        UNSAFE_JAM = 0xE9,
                        SAFE_JAM = 0xEA,
                        STACKED = 0xEB,
                        REJECTED = 0xEC,
                        REJECTING = 0xED,
                        CREDIT = 0xEE,
                        READ = 0xEF,
                        OK = 0xF0,
                        SLAVE_RESET = 0xF1,
                        COMMAND_UNKNOWN = 0xF2, 
                        WRONG_NUMBER_OF_PARAMETERS = 0xF3, 
                        PARAMETER_OUT_OF_RANGE = 0xF4, 
                        COMMAND_CAN_NOT_BE_PROCESSED = 0xF5,
                        SOFTWARE_ERROR = 0xF6,
                        FAIL = 0xF8,
                        KEY_NOT_SET = 0xFA
                    };
        public bool IsOK
        {
            get
            {
                return true;
            }
        }

        public readonly byte _deviceID = 0;
        public readonly int _dataLength = 0;
        public readonly NV10ResponseType _type = NV10ResponseType.NOT_SET;
        public readonly Byte[] _data = new Byte[0];

        public String getDataString()
        {
            if (_data != null)
            {
                string res = "{";
                for (int i = 0; i < _data.Length; i++)
                {
                    string num = String.Format("{0:X}", _data[i]);
                    if (i == 0)
                    {
                        res += " " +num;
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

        public ulong getSecurityData()
        {
            if (_data.Length != 8)
            {
                throw new Exception("Invalid secure data");
            }
            return BitConverter.ToUInt64(_data, 0);
        }

        public uint Serial
        {
            get
            {
                if (_dataLength != 4)
                {
                    throw new Exception("Incorrect data length for getSerial");
                }
                List<Byte> serialList = new List<Byte>(4);
                serialList.AddRange(_data);
                serialList.Reverse();
                UInt32 serial = BitConverter.ToUInt32(serialList.ToArray(), 0);
                return serial;
            }
        }

        public NV10Response(Byte[] responseBytes)
        {
            _deviceID = (byte)(responseBytes[0] & 0x7F);
            _dataLength = responseBytes[1]-1;
            _type = (NV10ResponseType)responseBytes[2];
            _data = new Byte[_dataLength];
            for (int i = 0; i < _dataLength; i++)
            {
                _data[i] = responseBytes[3 + i];
            }
        }
    }
}
