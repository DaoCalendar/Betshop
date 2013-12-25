using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using eSSPNV10.SC;

namespace NV10Connector
{
    public class BetShopATMConnector
    {
        public string serviceURL = null;
        public string client = null;
        public string accesskey = null;
        public string user = null;
        public string logPath = null;
        public string port = null;
        public bool autoRun = false;
        public bool debug = false;
        public int delay;
        public bool encryptSSP = false;

        public delegate void ThrowedException(Exception inException);
        private readonly ThrowedException _exceptionThrower;

        public delegate void ReceivedEvent(BetShopPushEvent inEvent);
        private readonly ReceivedEvent _eventReceiver;

        public BetShopATMConnector(string folderName, ReceivedEvent inEvent, ThrowedException inException)
        {
            LoadConfigFile(folderName);
            _exceptionThrower = inException;
            _eventReceiver = inEvent;
        }

        private void LoadConfigFile(string folderName)
        {
            string filename = Path.Combine(folderName, "BetShop.cfg");

            if (File.Exists(filename))
            {
                TextReader file = new StreamReader(filename);

                string line = null;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("client#"))
                    {
                        client = line.Substring("client#".Length);
                    }
                    else if (line.StartsWith("accesskey#"))
                    {
                        accesskey = line.Substring("accesskey#".Length);
                    }
                    else if (line.StartsWith("user#"))
                    {
                        user = line.Substring("user#".Length);
                    }
                    else if (line.StartsWith("serviceURL#"))
                    {
                        serviceURL = line.Substring("serviceURL#".Length);
                    }
                    else if (line.StartsWith("logPath#"))
                    {
                        //logPath = line.Substring("logPath#".Length);
                        logPath = folderName;
                    }
                    else if (line.StartsWith("port#"))
                    {
                        port = line.Substring("port#".Length);
                    }
                    else if (line.StartsWith("autoRun#"))
                    {
                        bool.TryParse(line.Substring("autoRun#".Length), out autoRun);
                    }
                    else if (line.StartsWith("debug#"))
                    {
                        bool.TryParse(line.Substring("debug#".Length), out debug);
                    }
                    else if (line.StartsWith("delay#"))
                    {
                        Int32.TryParse(line.Substring("delay#".Length), out delay);
                    }
                    else if (line.StartsWith("encryptSSP#"))
                    {
                        bool.TryParse(line.Substring("encryptSSP#".Length), out encryptSSP);
                    }
                }

                file.Close();
            }
        }

        public void AppendText(string text, double amount = 0.0)
        {
            //textBox.AppendText(text);

            string message = string.Format("{{ \"time\":\"{0}\", \"client\":\"{1}\", \"accesskey\":\"{2}\", \"user\":\"{3}\", \"amount\":\"{4}\", \"message\":\"{5}\" }}",
                DateTime.Now.ToString(), client, accesskey, user, amount.ToString(), text.Replace("\r\n", " "));
            string responseMessage = null;

            Exception ex = null;
            if (!string.IsNullOrEmpty(serviceURL))
            {
                HttpWebRequest webRequest = WebRequest.Create(serviceURL) as HttpWebRequest;
                webRequest.ContentType = "application/json";
                webRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    streamWriter.Write(message);
                    streamWriter.Close();
                }

                try
                {
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream);
                    responseMessage = streamReader.ReadToEnd();                    
                }
                catch (Exception exc)
                {
                    ex = exc;
                    //textBoxLog.Invoke(new MethodInvoker(delegate { textBoxLog.AppendText("\r\n" + ex.Message + "\r\n"); textBoxLog.SelectionStart = textBoxLog.Text.Length; textBoxLog.ScrollToCaret(); }));
                    //LogError(ex, false);
                }
            }
            if (!string.IsNullOrEmpty(logPath))
            {
                TextWriter log = new StreamWriter(Path.Combine(logPath, "eSSP.log"), true);
                if (ex != null)
                {
                    log.WriteLine("EXCEPTION: " + ex.GetType().Name + " - " + ex.Message);
                    log.WriteLine("STACK TRACE: \r\n" + ex.StackTrace);
                    _exceptionThrower(ex);
                }
                else
                {
                    log.WriteLine("POST: " + message);
                    if (!string.IsNullOrEmpty(responseMessage))
                        log.WriteLine("response: " + responseMessage);
                    BetShopPushEvent bsEvt = new BetShopPushEvent();
                    bsEvt.status = BetShopPushEvent.PushStatus.OK;
                    bsEvt.amountPushed = amount;
                    bsEvt.messageReceived = responseMessage;
                    _eventReceiver(bsEvt);
                    bsEvt = null;
                }
                log.Close();
            }
        }
    }
}
