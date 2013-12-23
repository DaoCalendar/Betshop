using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eSSPNV10
{
    internal static class SSPReplyParser
    {
        public static SSPReply Parse(SSPData inData)
        {
            Byte[] data = inData.Data.ToArray();
            SSPReply result = new SSPReply(data);
            if (data.Length == 1)
            {
                return new SSPSimpleReply(data);
            }

            #region OK
            if (data[0] == (Byte)SSPDataTypes.OK)
            {
                List<SSPNV10PollEvents> checkOperations = new List<SSPNV10PollEvents>()
                {
                    SSPNV10PollEvents.ReadNote,
                    SSPNV10PollEvents.CreditNote,
                    SSPNV10PollEvents.FraudAttempt,
                    SSPNV10PollEvents.NoteClearedFromFront,
                    SSPNV10PollEvents.NoteClearedToCashBox
                };

                List<SSPNV10PollEvents> checkSingle = new List<SSPNV10PollEvents>()
                {
                    SSPNV10PollEvents.SlaveReset,
                    SSPNV10PollEvents.NoteRejecting,
                    SSPNV10PollEvents.NoteRejected,
                    SSPNV10PollEvents.NoteStacking,
                    SSPNV10PollEvents.NoteStacked,
                    SSPNV10PollEvents.SafeNoteJam,
                    SSPNV10PollEvents.UnsafeNoteJam,
                    SSPNV10PollEvents.Disabled,
                    SSPNV10PollEvents.StackerFull,
                    SSPNV10PollEvents.NotePathOpen,
                    SSPNV10PollEvents.ChannelDisable,
                    SSPNV10PollEvents.Initialising
                };

                //TODO refactor
                #region 0xF0 0xF1 0xE8 ...
                if ((data[1] == 0xF1) && (data[2] == 0xE8))
                {
                    Byte[] dispData = new Byte[data.Length - 2];
                    dispData[0] = 0xF0;
					// for data.Length == 3 no copying is done
                    Array.Copy(data, 3, dispData, 1, dispData.Length - 1);
                    data = dispData;

                    if (data.Length == 1)
                    {
                        return new SSPSimpleReply(data);
                    }
                }
            	#endregion

                #region 0xF0 0xE8 ...
                if ((data[1] == 0xE8) && (data.Length > 2))
                {
                    Byte[] dispData = new Byte[data.Length - 1];
                    dispData[0] = 0xF0;
                    Array.Copy(data, 2, dispData, 1, dispData.Length - 1);
                    data = dispData;

                    if (data.Length == 1)
                    {
                        return new SSPSimpleReply(data);
                    }
                }
                #endregion

                Int32 counter = 0;
                //SSPMoneyContext currCoin;
                //List<SSPMoneyContext> coinsList;
                SSPNV10PollEvents currTypeEvent;

                #region Operations
                try
                {
                    if (checkOperations.Contains((SSPNV10PollEvents)data[counter + 1]))
                    {
                        List<SSPCoinsOperation> operList = new List<SSPCoinsOperation>();
                        SSPNoteReply reply = new SSPNoteReply(255, SSPNV10PollEvents.Initialising, new Byte[]{(byte)SSPDataTypes.Unknown});
                        while (checkOperations.Contains((SSPNV10PollEvents)data[counter + 1]))
                        {
                                counter++;
                                currTypeEvent = (SSPNV10PollEvents)data[counter];
                                // 0    1    2    3    4    5    6    7    8
                                //0xDE 0x02 0x00 0x00 0x00 0x00 0x52 0x55 0x42 0xD2 0x01 0xC8 0x00 0x00 0x00 0x52 0x55 0x42
                                uint currChannel = data[counter + 1];
                                reply = new SSPNoteReply(currChannel, currTypeEvent, new byte[] {data[counter], data[counter+1] });
                                counter+=2;
                                if (data.Length-1 <= counter)
                                {
                                    break;
                                }
                        }
                        result = reply;
                    }
                    #endregion

                    else if (checkSingle.Contains((SSPNV10PollEvents)data[counter + 1]))
                    {
                        result = new SSPPollReply((SSPNV10PollEvents)data[counter + 1], data);
                    }
                    //else if (data[counter + 1] == (Byte)SSPNV10PollEvents.CalibrationFail)
                    //{
                    //    result = new SSPCalibrationReply((SSPSmartHopperCalibErr)data[counter + 2], data);
                    //}
                    //else if (data[counter + 1] == (Byte)SSPNV10PollEvents.CoinCredit)
                    //{
                    //    currCoin = new SSPMoneyContext(BitConverter.ToUInt32(data, counter += 2),
                    //        new List<Byte>(data).GetRange(counter += 4, 3).ToArray());
                    //    result = new SSPCoinsReply(new List<SSPCoinsOperation>(1) 
                    //                                { new SSPCoinsOperation(SSPNV10PollEvents.CoinCredit, 
                    //                                        new SSPMoneyContext[] { currCoin }) }, data);
                    //}
                    else
                    {
                        result = new SSPReply(data);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion
            #region 0xF5
            else if (data[0] == 0xF5)
            {
                result = new SSPPayoutReply((SSPPayoutErr)data[1], data);
            }
            #endregion
            return result;

        }

        public static SSPSetupReply ParseSetup(SSPData inData)
        {
            if (!inData.IsOK)
            {
                throw new Exception("First byte isn't 0xF0");
            }
            List<Byte> data = new List<Byte>(inData.Data);
            Byte unitType = data[1];
            Byte[] firmware = data.GetRange(2, 4).ToArray();
            Byte[] country = data.GetRange(6, 3).ToArray();
            Byte protocol = data[9];
            Byte coinsCount = data[10];
            List<SSPMoneyContext> listContext = new List<SSPMoneyContext>(coinsCount);

            Int32 offsetCoins = 11;
            Int32 offsetCountry = offsetCoins + 2*coinsCount;

            UInt16 currValue;
            Byte[] currCountry;
            for (int i = 0; i < coinsCount; i++)
            {
                currValue = BitConverter.ToUInt16(data.ToArray(), i * 2 + offsetCoins);
                currCountry = data.GetRange(offsetCountry + i * 3, 3).ToArray();
                listContext.Add(new SSPMoneyContext(currValue, currCountry));
            }
            return new SSPSetupReply(unitType, firmware, country, protocol, listContext, inData.Data);
        }

        public static SSPAmountReply ParseAmount(SSPData inData)
        {
            if (!inData.IsOK)
            {
                throw new Exception("First byte isn't 0xF0");
            }

            UInt16 value = BitConverter.ToUInt16(inData.Data, 1);
            return new SSPAmountReply(value, inData.Data);
        }

        public static SSPSetRoutingReply ParseRouting(SSPData inData)
        {
            if (inData.IsOK)
            {
                SSPRoute route = (SSPRoute)inData.Data[1];
                return new SSPSetRoutingReply(route, inData.Data);   
            }
            else
            {
                throw new Exception("First byte err");
            }
        }

        public static SSPPayoutReply ParsePayout(SSPData inData)
        {
            List<SSPDataTypes> checkList = new List<SSPDataTypes>{
                                                    SSPDataTypes.OK,
                                                    SSPDataTypes.Unknown,
                                                    SSPDataTypes.ParamCountErr,
                                                    SSPDataTypes.ParamValErr,
                                                    SSPDataTypes.SoftErr,
                                                    SSPDataTypes.Fail,
                                                    SSPDataTypes.KeyNotSet};
            if (checkList.Contains((SSPDataTypes)inData.Data[0]))
            {
                return new SSPPayoutReply(SSPPayoutErr.None, inData.Data);
            }
            else if ((SSPDataTypes)inData.Data[0] == SSPDataTypes.CmdErr)
            {
                return new SSPPayoutReply((SSPPayoutErr)inData.Data[1], inData.Data);
            }
            else
            {
                throw new Exception("First byte err");
            }
        }

        public static SSPGetSerialReply ParseSerial(SSPData inData)
        {
            if ((inData.IsOK) && (inData.Data.Length == 5))
            {
                List<Byte> serialList = new List<Byte>(4);
                serialList.AddRange(inData.Data);
                serialList.Reverse();
                UInt32 serial = BitConverter.ToUInt32(serialList.ToArray(), 0);
                return new SSPGetSerialReply(serial, inData.Data);
            }
            else
            {
                throw new Exception("Parse err");
            }

        }
    }
}
