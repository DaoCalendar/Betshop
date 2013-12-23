using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eSSPNV10
{
    public enum SSPNV10PollEvents
    {
        SlaveReset = 0xF1,
        ReadNote = 0xEF,
        CreditNote = 0xEE,
        NoteRejecting = 0xED,
        NoteRejected = 0xEC,
        NoteStacking = 0xCC,
        NoteStacked = 0xEB,
        SafeNoteJam = 0xEA,
        UnsafeNoteJam = 0xE9,
        Disabled = 0xE8,
        FraudAttempt = 0xE6,
        StackerFull = 0xE7,
        NoteClearedFromFront = 0xE1,
        NoteClearedToCashBox = 0xE2,
        NotePathOpen = 0xE0,
        ChannelDisable = 0xB5,
        Initialising = 0xB6
    }

/*
    public enum SSPSmartHopperPollEvents
    {
        Dispensing = 0xDA,
        Dispensed = 0xD2,
        LidOpen = 0x81,
        LidClosed = 0x82,
        CalibrationFail = 0x83,
        Jammed = 0xD5,
        Halted = 0xD6,
        Floating = 0xD7,
        Floated = 0xD8,
        TimeOut = 0xD9,
        IncompletePayout = 0xDC,
        IncompleteFloat = 0xDD,
        Emptying = 0xC2,
        Empty = 0xC3,
        CashBoxPaid = 0xDE,
        CoinCredit = 0xDF,
        CoinMechJammed = 0xC4,
        CoinMechReturnPressed = 0xC5,
        FraudAttempt = 0xE6,
        Disabled = 0xE8,
        LowPayoutLevel = 0xB2,
        SmartEmptying = 0xB3,
        SmartEmptied = 0xB4,
        SlaveReset = 0xF1
    }
*/
    public enum SSPSmartHopperCalibErr
    {
        NoFailture = 0x00,
        OpticalSensorFlap = 0x01,
        OticalSensorExit = 0x02,
        CoilSensor1 = 0x03,
        CoilSensor2 = 0x04,
        UintNotInitialised = 0x05,
        DataChecksumErr = 0x06,
        RecalibRequired = 0x07
    }

    public enum SSPPayoutErr
    {
        None= 0x00,
        NotEnoughValue = 0x01,
        CantPayExactValue = 0x02,
        HopperBusy = 0x03,
        HopperDisabled = 0x04
    }

    public class SSPReply
    {
        private List<Byte> _data = new List<Byte>(1);
        public SSPReply(Byte[] inData)
        {
            _data.AddRange(inData);
        }

        public SSPDataTypes Type
        {
            get
            {
                return (SSPDataTypes)_data[0];
            }
        }

        public Byte[] RawData
        {
            get
            {
                return _data.ToArray();
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
    
    public class SSPSimpleReply : SSPReply
    {
        public SSPSimpleReply(Byte[] inData) : base(inData) { }
    }


    public class SSPPayoutReply : SSPReply
    {
        public SSPPayoutErr Error
        {
            get;
            private set;
        }
        public SSPPayoutReply(SSPPayoutErr inError, Byte[] inData)
            : base(inData)
        {
            Error = inError;
        }
    }

    public class SSPPollReply : SSPReply
    {
        SSPNV10PollEvents _event;
        public SSPPollReply(SSPNV10PollEvents inEvent, Byte[] inData)
            : base(inData)
        {
            _event = inEvent;
        }

        public SSPNV10PollEvents Event
        {
            get
            {
                return _event;
            }
        }
    }

    //public class SSPCalibrationReply : SSPPollReply
    //{
    //    private SSPSmartHopperCalibErr _error;

    //    public SSPCalibrationReply(SSPSmartHopperCalibErr inError, Byte[] inData)
    //        : base(SSPNV10PollEvents.CalibrationFail, inData)
    //    {
    //        _error = inError;
    //    }

    //    public SSPSmartHopperCalibErr Error
    //    {
    //        get
    //        {
    //            return _error;
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return String.Format("Calibration fail: {0}\n", Error);
    //    }
    //}

    public class SSPNoteReply : SSPReply
    {
        private uint _channel;
        private SSPNV10PollEvents _event;
        public SSPNoteReply(uint channel, SSPNV10PollEvents inEvent, Byte[] data)
            : base(data)
        {
            _channel = channel;
            _event = inEvent;
        }

        public uint Channel
        {
            get
            {
                return _channel;
            }
        }

        public SSPNV10PollEvents Event
        {
            get
            {
                return _event;
            }
        }
    }

    public class SSPCoinsReply : SSPReply
    {
        private List<SSPCoinsOperation> _operations = new List<SSPCoinsOperation>(1);

        public SSPCoinsReply(List<SSPCoinsOperation> inOperations, Byte[] inData)
            : base(inData)
        {
            _operations.AddRange(inOperations);
        }

        public SSPCoinsOperation[] Operations
        {
            get
            {
                return _operations.ToArray();
            }
        }

        //public SSPCoinsOperation GetEmptied()
        //{
        //    return GetCoinOperation(SSPNV10PollEvents.SmartEmptied);
        //}

        //public SSPCoinsOperation GetDispenced()
        //{
        //    return GetCoinOperation(SSPNV10PollEvents.Dispensed);
        //}

        //public SSPCoinsOperation GetTimeOut()
        //{
        //    return GetCoinOperation(SSPNV10PollEvents.TimeOut);
        //}

        //public SSPCoinsOperation GetCashBoxPaid()
        //{
        //    return GetCoinOperation(SSPNV10PollEvents.CashBoxPaid);
        //}

        //private SSPCoinsOperation GetCoinOperation(SSPNV10PollEvents ev)
        //{
        //    for (int i = 0; i < _operations.Count; i++)
        //    {
        //        if (_operations[i].Event == ev)
        //        {
        //            return _operations[i];
        //        }
        //    }
        //    return null;
        //}

        //public SSPCoinsOperation GetDispencing()
        //{
        //    return GetCoinOperation(SSPNV10PollEvents.Dispensing);
        //}

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(20);
            for (int i = 0; i < _operations.Count; i++)
            {
                for (int j = 0; j < _operations[i].Coins.Length; j++)
                {
                    builder.Append(String.Format("{0} {1}\n", _operations[i].Event, _operations[i].Coins[j].Value));
                }
            }
            return builder.ToString();
        }
    }

    public class SSPCoinsOperation
    {
        private SSPNV10PollEvents _event;
        private readonly List<SSPMoneyContext> _money = new List<SSPMoneyContext>(1);

        public SSPCoinsOperation(SSPNV10PollEvents inEvent, SSPMoneyContext[] inCoins)
        {
            _event = inEvent;
            if (inCoins != null)
            {
                _money.AddRange(inCoins);
            }
        }


        public SSPNV10PollEvents Event
        {
            get
            {
                return _event;
            }
        }

        public SSPMoneyContext[] Coins
        {
            get
            {
                return _money.ToArray();
            }
        }
    }

    internal class SSPSetupReply : SSPReply
    {
        public Byte UnitType
        {
            get;
            private set;
        }

        private Byte[] _firmware;
        public Byte[] Firmware
        {
            get
            {
                return (Byte[])_firmware.Clone();
            }
            private set
            {
                _firmware = (Byte[])value.Clone();
            }
        }

        private Byte[] _country;
        public Byte[] MainCountry
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

        public Byte Protocol
        {
            get;
            private set;
        }

        private List<SSPMoneyContext> _coins = new List<SSPMoneyContext>(6);
        public SSPMoneyContext[] Coins
        {
            get
            {
                return _coins.ToArray();
            }
        }

        public SSPSetupReply(Byte inUnitType,
                                Byte[] inFirmware,
                                Byte[] inMainCountry,
                                Byte inProtocol,
                                List<SSPMoneyContext> inCoins,
                                Byte[] inData)
            : base(inData)
        {
            UnitType = inUnitType;
            Firmware = inFirmware;
            MainCountry = inMainCountry;

            Protocol = inProtocol;

            _coins.AddRange(inCoins);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(6 * (3 + 1 + 3));
            builder.Append("Coin\tCode\n");
            for (int i = 0; i < _coins.Count; i++)
            {
                builder.AppendFormat("{0}\t{1}\n", _coins[i].Value, Encoding.ASCII.GetString(_coins[i].CountryCode));
            }
            return builder.ToString();
        }

        public Boolean IsValid
        {
            get{
                Boolean result = true;
                SSPCoin test;
                try
                {
                    for (int i = 0; i < _coins.Count; i++)
                    {
                        test = new SSPCoin(_coins[i].Value);
                    }
                }
                catch (ArgumentException)
                {
                    result = false;
                }
                return result;
            }
        }
    }

    internal class SSPAmountReply : SSPReply
    {
        public UInt16 Amount
        {
            get;
            private set;
        }

        public SSPAmountReply(UInt16 inAmount, Byte[] inData)
            : base(inData)
        {
            Amount = inAmount;
        }
    }

    internal class SSPSetRoutingReply : SSPReply
    {
        public SSPRoute Route
        {
            get;
            private set;
        }

        public SSPSetRoutingReply(SSPRoute inRoute, Byte[] inData)
            : base(inData)
        {
            Route = inRoute;
        }
    }

    internal class SSPGetSerialReply : SSPReply
    {
        public UInt32 Serial
        {
            get;
            private set;
        }

        public SSPGetSerialReply(UInt32 inSerial, Byte[] inData)
            : base(inData)
        {
            Serial = inSerial;
        }
    }
}
