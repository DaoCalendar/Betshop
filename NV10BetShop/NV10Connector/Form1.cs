using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Diagnostics;
using eSSPNV10.SC;
using eSSPNV10;

namespace NV10Connector
{
    public partial class MainForm : Form
    {
        private eSSPNV10.SC.NV10Controller _nv10sc;
        private BetShopATMConnector betShopATM;
        private string fileVersion = "";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();
            cbxCOMPort.Items.Clear();
            for (int i = 0; i < portNames.Length; i++)
            {
                cbxCOMPort.Items.Add(portNames[i]);
            }
            
            cbxDeviceID.Items.Clear();
            for (int i = 0; i < 20; i++)
            {
                cbxDeviceID.Items.Add("" + i);
            }

            betShopATM = new BetShopATMConnector(Application.StartupPath);
            if (betShopATM.debug)
            {
                cbDebug.Checked = true;
            }
            else
            {
                cbDebug.Checked = false;
            }
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            fileVersion = version.ToString();
            lblAppVer.Text = fileVersion;
            _nv10sc = new eSSPNV10.SC.NV10Controller(betShopATM.port, 0, GetResponseData, GetResponseData, Application.StartupPath, betShopATM.debug, fileVersion);
            _nv10sc.encryptSSP = betShopATM.encryptSSP;
            if (betShopATM.autoRun)
            {
                if (betShopATM.delay > 0)
                {
                    System.Threading.Thread.Sleep(betShopATM.delay * 1000);
                }
                btnInitNV10_Click(this, null);
                _nv10sc.StartPoll();
            }
        }

        private void btnInitNV10_Click(object sender, EventArgs e)
        {
            timer1.Interval = 5000;            
            _nv10sc.Init();
            timer1.Start();
        }

        public int GetChannelValue(int channelNum)
        {
            if (channelNum >= 1 && channelNum <= _nv10sc.m_NumberOfChannels)
            {
                foreach (ChannelData d in _nv10sc.m_UnitDataList)
                {
                    if (d.Channel == channelNum)
                        return d.Value;
                }
            }
            return -1;
        }

        static public string FormatToCurrency(int unformattedNumber)
        {
            float f = unformattedNumber * 0.01f;
            return f.ToString("0.00");
        }


        public void GetResponseData(NV10PollEvent response)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    GetResponseData(response);
                });
                return;
            }
            if (response.EventType != NV10PollEvent.NV10PollEventType.Ok)
            {
                LogWrite("Poll response: " + response.EventType + (NV10PollEvent.dataEvents.Contains(response.EventType) ? ", Data: " + response.EventData.ToString() : ""));
            }
            if (response.EventType == NV10PollEvent.NV10PollEventType.CreditNote)
            {
                int noteVal = GetChannelValue(response.EventData);
                betShopATM.AppendText("Credit " + FormatToCurrency(noteVal) + "\r\n", noteVal);
            }
            else if(response.EventType == NV10PollEvent.NV10PollEventType.Disabled)
            {
                _nv10sc.Enable();
            }
        }

        public void GetResponseData(Exception exc)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    GetResponseData(exc);
                });
                return;
            }
            _nv10sc.StopPoll();
            LogWrite("Poll exception: " + exc.GetType().Name + ", " + exc.Message);
            LogWrite("Source: ");
            LogWrite(exc.Source);
            LogWrite("Stacktrace: ");
            LogWrite(exc.StackTrace);
        }

        public void LogWrite(string message)
        {
            if (textBox1.Text.Length > 102400)
            {
                textBox1.Text = "";
            }
            if (textBox1.Text.Length > 10240)
            {
                textBox1.Text = "";
            }
            textBox1.AppendText(message + "\r\n");
            string fileName = Path.Combine(Application.StartupPath, "NV10ControllerExceptionLog.log");
            string messageForSend = "<" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffffffK") + "> " + message;
            StreamWriter sr = File.AppendText(fileName);
            sr.WriteLine(messageForSend);
            sr.Flush();
            sr.Close();
            sr.Dispose();
        }

        public void LogWriteSynched(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    LogWriteSynched(message);
                });
                return;
            }
            if (textBox1.Text.Length > 10240)
            {
                textBox1.Text = "";
            }
            textBox1.AppendText(message + "\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_nv10sc.isPolling)
            {
                _nv10sc.StopPoll();
            }
            else
            {
                _nv10sc.StartPoll();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _nv10sc.StopPoll();
            _nv10sc.Reset();
            _nv10sc.Init();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            bool wasPolling = false;
            if (_nv10sc.isPolling)
            {
                wasPolling = true;
                _nv10sc.StopPoll();
            }
            System.Threading.Thread.Sleep(250);           
            _nv10sc.ExchangeNewKeys();
            if (wasPolling)
            {
                _nv10sc.StartPoll();
            }
            timer1.Start();
        }

        private void cbDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDebug.Checked)
            {
                if (betShopATM != null)
                {
                    betShopATM.debug = true;
                }
                if (_nv10sc != null)
                {
                    _nv10sc._debug = true;
                }
            }
            else
            {
                if (betShopATM != null)
                {
                    betShopATM.debug = false;
                }
                if (_nv10sc != null)
                {
                    _nv10sc._debug = false;
                }
            }
        }

        private void cbEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEncrypt.Checked)
            {
                if (betShopATM != null)
                {
                    betShopATM.encryptSSP = true;
                }
                if (_nv10sc != null)
                {
                    _nv10sc.encryptSSP = true;
                }
            }
            else
            {
                if (betShopATM != null)
                {
                    betShopATM.encryptSSP = false;
                }
                if (_nv10sc != null)
                {
                    _nv10sc.encryptSSP = false;
                }
            }
        }

        private void btnTestServer_Click(object sender, EventArgs e)
        {
            string text = "Test cash push";
            double amount = 100.0;
            betShopATM.AppendText(text, amount);
        }


    }
}
