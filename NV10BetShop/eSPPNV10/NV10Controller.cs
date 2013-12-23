using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Timers;

namespace eSSPNV10
{
    public delegate void ReceivedEvent(SSPReply inEvent);
    public delegate void ThrowedException(Exception inException);

    public class NV10Controller
    {
        private SerialPort _port;
        private string _portName;
        private byte _deviceID;
        private uint _deviceSerial;
        private SSPSetupReply _setup;
        private string logPath = @"C:\Users\Sale\Desktop\NV10Controller.log";
        

        private static Byte SYNC_BYTE = 0x7F;
        private static Int32 NV10_TIMEOUT_READ = 1000;
        private static Int32 NV10_TIMEOUT_WRITE = 1000;
        internal static readonly Int32 SSP_POLL_TMO = 210;

        private Boolean _seq = true;
        private SSPCryptManager _crypter = new SSPCryptManager();
        private readonly Object _sendMonitor = new Object();
        private Boolean _stopFlag = true;

        private readonly ReceivedEvent _eventReceiver;
        private readonly ThrowedException _exceptionThrower;
        private readonly Timer _tmoPoll = new Timer(SSP_POLL_TMO);

        public void logFile(string message)
        {
            string messageForSend = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffffffK") + message;
            StreamWriter sr = File.AppendText(logPath);
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

        public NV10Controller(string port, byte deviceID, ReceivedEvent inDelegate, ThrowedException inException)
        {
            logFile("Constructor started");            
            _deviceID = deviceID;
            _portName = port;
            _eventReceiver = inDelegate;
            _exceptionThrower = inException;

            _port = null;

            _tmoPoll.AutoReset = false;
            _tmoPoll.Elapsed += new System.Timers.ElapsedEventHandler(_tmoPoll_Elapsed);
            logFile("Constructor done");
        }

        public void DeInit()
        {
            logFile("Deinit");
            closePort();
        }

        public void Init()
        {
            try
            {
                logFile("Init start");
                if (_port == null)
                {
                    initPort();
                }
                else
                {
                    closePort();
                    System.Threading.Thread.Sleep(2000);
                }
                _seq = true;

                //1. Sync
                SSPData res = SendData(new SSPSyncData());

                if (!res.IsOK)
                {
                    throw new Exception("SSP slave answer invalid: not synched");
                }

                _crypter = new SSPCryptManager();
                SSPData data2send;

                //2. Generator
                data2send = _crypter.GetSSPGeneratorData();
                res = SendData(data2send);

                if (!res.IsOK)
                {
                    throw new Exception("SSP slave answer invalid: generator");
                }

                //3. Modulus
                data2send = _crypter.GetSSPModulusData();
                res = SendData(data2send);
                if (!res.IsOK)
                {
                    throw new Exception("SSP slave answer invalid: modulus");
                }

                //4. Exchange key

                data2send = _crypter.GetSSPExchangeData();
                res = SendData(data2send);
                if (!res.IsOK)
                {
                    throw new Exception("SSP slave answer invalid: exchange key");
                }
                _crypter.SolveSyncKey(res.Data);
                System.Threading.Thread.Sleep(2000);
                _crypter.Inited = true;

                //5. GetSerial
                SSPGetSerialData getSerial = new SSPGetSerialData();
                SSPData dataReceived = SendData(getSerial);
                SSPReply reply = SSPReplyParser.ParseSerial(dataReceived);
                _deviceSerial = (reply as SSPGetSerialReply).Serial;

                //6. getSetup
                SSPGetSetupData getSetup = new SSPGetSetupData();
                SSPData received = SendData(getSetup);
                _setup = SSPReplyParser.ParseSetup(received) as SSPSetupReply;

                //7. Enable
                res = SendData(new SSPEnableData());
                if (!res.IsOK)
                {
                    throw new Exception("SSP slave answer invalid: not enabled");
                }
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
        }

        public void StartPoll()
        {
            if (!_stopFlag)
            {
                return;
            }
            _stopFlag = false;
            Poll();
        }

        public void StopPoll()
        {
            _stopFlag = true;
        }

        private void Poll()
        {
            _tmoPoll.Stop();
            if (_stopFlag)
            {
                return;
            }
            SSPData replyData;
            if (System.Threading.Monitor.TryEnter(_sendMonitor, 0))
            {
                try
                {
                    ThrowEvent(new SSPSimpleReply(new byte[] {(byte)SSPDataTypes.Poll }));
                    replyData = SendData(new SSPPollData());
                    SSPReply rep = SSPReplyParser.Parse(replyData);
                    //if (!((rep.RawData.Length == 1) && (rep.Type == SSPDataTypes.OK)))
                    //{
                        ThrowEvent(rep);
                    //}
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
        }

        void _tmoPoll_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Poll();
        }

        private void ThrowEvent(SSPReply inEvent)
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

        internal SSPData SendData(SSPData inData)
        {
            lock (_sendMonitor)
            {
                SSPData res;
                if (_crypter.Inited)
                {
                    SSPCommand cmd = SendData(_crypter.Crypt(inData));
                    res = _crypter.Decrypt(cmd.Data);
                    logFile("Bytes decrypted: " + byteArrToHexString(res.Data));
                }
                else
                {
                    res = SendData(inData.Data).SmileData;
                }
                return res;
            }
        }

        internal SSPCommand SendData(Byte[] inData)
        {
            lock (_sendMonitor)
            {
                SSPCommand cmd = new SSPCommand(_seq, _deviceID, inData);
                SSPCommand res = SendAndReceive(cmd);
                if (!checkAddress(res))
                {
                    throw new Exception("SSP slave answer invalid: address");
                }

                _seq = !_seq;
                return res;
            }
        }


        public SSPCommand SendAndReceive(SSPCommand inCmd)
        {
            SendCommand(inCmd);
            return ReceiveCommand();
        }

        private void SendCommand(SSPCommand inCmd)
        {
            logFile("=======================================================");
            logFile("Bytes to send: " + byteArrToHexString(inCmd.RawData));
            Byte[] cmdStuffed = SSPTransportStuff.InsertByteStuff(inCmd.RawData);
            logFile("Bytes on wire send: " + byteArrToHexString(cmdStuffed));
            Byte[] data2send = new Byte[cmdStuffed.Length + 1];
            data2send[0] = SYNC_BYTE;
            cmdStuffed.CopyTo(data2send, 1);
            _port.Write(data2send, 0, data2send.Length);

        }

        private SSPCommand ReceiveCommand()
        {

            logFile("-------------------------------------------------------");
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

            logFile("Bytes from wire receive: " + byteArrToHexString(result.ToArray()));
            Byte[] resultwoStuff = SSPTransportStuff.DeleteByteStuff(result.ToArray());
            logFile("Bytes from receive: " + byteArrToHexString(resultwoStuff));

            if (!SSPTransportStuff.IsCRCValid(resultwoStuff))
            {
                throw new Exception("crc");
            }

            _port.ReadExisting();

            return new SSPCommand(resultwoStuff);
        }


        private Boolean checkAddress(SSPCommand inCommand)
        {
            return inCommand.SlaveID == _deviceID;
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
;