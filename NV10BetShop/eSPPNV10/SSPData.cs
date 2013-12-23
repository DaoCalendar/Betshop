using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace eSSPNV10
{
    public enum SSPDataTypes
    {
        #region Generic
        Reset = 0x01,
        HostVersion = 0x06,
        Poll = 0x07,
        Disable = 0x09,
        Enable = 0x0A,
        Program = 0x0B,
        Serial = 0x0C,
        Sync = 0x11,
        Extension = 0x30,
        OK = 0xF0,
        Unknown = 0xF2,
        ParamCountErr = 0xF3,
        ParamValErr = 0xF4,
        CmdErr = 0xF5,
        SoftErr = 0xF6,
        Fail = 0xF8,
        KeyNotSet = 0xFA,

        SetGenerator = 0x4A,
        SetModulus = 0x4B,
        SetExchangeKey = 0x4C,
        #endregion

        #region SmartHopper
        GetSetup = 0x05,
        SetRouting = 0x3B,
        GetRouting = 0x3C,
        PayoutAmount = 0x33,
        GetCoinAmount = 0x35,
        SetCoinAmount = 0x34,
        HaltPayout = 0x38,
        FloatAmount = 0x3D,
        GetMinPayout = 0x3E,
        SetCoinMechInhibits = 0x40,
        PayoutDenomination = 0x46,
        FloatDenomination = 0x44,
        SetCalibration = 0x47,
        RunCalibration = 0x48,
        EmptyAll = 0x3F,
        SetOptions = 0x50,
        GetOptions = 0x51,
        CoinMechGlobalInhbit = 0x49,
        SmartEmpty = 0x52,
        CashboxPayoutData = 0x53,
        PollWithAck = 0x56,
        EventAck = 0x57
        #endregion
    }

    public enum SSPRoute
    {
        Recycle = 0x00,
        Cashbox = 0x01
    }

    public enum SSPCoinAcceptance
    {
        Disabled = 0x00,
        Enabled = 0x01
    }

    public enum SSPCalibrationMode
    {
        Auto = 0x00,
        Manual = 0x01
    }

    public enum SSPPayoutFunc
    {
        Test    = 0x19,
        Work = 0x58
    }

    public class SSPMoneyContext
    {
        internal static readonly Byte[] SSP_RUB_CODE = new Byte[] { (Byte)'R', (Byte)'U', (Byte)'B' };
        private readonly List<Byte> _data = new List<Byte>(4 + 3);

        public SSPMoneyContext(UInt32 inCoinValue)
        {
            Ctor(inCoinValue, SSP_RUB_CODE);
        }

        public SSPMoneyContext(UInt32 inCoinValue, Byte[] inCountryCode)
        {
            Ctor(inCoinValue, inCountryCode);
        }

        private void Ctor(UInt32 inCoinValue, Byte[] inCountryCode)
        {
            Value = inCoinValue;
            _data.AddRange(BitConverter.GetBytes(inCoinValue));
            CountryCode = inCountryCode;
            _data.AddRange(inCountryCode);
        }

        public Byte[] Data
        {
            get
            {
                return _data.ToArray();
            }
        }

        public UInt32 Value
        {
            get;
            private set;
        }

        private Byte[] _country;
        public Byte[] CountryCode
        {
            get
            {
                return (Byte[])_country.Clone();
            }
            private set
            {
                _country = (Byte[])value.Clone();
            }
        }
    }

    public class SSPCoin : SSPMoneyContext
    {
        protected static readonly List<UInt32> SSP_COIN_VALUES = new List<UInt32> { 0, 10, 50, 100, 200, 500, 1000 };

        public SSPCoin(UInt32 inCoinValue)
            : base(inCoinValue)
        {
            CheckCoinValue(inCoinValue);
        }

        public SSPCoin(UInt32 inCoinValue, Byte[] inCountryCode)
            : base(inCoinValue, inCountryCode)
        {
            CheckCoinValue(inCoinValue);
        }

        protected void CheckCoinValue(UInt32 inCoinValue)
        {
            if (!SSP_COIN_VALUES.Contains(inCoinValue))
            {
                throw new ArgumentException(String.Format("Invalid coin value: {0}", inCoinValue));
            }
        }
    }

    public class SSPCoinCount
    {
        private readonly List<Byte> _data = new List<Byte>(2 + 7);

        public SSPCoinCount(UInt16 inCoinCount, UInt32 inCoinValue)
        {
            _data.AddRange(BitConverter.GetBytes(inCoinCount));
            _data.AddRange(new SSPCoin(inCoinValue).Data);
        }

        public SSPCoinCount(UInt16 inCoinCount, SSPCoin inCoin)
        {
            _data.AddRange(BitConverter.GetBytes(inCoinCount));
            _data.AddRange(inCoin.Data);
        }

        public Byte[] Data
        {
            get
            {
                return _data.ToArray();
            }
        }
    }

    public class SSPData
    {
        protected static readonly Byte SSP_VERSION = 0x06;

        //to be used carefully
        protected static readonly UInt32 SSP_PAYOUT_MOD_CHECK_VALUE = 10;

        protected readonly List<Byte> _data = new List<Byte>(1 + 8);

        public Byte[] Data
        {
            get
            {
                return _data.ToArray();
            }
        }

        public SSPData(params Byte[] inData)
        {
            _data.AddRange(inData);
        }

        public SSPData(SSPDataTypes inType, params Byte[] inData)
        {
            _data.Add((Byte)inType);
            _data.AddRange(inData);
        }

        //protected void CheckAmount(UInt32 inAmount){
        //                if (inAmount % SSP_PAYOUT_MOD_CHECK_VALUE != 0){
        //        throw new ArgumentException(String.Format("Invalid coin amount: {0}", inAmount));
        //    }
        //}

        public Boolean IsOK
        {
            get
            {
                return _data[0] == (Byte)SSPDataTypes.OK;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(40);
            for (int i = 0; i < _data.Count; i++)
            {
                builder.AppendFormat("0x{0} ", _data[i].ToString("X2"));
            }
            return builder.ToString();
        }
    }

    public class SSPSingleData : SSPData
    {
        public SSPSingleData(SSPDataTypes inType)
        {
            _data.Add((Byte)inType);
        }

        public SSPDataTypes SSPType
        {
            get
            {
                return (SSPDataTypes)_data[0];
            }
        }
    }

    #region Generic commands
    public class SSPResetData : SSPSingleData
    {
        public SSPResetData()
            : base(SSPDataTypes.Reset)
        {

        }
    }

    public class SSPPollData : SSPSingleData
    {
        public SSPPollData()
            : base(SSPDataTypes.Poll)
        {

        }
    }

    public class SSPGetSerialData : SSPSingleData
    {
        public SSPGetSerialData()
            : base(SSPDataTypes.Serial)
        {

        }
    }

    public class SSPSyncData : SSPSingleData
    {
        public SSPSyncData()
            : base(SSPDataTypes.Sync)
        {

        }
    }

    public class SSPDisableData : SSPSingleData
    {
        public SSPDisableData()
            : base(SSPDataTypes.Disable)
        {

        }
    }

    public class SSPEnableData : SSPSingleData
    {
        public SSPEnableData()
            : base(SSPDataTypes.Enable)
        {

        }
    }

    public class SSPSetProtocolData : SSPData
    {
        public SSPSetProtocolData()
            : base(new Byte[] { (Byte)SSPDataTypes.HostVersion, SSP_VERSION })
        {

        }
    }
    #endregion

    #region Smart Hopper commands
    public class SSPGetSetupData : SSPSingleData
    {
        public SSPGetSetupData()
            : base(SSPDataTypes.GetSetup)
        {

        }
    }

    public class SSPSetRoutingData : SSPData
    {
        public SSPSetRoutingData(SSPRoute inRoute, SSPCoin inCoin)
            : base(SSPDataTypes.SetRouting, (Byte)inRoute)
        {
            _data.AddRange(inCoin.Data);
        }
    }

    public class SSPGetRoutingData : SSPData
    {
        public SSPGetRoutingData(SSPCoin inCoin)
            : base(SSPDataTypes.GetRouting)
        {
            _data.AddRange(inCoin.Data);
        }
    }

    public class SSPPayoutAmountData : SSPData
    {
        public SSPPayoutAmountData(UInt32 inAmount, SSPPayoutFunc inPayoutFunc)
            : base(SSPDataTypes.PayoutAmount)
        {
            //CheckAmount(inAmount);

            _data.AddRange(BitConverter.GetBytes(inAmount));
            _data.AddRange(SSPCoin.SSP_RUB_CODE);
            _data.Add((Byte)inPayoutFunc);
        }
    }

    public class SSPGetCoinAmountData : SSPData
    {
        public SSPGetCoinAmountData(SSPCoin inCoin)
            : base(SSPDataTypes.GetCoinAmount)
        {
            _data.AddRange(inCoin.Data);
        }

        //public SSPGetCoinAmountData(SSPMoneyContext inMoneyContext)
        //    : base(SSPDataTypes.GetCoinAmount)
        //{
        //    _data.AddRange(inMoneyContext.Data);
        //}
    }

    public class SSPSetCoinAmountData : SSPData
    {
        public SSPSetCoinAmountData(UInt16 inAmount, SSPCoin inCoin)
            : base(SSPDataTypes.SetCoinAmount)
        {
            _data.AddRange(BitConverter.GetBytes(inAmount));
            _data.AddRange(inCoin.Data);
        }
    }

    public class SSPHaltPayoutData : SSPSingleData
    {
                public SSPHaltPayoutData()
            : base(SSPDataTypes.HaltPayout)
        {

        }
    }

    public class SSPFloatAmountData : SSPData
    {
        public SSPFloatAmountData(SSPCoin inCoin, UInt32 inFloat, SSPPayoutFunc inPayoutFunc)
            : base(SSPDataTypes.FloatAmount)
        {
            //CheckAmount(inFloat);
            _data.AddRange(BitConverter.GetBytes((UInt16)inCoin.Value));
            _data.AddRange(BitConverter.GetBytes(inFloat));
            _data.AddRange(inCoin.CountryCode);
            _data.Add((Byte)inPayoutFunc);
        }
    }

    public class SSPGetMinimumPayoutData : SSPData
    {
        public SSPGetMinimumPayoutData()
            : base(SSPDataTypes.GetMinPayout)
        {
            _data.AddRange(SSPCoin.SSP_RUB_CODE);
        }
    }

    public class SSPSetCoinMechInhibitsData : SSPData
    {
        public SSPSetCoinMechInhibitsData(SSPCoinAcceptance inCoinAcc, SSPCoin inCoin)
            : base(SSPDataTypes.SetCoinMechInhibits, (Byte)inCoinAcc)
        {
            _data.AddRange(BitConverter.GetBytes((UInt16)inCoin.Value));
            _data.AddRange(inCoin.CountryCode);
        }
    }

    public class SSPPayoutDenomData : SSPData
    {
        public SSPPayoutDenomData(List<SSPCoinCount> inPayout, SSPPayoutFunc inPayoutFunc)
            : base(SSPDataTypes.PayoutDenomination)
        {
            if (inPayout.Count > 20)
            {
                throw new ArgumentOutOfRangeException("max payout denomination level requests is 20");
            }
            _data.Add((Byte)inPayout.Count);

            for (int i = 0; i < inPayout.Count; i++)
            {
                _data.AddRange(inPayout[i].Data);
            }

            _data.Add((Byte)inPayoutFunc);
        }
    }

    public class SSPFloatDenomData : SSPData
    {
        public SSPFloatDenomData(List<SSPCoinCount> inPayout, SSPPayoutFunc inPayoutFunc)
            : base(SSPDataTypes.FloatDenomination)
        {
            if (inPayout.Count > 20)
            {
                throw new ArgumentOutOfRangeException("max payout denomination level requests is 20");
            }
            _data.Add((Byte)inPayout.Count);

            for (int i = 0; i < inPayout.Count; i++)
            {
                _data.AddRange(inPayout[i].Data);
            }

            _data.Add((Byte)inPayoutFunc);
        }
    }

    public class SSPSetCalibrationData : SSPData
    {
        public SSPSetCalibrationData(SSPCalibrationMode inCalibration)
            : base(SSPDataTypes.SetCalibration, (Byte)inCalibration)
        {
        }
    }

    public class SSPRunCalibrationData : SSPSingleData
    {
                public SSPRunCalibrationData()
            : base(SSPDataTypes.RunCalibration)
        {

        }
    }

    public class SSPEmptyAllData : SSPSingleData
    {
                public SSPEmptyAllData()
            : base(SSPDataTypes.EmptyAll)
        {

        }
    }

    public class SSPSetOptionsData : SSPData
    {
        public SSPSetOptionsData(Boolean inPayMode, Boolean inLevelCheck, Boolean inHighSpeed, Boolean inCashboxPay)
            : base(SSPDataTypes.SetOptions)
        {
            // 0000 1110 
            //      |||+-Paymode        0
            //      ||+--LevelCheck     1
            //      |+---High speed     2
            //      +----CashBox        3
            Byte data = Convert.ToByte(inPayMode);
            data |= (Byte)(Convert.ToInt32(inLevelCheck)    << 1);
            data |= (Byte)(Convert.ToInt32(inHighSpeed)     << 2);
            data |= (Byte)(Convert.ToInt32(inCashboxPay)    << 3);

            _data.Add(data);
            _data.Add(0);
        }  

        public SSPSetOptionsData(Boolean inLowSpeed)
            : this(true, true, inLowSpeed, false)
        {
        }
    }

    public class SSPGetOptionsData : SSPSingleData
    {
                public SSPGetOptionsData()
            : base(SSPDataTypes.GetOptions)
        {

        }
    }

    [Obsolete("not implemented")]
    public class SSPCoinMechGlobalInhibitData : SSPData
    {
        public SSPCoinMechGlobalInhibitData()
            : base(SSPDataTypes.CoinMechGlobalInhbit)
        {
            throw new NotImplementedException();
        }
    }

    public class SSPSmartEmptyData : SSPSingleData
    {
                public SSPSmartEmptyData()
            : base(SSPDataTypes.SmartEmpty)
        {

        }
    }

    [Obsolete("not implemented")]
    public class SSPCashBoxPayoutData : SSPData
    {
        public SSPCashBoxPayoutData()
            : base(SSPDataTypes.CashboxPayoutData)
        {
            throw new NotImplementedException();
        }
    }

    public class SSPPollAckData : SSPSingleData
    {
                public SSPPollAckData()
            : base(SSPDataTypes.PollWithAck)
        {

        }
    }

    public class SSPEventAckData : SSPSingleData
    {
                public SSPEventAckData()
            : base(SSPDataTypes.EventAck)
        {

        }
    }

    //#endregion


#endregion

    #region Crypted
    public class SSPCryptedData : SSPData
    {
        private static readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private static readonly Int32 CRC_LEN = 2;
        //private readonly List<Byte> _data = new List<Byte>(16);

        public Byte[] RawData
        {
            get
            {
                return _data.ToArray();
            }
        }

        private void Ctor(Byte[] inData, Byte inRealLen, UInt32 inCounter)
        {
            if (inData == null)
            {
                throw new ArgumentNullException("inData");
            }

            _data.Add(inRealLen);
            _data.AddRange(BitConverter.GetBytes(inCounter));
            _data.AddRange(inData);

            while ((_data.Count + CRC_LEN) % 16 != 0)
            {
                _data.Add((Byte)_rnd.Next(0x00, 0xFF));
            }

            _data.AddRange(SSPTransportStuff.GetCRC(_data.ToArray()));
        }

        public SSPCryptedData(Byte[] inData, UInt32 inCounter)
            : base()
        {
            Ctor(inData,  (Byte)inData.Length,  inCounter);
        }

        public SSPCryptedData(SSPData inData, UInt32 inCounter)
            : base()
        {
            if (inData == null)
            {
                throw new ArgumentNullException("inData");
            }
            Ctor(inData.Data, (Byte)inData.Data.Length, inCounter);
        }

        public SSPCryptedData(Byte[] inReceivedData)
            : base()
        {
            _data.AddRange(inReceivedData);
            if (!SSPTransportStuff.IsCRCValid(_data.ToArray()))
            {
                throw new Exception("CRC is invalid");
            }
        }

        public UInt32 Counter
        {
            get
            {
                return BitConverter.ToUInt32(RawData, 1);
            }
        }

        public new Byte[] Data
        {
            get
            {
                Byte len = RawData[0];
                Byte[] result = new Byte[len];
                Array.Copy(RawData, 5, result, 0, len);
                return result;
            }
        }
    }
    #endregion

    #region Security commands
    public class SSPSecurityData : SSPData
    {                                               //BigEndian
        public SSPSecurityData(SSPDataTypes inType, Byte[] inData)
        {
            _data.Add((Byte)inType);

            Byte[] value = new Byte[8];
            if (inData.Length <= 8)
            {
                Array.Copy(inData, 0, value, 0, inData.Length);
            }
            else
            {
                Array.Copy(inData, inData.Length - 8, value, 0, 8);
            }
            _data.AddRange(value);
        }

        public SSPSecurityData(Byte[] inData)
        {
            _data.AddRange(inData);
        }

        public UInt64 GetSecureData()
        {
            if (_data.Count < 1 + 8)
            {
                throw new Exception("Invalid secure data");
            }
            //Byte[] secData = new Byte[8];
            //_data.CopyTo(secData, 1);
            return BitConverter.ToUInt64(_data.ToArray(), 1);
        }
    }

    public class SSPSetGeneratorData : SSPSecurityData
    {
        public SSPSetGeneratorData(Byte[] inValue)
            : base(SSPDataTypes.SetGenerator, inValue)
        {

        }
    }

    public class SSPSetModulusData : SSPSecurityData
    {
        public SSPSetModulusData(Byte[] inValue)
            : base(SSPDataTypes.SetModulus, inValue)
        {

        }
    }

    public class SSPSetExchangeKeyData : SSPSecurityData
    {
        public SSPSetExchangeKeyData(Byte[] inValue)
            : base(SSPDataTypes.SetExchangeKey, inValue)
        {

        }
    }
    #endregion

}
