using System;
using System.Collections.Generic;
using System.Text;

namespace eSSPNV10
{
    public class SSPCommand
    {
        private static Byte SSP_SEQ_OFFSET = 0;
        private static Byte SSP_LEN_OFFSET = 1;
        //private static Byte SSP_DATA_OFFSET = 2;

        private readonly List<Byte> _data = new List<Byte>(10);
        public Byte[] RawData
        {
            get
            {
                return _data.ToArray();
            }
        }

        private void Ctor(Boolean inSEQ, Byte inSlaveID, Byte[] inData)
        {
            _data.Add(0xFF);
            _data.Add(0xFF);

            SetSEQ(inSEQ);
            SetSlaveID(inSlaveID);
            SetData(inData);
        }

        public SSPCommand(Boolean inSEQ, Byte inSlaveID, SSPData inData)
        {
            Ctor(inSEQ, inSlaveID, inData.Data);
        }

        public SSPCommand(Boolean inSEQ, Byte inSlaveID, Byte[] inData)
        {
            Ctor(inSEQ, inSlaveID, inData);
        }

        public SSPCommand(Byte[] inReceivedCmd)
        {
            _data.AddRange(inReceivedCmd);
        }

        private void SetSEQ(Boolean inSEQ)
        {
            if (inSEQ)
            {
                _data[SSP_SEQ_OFFSET] |= 0x80;
            }
            else
            {
                _data[SSP_SEQ_OFFSET] &= 0xFF - 0x80;
            }
        }

        public Boolean IsSEQ
        {
            get
            {
                if (_data.Count < 5)
                {
                    throw new Exception("SSP packet is too small");
                }
                return (_data[SSP_SEQ_OFFSET] & 0x80) > 0;
            }
        }

        public Byte SlaveID
        {
            get
            {
                if (_data.Count < 5)
                {
                    throw new Exception("SSP packet is too small");
                }
                return (Byte)(_data[SSP_SEQ_OFFSET] & (0xFF - 0x80));
            }
        }

        public Byte DataLength
        {
            get
            {
                if (_data.Count < 5)
                {
                    throw new Exception("SSP packet is too small");
                }
                return _data[SSP_LEN_OFFSET];
            }
        }

        public Byte[] Data
        {
            get
            {
                if (_data.Count < 5)
                {
                    throw new Exception("SSP packet is too small");
                }

                if (DataLength < 1)
                {
                    throw new Exception("Data length is too small");
                }

                if (_data.Count - 4 != DataLength)
                {
                    throw new Exception("SSP packet is invalid");
                }

                Byte[] data = new Byte[DataLength];
                _data.CopyTo(2, data, 0, data.Length);
                return data;
            }
        }

        public SSPData SmileData
        {
            get
            {
                return new SSPData(Data);
            }
        }

        private void SetSlaveID(Byte inSlaveID)
        {
            if (inSlaveID > 0x7F)
            {
                throw new ArgumentOutOfRangeException("inSlaveID");
            }
            _data[SSP_SEQ_OFFSET] &= (Byte)(0x80 + inSlaveID);
        }

        private void SetDataLength(Byte inDataLength)
        {
            _data[SSP_LEN_OFFSET] = inDataLength;
        }

        private void SetData(Byte[] inData)
        {
            if (inData == null)
            {
                throw new ArgumentNullException("inData");
            }
            if ((inData.Length == 0) || (inData.Length > Byte.MaxValue))
            {
                throw new ArgumentOutOfRangeException("inData");
            }

            SetDataLength((Byte)inData.Length);

            _data.AddRange(inData);
            _data.AddRange(SSPTransportStuff.GetCRC(_data.ToArray()));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(40);
            for (int i = 0; i < RawData.Length; i++)
            {
                builder.AppendFormat("0x{0} ", RawData[i].ToString("X2"));
            }
            return builder.ToString();
        }
    }

}
