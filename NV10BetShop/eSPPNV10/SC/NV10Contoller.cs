using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Timers;

namespace eSSPNV10.SC
{
    public delegate void ReceivedEvent(NV10PollEvent inEvent);
    public delegate void ThrowedException(Exception inException);

    public class NV10Controller
    {
        private SerialPort _port;
        private string _portName;
        private byte _deviceID;
        private uint _deviceSerial;
        public int m_NumberOfChannels = 0;
        public int m_ValueMultiplier = 0;
        public int m_RealValueMultiplier = 0;
        public List<ChannelData> m_UnitDataList;
        public string _country = "";
        public char m_UnitType = '-';
        public int _protocol;
        public bool _ignoreRealValueMultiplyer = false;
        public bool _encryptSSP = false;

        private static Byte SYNC_BYTE = 0x7F;
        private static Int32 NV10_TIMEOUT_READ = 1000;
        private static Int32 NV10_TIMEOUT_WRITE = 1000;
        internal static readonly Int32 SSP_POLL_TMO = 210;

        private Boolean _seq = true;
        private readonly Object _sendMonitor = new Object();
        private Boolean _stopFlag = true;
        public bool _debug;

        private readonly ReceivedEvent _eventReceiver;
        private readonly ThrowedException _exceptionThrower;
        private readonly Timer _tmoPoll = new Timer(SSP_POLL_TMO);
        private NV10CryptManager _crypter = new NV10CryptManager();
        //private string logPath = @"C:\Users\Sale\Desktop\NV10Controller.log";
        private string _logPath = @"C:\Users\Sale\Desktop\NV10Controller.log";

        public NV10Controller(string port, byte deviceID, ReceivedEvent inDelegate, ThrowedException inException, string logPath, bool debug, string appVersion, bool encryptSSP)
        {
            _encryptSSP = encryptSSP;
            _debug = debug;
            _deviceID = deviceID;
            _portName = port;
            _eventReceiver = inDelegate;
            _exceptionThrower = inException;

            _tmoPoll.AutoReset = false;
            _tmoPoll.Elapsed += new System.Timers.ElapsedEventHandler(_tmoPoll_Elapsed);

            m_UnitDataList = new List<ChannelData>();
            _logPath = logPath;

            logFile("Initialised controller, application version: " + appVersion);

        }

        public void initPort()
        {
            try
            {
                _port = new SerialPort(_portName, 9600, Parity.None, 8, StopBits.Two);
                _port.ReadTimeout = NV10_TIMEOUT_READ;
                _port.WriteTimeout = NV10_TIMEOUT_WRITE;
                _port.RtsEnable = false;
                _port.Handshake = Handshake.None;
                _port.DiscardNull = false;
                _port.DtrEnable = false;
                _port.Open();
            }
            catch (Exception ex)
            {
                logFile("Exception " + ex.GetType().Name + ", " + ex.Message);
                logFile(ex.StackTrace);
                ThrowException(ex);
            }
        }

        public void closePort()
        {
            logFile("Closing port!");
            try
            {
                if (_port == null)
                {
                    _port = new SerialPort(_portName, 9600, Parity.None, 8, StopBits.Two);
                }
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Dispose();
            }
            catch (Exception ex)
            {
                logFile("Exception " + ex.GetType().Name + ", " + ex.Message);
                logFile(ex.StackTrace);
                ThrowException(ex);
            }
        }

        public void logFile(string message)
        {
            string logFile = Path.Combine(_logPath, "CashAcceptor." + DateTime.Now.ToString("yyyy.MM.dd") + ".log");
            string messageForSend = "<" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffffffK") + "> " + message;
            StreamWriter sr = File.AppendText(logFile);
            sr.WriteLine(messageForSend);
            sr.Flush();
            sr.Close();
            sr.Dispose();
        }

        public Boolean isPolling
        {
            get
            {
                return !_stopFlag;
            }
        }

        public void DeInit()
        {
            StopPoll();
            closePort();
        }

        public void ExchangeNewKeys()
        {
            if (_crypter != null)
            {
                _crypter.Inited = false;
            }
            if (!_encryptSSP)
            {
                return;
            }

            try
            {
                //2. Generator
                logFile("Set generator start");
                _crypter = new NV10CryptManager();
                _crypter.Inited = false;
                NV10Request setgeneratorReq = NV10Request.NV10RequestTemplates.NV10SetGenerator;
                setgeneratorReq.AddSecurityBytes(_crypter.Generator.ToByteArray());
                NV10Response res = SendData(setgeneratorReq);
                if (!res.IsOK)
                {
                    logFile("Set generator error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    throw new Exception("SSP slave answer invalid: generator");
                }
                logFile("Set generator done");

                //3. Modulus
                logFile("Set modulus start");
                NV10Request setModulusReq = NV10Request.NV10RequestTemplates.NV10SetModulus;
                setModulusReq.AddSecurityBytes(_crypter.Modulus.ToByteArray());
                res = SendData(setModulusReq);
                if (!res.IsOK)
                {
                    logFile("Set Modulus error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    throw new Exception("SSP slave answer invalid: modulus");
                }
                logFile("Set modulus done");

                //4. Exchange key
                logFile("Set exchange key start");
                NV10Request setKeyReq = NV10Request.NV10RequestTemplates.NV10SetExchangeKey;
                setKeyReq.AddSecurityBytes(_crypter.HostExchangeKey.ToByteArray());
                res = SendData(setKeyReq);
                if (!res.IsOK)
                {
                    logFile("Set exchange key error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    throw new Exception("SSP slave answer invalid: exchange key");
                }
                logFile("Res data: " + res._data);
                logFile("Res dataLen: " + res._dataLength);
                logFile("Res deviceID: " + res._deviceID);
                logFile("Res _type: " + res._type.ToString());
                _crypter.SolveSynchKey(res.getSecurityData());
                System.Threading.Thread.Sleep(2000);
                _crypter.Inited = true;
                logFile("Set exchange key done");
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
        }

        public void Init()
        {
            try
            {
                logFile("Init start");

                closePort();
                initPort();

                _seq = true;

                //1. Sync
                logFile("Send sync");
                NV10Response res = SendData(NV10Request.NV10RequestTemplates.NV10SyncRequest);
                if (!res.IsOK)
                {
                    logFile("Sync error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    throw new Exception("SSP slave answer invalid: not synched");
                }
                logFile("Sync done");
                
                if (!_encryptSSP)
                {
                    ExchangeNewKeys();
                }

                //5. GetSerial
                logFile("Get serial start");
                NV10Response reply = SendData(NV10Request.NV10RequestTemplates.NV10GetSerial);
                //logFile("Get Serial error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                _deviceSerial = reply.Serial;
                logFile("Get serial done");

                //6. getSetup
                logFile("Get setup start");
                NV10Response setupReply = SendData(NV10Request.NV10RequestTemplates.NV10GetSetup);
                List<Byte> bl = new List<Byte>();
                bl.Add(0x7f);
                bl.AddRange(setupReply._data);
                ParseSetupResponse(bl.ToArray());
                logFile("Get setup done");


                //7. ChannelValues
                if (_protocol < 6)
                {
                    logFile("Channel values start");
                    NV10Response channelValueReply = SendData(NV10Request.NV10RequestTemplates.NV10ChannelValues);
                    if (!channelValueReply.IsOK)
                    {
                        logFile("Sync error, deviceID: " + channelValueReply._deviceID + ", response type: " + channelValueReply._type + ", response data " + channelValueReply.getDataString());
                        throw new Exception("SSP slave answer invalid: not synched");
                    }
                    ParseChannelValues(channelValueReply._data);
                    logFile("Channel values done");
                }

                //8. Inhibits
                logFile("Inhibits start");
                NV10Request inhibitsReq = NV10Request.NV10RequestTemplates.NV10SetChannelInhibits;
                inhibitsReq.AddDataBytes(new Byte[] { 0xff, 0xff });
                res = SendData(inhibitsReq);
                if (!res.IsOK)
                {
                    logFile("Set channel inhibits error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    throw new Exception("SSP slave answer invalid: set channel inhibits");
                }
                logFile("Inhibits done");

                //9. Enable
                logFile("Enable start");
                res = SendData(NV10Request.NV10RequestTemplates.NV10Enable);
                if (!res.IsOK)
                {
                    logFile("Enable error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    throw new Exception("SSP slave answer invalid: not enabled");
                }
                logFile("Enable done");
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
        }

        private void ParseChannelValues(byte[] channelData)
        {
            logFile("Protocol 1-5 Channel values");
            int noChannels = channelData[0];
            m_UnitDataList.Clear();
            for (int index = 1; index <= noChannels; index++)
            {
                int channelValue = channelData[index];
                ChannelData d = new ChannelData();
                d.Channel = (byte)(index);
                d.Currency = _country;
                d.Level = 0;
                d.Recycling = false;
                if (!_ignoreRealValueMultiplyer)
                {
                    d.Value = channelValue * m_ValueMultiplier * m_RealValueMultiplier;
                }
                else
                {
                    d.Value = channelValue * m_ValueMultiplier * 100;
                }
                m_UnitDataList.Add(d);
                logFile("Channel ID: " + d.Channel + ", value: " + d.Value + " " +_country);
            }
        }

        public void ParseSetupResponse(byte[] setupData)
        {
            string displayString = "Unit Type: ";
            int index = 1;

            // unit type (table 0-1)
            m_UnitType = (char)setupData[index++];

            string logMsg = "";
            switch (m_UnitType)
            {
                case (char)0x00: logMsg = "Validator"; break;
                case (char)0x03: logMsg += "SMART Hopper"; break;
                case (char)0x06: logMsg += "SMART Payout"; break;
                case (char)0x07: logMsg += "NV11"; break;
                default: logMsg += "Unknown Type"; break;
            }
            displayString += logMsg;
            logFile("Unit type: " + logMsg);

            displayString += "\r\nFirmware: ";

            // firmware (table 2-5)
            logMsg = "";
            while (index <= 5)
            {
                logMsg += (char)setupData[index++];
                if (index == 4)
                    logMsg += ".";
            }
            displayString += logMsg;
            logFile("Firmware type: " + logMsg);

            // country code (table 6-8)
            List<byte> byteCurr = new List<byte>();
            byteCurr.Add(setupData[index++]);
            byteCurr.Add(setupData[index++]);
            byteCurr.Add(setupData[index++]);
            _country = Encoding.ASCII.GetString(byteCurr.ToArray());
            logFile("Country: " + _country);

            // value multiplier (table 9-11) 
            byteCurr = new List<byte>();
            int value = 0;
            value = setupData[index++];
            value = value << 8;
            value = setupData[index++];
            value = value << 8;
            value = setupData[index++];
            m_ValueMultiplier = value;
            logFile("Value multiplier: " + m_ValueMultiplier);
            if (m_ValueMultiplier == 0)
            {
                m_ValueMultiplier = 1;
                logFile("Value multiplier set to 1, protocol 6 or higher expected!");
            }


            displayString += "\r\nNumber of Channels: ";
            int numChannels = setupData[index++];
            m_NumberOfChannels = numChannels;
            logFile("Channel number: " + m_NumberOfChannels);

            displayString += numChannels + "\r\n";
            // channel values (table 13 to 13+n)
            // the channel values located here in the table are legacy, protocol 6+ provides a set of expanded
            // channel values.
            index = 13 + m_NumberOfChannels; // Skip channel values

            // channel security (table 13+n to 13+(n*2))
            // channel security values are also legacy code
            index = 13 + (m_NumberOfChannels * 2); // Skip channel security

            displayString += "Real Value Multiplier: ";

            // real value multiplier (table 13+(n*2) to 15+(n*2))
            // (big endian)
            m_RealValueMultiplier = setupData[index + 2];
            m_RealValueMultiplier += setupData[index + 1] << 8;
            m_RealValueMultiplier += setupData[index] << 16;
            logFile("Real value multiplyer: " + m_RealValueMultiplier);
            if (m_RealValueMultiplier == 0)
            {
                _ignoreRealValueMultiplyer = true;
                m_RealValueMultiplier = 100;
                logFile("Real value multiplyer ZERO, forcing "+m_RealValueMultiplier+"!");
            }
            else
            {
                _ignoreRealValueMultiplyer = false;
            }

            displayString += m_ValueMultiplier + "\r\nProtocol Version: ";
            index += 3;


            // protocol version (table 16+(n*2))
            index = 16 + (m_NumberOfChannels * 2);
            int protocol = setupData[index++];
            _protocol = protocol;
            displayString += protocol + "\r\n";
            logFile("Protocol version: " + _protocol);


            //*****************************************
            //not execute always:
            // protocol 6+ only
            //else use get channel values
            //*****************************************
            if (_protocol >= 6)
            {

                // channel currency country code (table 17+(n*2) to 17+(n*5))
                index = 17 + (m_NumberOfChannels * 2);
                int sectionEnd = 17 + (m_NumberOfChannels * 5);
                int count = 0;
                byte[] channelCurrencyTemp = new byte[3 * m_NumberOfChannels];
                while (index < sectionEnd)
                {
                    displayString += "Channel " + ((count / 3) + 1) + ", currency: ";
                    channelCurrencyTemp[count] = setupData[index++];
                    displayString += (char)channelCurrencyTemp[count++];
                    channelCurrencyTemp[count] = setupData[index++];
                    displayString += (char)channelCurrencyTemp[count++];
                    channelCurrencyTemp[count] = setupData[index++];
                    displayString += (char)channelCurrencyTemp[count++];
                    displayString += "\r\n";
                }

                // expanded channel values (table 17+(n*5) to 17+(n*9))
                index = sectionEnd;
                displayString += "Expanded channel values:\r\n";
                sectionEnd = 17 + (m_NumberOfChannels * 9);
                int n = 0;
                count = 0;
                int[] channelValuesTemp = new int[m_NumberOfChannels];
                while (index < sectionEnd)
                {
                    n = BitConverter.ToInt32(setupData, index);
                    channelValuesTemp[count] = n;
                    index += 4;
                    displayString += "Channel " + ++count + ", value = " + n + "\r\n";
                }

                logFile("Protocol 6+ Channel values");
                // Create list entry for each channel
                m_UnitDataList.Clear(); // clear old table
                for (byte i = 0; i < m_NumberOfChannels; i++)
                {
                    ChannelData d = new ChannelData();
                    d.Channel = i;
                    d.Channel++; // Offset from array index by 1
                    d.Value = channelValuesTemp[i] * m_ValueMultiplier * m_RealValueMultiplier;
                    d.Currency = "" + (char)channelCurrencyTemp[0 + (i * 3)] + (char)channelCurrencyTemp[1 + (i * 3)] + (char)channelCurrencyTemp[2 + (i * 3)];
                    d.Level = 0; // Can't store notes 
                    d.Recycling = false; // Can't recycle notes
                    logFile("Channel ID: " + d.Channel + ", value: " + (d.Value) + " " + d.Currency);
                    m_UnitDataList.Add(d);
                }

                // Sort the list of data by the value.
                m_UnitDataList.Sort(delegate(ChannelData d1, ChannelData d2) { return d1.Value.CompareTo(d2.Value); });
            }
        }

        public void Reset()
        {
            try
            {
                logFile("Reset call");
                NV10Response res = SendData(NV10Request.NV10RequestTemplates.NV10Reset);
                if (!res.IsOK)
                {
                    logFile("Reset error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    ThrowException(new Exception("SSP slave answer invalid: " + res._type.ToString() + " - not enabled"));
                }
                System.Threading.Thread.Sleep(5000);
                logFile("Reset done");
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
        }

        public void Enable()
        {
            try
            {
                logFile("Enable call");
                NV10Response res = SendData(NV10Request.NV10RequestTemplates.NV10Enable);
                if (!res.IsOK)
                {
                    logFile("Enable error, deviceID: " + res._deviceID + ", response type: " + res._type + ", response data " + res.getDataString());
                    ThrowException(new Exception("SSP slave answer invalid: " + res._type.ToString() + " - not enabled"));
                }
                logFile("Enable done");
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }

        }

        public void StartPoll()
        {
            logFile("Start poll call");
            if (!_stopFlag)
            {
                return;
            }
            _stopFlag = false;
            Poll();
            logFile("Start poll done");
        }

        public void StopPoll()
        {
            logFile("Stop poll call");
            _stopFlag = true;
            logFile("Stop poll done");
        }

        public NV10PollEvent Hold()
        {
            NV10Response replyData;
            NV10PollEvent evt = new NV10PollEvent(NV10PollEvent.NV10PollEventType.Disabled, 0);
            if (System.Threading.Monitor.TryEnter(_sendMonitor, 0))
            {
                try
                {
                    replyData = SendData(NV10Request.NV10RequestTemplates.NV10Hold);
                    evt = NV10PollEvent.Parse(replyData);
                    return evt;
                }
                finally
                {
                    System.Threading.Monitor.Exit(_sendMonitor);
                }
            }
            return evt;            
        }

        public NV10PollEvent RejectNote()
        {
            NV10Response replyData;
            NV10PollEvent evt = new NV10PollEvent(NV10PollEvent.NV10PollEventType.Disabled, 0);
            if (System.Threading.Monitor.TryEnter(_sendMonitor, 0))
            {
                try
                {
                    replyData = SendData(NV10Request.NV10RequestTemplates.NV10RejectNote);
                    evt = NV10PollEvent.Parse(replyData);
                    return evt;
                }
                finally
                {
                    System.Threading.Monitor.Exit(_sendMonitor);
                }
            }
            return evt;
        }

        private void Poll()
        {
            try
            {
                if (_debug)
                {
                    logFile("Poll call");
                }
                _tmoPoll.Stop();
                if (_stopFlag)
                {
                    return;
                }
                NV10Response replyData;
                if (System.Threading.Monitor.TryEnter(_sendMonitor, 0))
                {
                    try
                    {
                        replyData = SendData(NV10Request.NV10RequestTemplates.NV10Poll);
                        if (replyData.IsOK)
                        {
                            NV10PollEvent evt = NV10PollEvent.Parse(replyData);
                            if (_debug)
                            {
                                logFile("Event: " + evt.EventType.ToString() + ", throwing deviceID: " + replyData._deviceID + ", response type: " + replyData._type + ", response data " + replyData.getDataString());
                            }
                            ThrowEvent(evt);
                        }
                        else
                        {
                            if (_debug)
                            {
                                logFile("Poll error, deviceID: " + replyData._deviceID + ", response type: " + replyData._type + ", response data " + replyData.getDataString());
                            }
                            throw new Exception("Error, NV10 returned message type: " + replyData._type.ToString());
                        }
                    }
                    catch (Exception exc)
                    {
                        ThrowException(exc);
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(_sendMonitor);
                    }
                }
                _tmoPoll.Start();
                if (_debug)
                {
                    logFile("Poll done");
                }
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }

        }

        void _tmoPoll_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Poll();
        }

        private void ThrowEvent(NV10PollEvent inEvent)
        {
            if (_eventReceiver != null)
            {
                System.Threading.Thread thr = new System.Threading.Thread(delegate(Object inState)
                {
                    _eventReceiver(inEvent);
                });
                thr.Start();
            }
        }

        private void ThrowException(Exception inException)
        {
            if (_exceptionThrower != null)
            {
                System.Threading.Thread thr = new System.Threading.Thread(delegate(Object inState)
                {
                    _exceptionThrower(inException);
                });
                thr.Start();
            }
        }

        internal NV10Response SendData(NV10Request inData)
        {
            lock (_sendMonitor)
            {
                NV10Response res = null;
                if (_debug)
                {
                    logFile("Message data plain to send: " + byteArrToHexString(inData.getSendBytes(_seq)));
                }
                if (_crypter.Inited)
                {
                    inData.setNewDataBytes(_crypter.Crypt(inData.getBytesForCrypting()));
                    if (_debug)
                    {
                        logFile("Message data encrypted to send: " + byteArrToHexString(inData.getSendBytes(_seq)));
                    }
                    Byte[] response = SendData(inData.getSendBytes(_seq));
                    if (_debug)
                    {
                        logFile("Response data received encrypted: " + byteArrToHexString(response));
                    }
                    Byte[] forDec = new Byte[response[1]];
                    Array.Copy(response, 2, forDec, 0, forDec.Length);

                    Byte[] decRes = _crypter.Decrypt(forDec);

                    //Byte[] decryptedResp = new Byte[2+decRes.Length];
                    //Array.Copy(response, 0, decryptedResp, 0, 2);
                    //Array.Copy(decRes, 0, decryptedResp, 2, decRes.Length);

                    Byte[] decryptedResp = new Byte[2 + decRes[0]];
                    Array.Copy(response, 0, decryptedResp, 0, 1);
                    Array.Copy(decRes, 0, decryptedResp, 1, 1);
                    Array.Copy(decRes, 5, decryptedResp, 2, decRes[0]);

                    res = new NV10Response(decryptedResp);
                    if (_debug)
                    {
                        logFile("Response data received decrypted: " + byteArrToHexString(res._data));
                    }
                }
                else
                {
                    Byte[] response = SendData(inData.getSendBytes(_seq));
                    res = new NV10Response(response);
                    if (_debug)
                    {
                        logFile("Response data received decrypted: " + byteArrToHexString(res._data));
                    }
                }
                return res;
            }
        }

        internal Byte[] SendData(Byte[] inData)
        {
            lock (_sendMonitor)
            {
                //stuff bytes for message send
                //if data contains SYNC_BYTE, repeat it twice
                List<Byte> result = new List<Byte>(10);
                for (int i = 0; i < inData.Length; i++)
                {
                    if (i>0 && inData[i] == SYNC_BYTE)
                    {
                        result.Add(inData[i]);
                        result.Add(inData[i]);
                    }
                    else
                    {
                        if (i > 0)
                        {
                            result.Add(inData[i]);
                        }
                    }
                }
                Byte[] cmdStuffed = result.ToArray();
                //Byte[] cmdStuffed = SSPTransportStuff.InsertByteStuff(inCmd.RawData);
                Byte[] data2send = new Byte[cmdStuffed.Length + 1];
                data2send[0] = SYNC_BYTE;
                cmdStuffed.CopyTo(data2send, 1);
                if (_debug)
                {
                    logFile("Bytes on wire: " + byteArrToHexString(data2send));
                }
                _port.Write(data2send, 0, data2send.Length);
                
                Byte[] res = ReceiveData();

                if (!checkAddress(res))
                {
                    logFile(String.Format("deviceID wrong data: {0:X}, expected {1:X}", (res[0] & 0x7f), _deviceID));
                    throw new Exception("SSP slave answer invalid: address");
                }

                _seq = !_seq;
                return res;
            }
        }



        private Byte[] ReceiveData()
        {

            int readedByte = 0;
            bool sync = false;
            Boolean packetReceived = false;
            List<Byte> result = new List<Byte>(10);
            while (!packetReceived)
            {
                readedByte = _port.ReadByte();
                if (readedByte == -1)
                {
                    break;
                }
                if (readedByte == SYNC_BYTE)
                {
                    sync = true;
                }
                else if (sync)
                {
                    result.Add((Byte)readedByte);
                    readedByte = _port.ReadByte();
                    if (readedByte == -1)
                    {
                        break;
                    }
                    result.Add((Byte)readedByte);
                    //CRC LEN
                    Int32 waitCounter = result[1] + 2;
                    if (readedByte == SYNC_BYTE)
                    {
                        waitCounter++;
                    }

                    Boolean syncPacket = false;
                    packetReceived = true;
                    while (waitCounter > 0)
                    {

                        readedByte = _port.ReadByte();

                        if (readedByte == -1)
                        {
                            break;
                        }
                        result.Add((Byte)readedByte);
                        if (readedByte == SYNC_BYTE)
                        {
                            if (!syncPacket)
                            {
                                syncPacket = true;
                                waitCounter++;
                            }
                            else
                            {
                                syncPacket = false;
                            }
                        }
                        waitCounter--;
                    }
                }
            }
            if (!packetReceived)
            {
                return null;
            }
            Byte[] resultwoStuff = null; // SSPTransportStuff.DeleteByteStuff(result.ToArray());
            List<Byte> resultClear = new List<Byte>(10);
            Boolean added = false;
            Byte[] inData = result.ToArray();
            if (_debug)
            {
                logFile("Bytes from wire: " + byteArrToHexString(inData));
            }

            for (int i = 0; i < inData.Length; i++)
            {
                if ((i > 0) &&
                    (inData[i] == SYNC_BYTE) &&
                    (inData[i - 1] == SYNC_BYTE) &&
                    (!added))
                {
                    added = true;
                    continue;
                }
                else
                {
                    resultClear.Add(inData[i]);
                    added = false;
                }
            }
            resultwoStuff = resultClear.ToArray();

            if (!NV10CRCTools.IsCRCValid(resultwoStuff))
            {
                logFile("Wrong CRC data!");
                throw new Exception("crc");
            }

            _port.ReadExisting();

            return resultwoStuff;
        }


        private Boolean checkAddress(Byte[] response)
        {
            if (response.Length >= 2)
            {
                return (response[0] & 0x7f) == _deviceID;
            }
            else
            {
                return false;
            }
        }

        private static String GetTime()
        {
            return DateTime.Now.ToString("ss.fff");
        }

        private string byteArrToHexString(Byte[] _data)
        {
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

    }
}
