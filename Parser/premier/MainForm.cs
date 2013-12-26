using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
//using System.Xml.Linq;
using premier.parser;
using System.Media;

namespace premier
{
    public partial class MainForm : Form
    {
        #region Fields

        // vreme poslednje promene na sajtu
        private DateTime lastUpdateTime = DateTime.Now;

        // timer koji se koristi za proveru da li je stigao neki update sa tipico sajta
        private Timer updateTimer = new Timer();

        // timer koji se koristi za okidanje 'refresh page' komande na browseru
        private Timer refreshTimer = new Timer();

        // generator random brojeva (koji se vise ne koristi)
        private Random random = new Random();

        // forma u kojoj se sama browser kontrola nalazi
        private BrowserForm browserForm = new BrowserForm();

        // adresa servisa na koji cemo da uploadujemo podatke koje parsiramo
        private List<string> serviceURL = new List<string>();

        // sound file
        private string soundToPlay = @"c:\Windows\Media\Windows Ringin.wav";

        // sound interval
        private int soundInterval = 60;

        // katalog id-ova dogadjaja koje pratimo i koje smo pratili
        // kljuc je sam id, value je naziv dogadjaja
        private Dictionary<string, string> listOfIds = new Dictionary<string, string>();

        private string logPath;

        #endregion

        #region Construction and initialization

        public MainForm()
        {
            InitializeComponent();

#if DEBUG
            buttonTest.Visible = true;
            // browserForm.webBrowser.Url = new Uri("file:///C:/Users/Milos/Downloads/top%20match/02.htm");
            // browserForm.webBrowser.Url = new Uri("file:///C:/Users/Milos/Downloads/top%20match/08%20-%20ht.htm");
            // browserForm.webBrowser.Url = new Uri("file:///C:/Users/Milos/Downloads/htmlscreen.html");
#endif
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadIds();

            browserForm.webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            browserForm.Show();

            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            refreshTimer.Tick += new EventHandler(refreshTimer_Tick);
        }

        #endregion 

        #region Event handlers

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (Modal)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                Close();
            }
        }

        bool forceUpdate = false;
        private void buttonParseData_Click(object sender, EventArgs e)
        {
            forceUpdate = true;
            webBrowser_DocumentCompleted(null, null);
            // ParseHtmlDocument(true);
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
#if DEBUG
            //if (true)
            if (browserForm.webBrowser.Url.AbsoluteUri == BrowserForm.URL)
#else
            if (browserForm.webBrowser.Url.AbsoluteUri == BrowserForm.URL)
#endif
            {
                checkBoxAutoUpdate_CheckedChanged(sender, e);
                checkBoxAutoClick_CheckedChanged(sender, e);
            }
            else
            {
                browserForm.webBrowser.Url = new Uri(BrowserForm.URL);
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Stop();

            refreshTimer.Stop();
            textBoxClick_TextChanged(null, null);
            refreshTimer.Start();

            // browserForm.webBrowser.Refresh();
            browserForm.webBrowser.Url = new Uri(BrowserForm.URL);
        }

        private void old_refreshTimer_Tick(object sender, EventArgs e)
        {
            foreach (HtmlElement button in browserForm.webBrowser.Document.GetElementsByTagName("button"))
            {
                // Console.WriteLine(button.InnerHtml);
                if (button.OuterHtml.Contains("WEB_LIVE_UPCOMING"))
                {
                    if (random.Next(1000) < 10)
                    {
                        button.InvokeMember("onclick");
                        break;
                    }
                }
            }

            List<string> links = new List<string>();

            int startIndex = 0;
            while (true)
            {
                // startIndex = browserForm.webBrowser.DocumentText.IndexOf("upcoming_", startIndex);
                // <button name="q6062813910" type="button" class="but qbtn roll_red" onclick="javascript:tr(6062813910,'WEB_LIVE_UPCOMING')">1,55</button>
                // startIndex = "WEB_LIVE_UPCOMING"; 
                startIndex = browserForm.webBrowser.DocumentText.IndexOf("<button name=");
                if (startIndex == -1)
                {
                    break;
                }

                startIndex = browserForm.webBrowser.DocumentText.IndexOf('\"', startIndex);
                if (startIndex == -1)
                {
                    break;
                }

                int endIndex = browserForm.webBrowser.DocumentText.IndexOf('\"', startIndex + 1);
                if (endIndex == -1)
                {
                    break;
                }

                if (startIndex + 20 > endIndex)
                {
                    string tryThis = browserForm.webBrowser.DocumentText.Substring(startIndex, endIndex - startIndex + 1);
                    links.Add(tryThis);

                    break;
                }
                startIndex++;
            }

            foreach (string link in links)
            {
                HtmlElement el = browserForm.webBrowser.Document.All[link];
                if (el != null)
                {
                    el.InvokeMember("onclick");
                }
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            ParseHtmlDocument(forceUpdate);
        }

        private void buttonBrowser_Click(object sender, EventArgs e)
        {
            if (browserForm.Visible) browserForm.Hide(); else browserForm.Show();
        }

        private void buttonSavePage_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "html files (*.html)|*.html|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string html = browserForm.webBrowser.DocumentText;

                TextWriter file = new StreamWriter(saveFileDialog.FileName);

                file.WriteLine(html);
                file.Close();
            }
        }

        private void checkBoxAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            textBoxInterval_TextChanged(sender, e);

            if (checkBoxAutoUpdate.Checked)
            {
                if (!updateTimer.Enabled)
                {
                    updateTimer.Start();
                }
            }
            else
            {
                if (updateTimer.Enabled)
                {
                    updateTimer.Stop();
                }
            }
        }

        private void textBoxInterval_TextChanged(object sender, EventArgs e)
        {
            int interval = 500;
            if (int.TryParse(textBoxInterval.Text, out interval))
            {
                updateTimer.Interval = interval;
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = @"C:\Users\Milos\Downloads";
            openFileDialog.Filter = "Web files (*.htm)|*.htm|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                browserForm.webBrowser.Url = new Uri(@"file:///" + openFileDialog.FileName.Replace('\\', '/'));
            }
        }

        private void checkBoxAutoClick_CheckedChanged(object sender, EventArgs e)
        {
            textBoxClick_TextChanged(sender, e);

            if (checkBoxAutoClick.Checked)
            {
                if (!refreshTimer.Enabled)
                {
                    refreshTimer.Start();
                }
            }
            else
            {
                if (refreshTimer.Enabled)
                {
                    refreshTimer.Stop();
                }
            }
        }

        private void textBoxClick_TextChanged(object sender, EventArgs e)
        {
            int interval = 500;
            if (int.TryParse(textBoxClick.Text, out interval))
            {
                refreshTimer.Interval = random.Next(interval * 800, interval * 1200);
            }
        }

        private void textBoxToken_LostFocus(object sender, EventArgs e)
        {
            SaveIds();
        }

        private void checkBoxWebUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWebUpdate.Checked)
            {
                foreach (header h in itemCatalog.Keys)
                {
                    foreach (item i in itemCatalog[h])
                    {
                        if (!string.IsNullOrEmpty(i.Id))
                        {
                            SendUpdate(i.JSON(textBoxToken.Text), i.Id);
                        }
                    }
                }
            }
        }

        #endregion

        #region Data parsing

        // katalog koji sadrzi sve dogadjaje koje smo prepoznali na stranici
        // kljuc je sam heder dogadjaja (uglavnom sport)
        // katalog sadrzi listu dogadjaja iz date sekcije (sa istim hederom)
        private Dictionary<header, List<item>> itemCatalog = new Dictionary<header, List<item>>();

        // katalog koji sadrzi samu datagrid kontrolu, u kojoj prikazujemo sve dogadjaje
        // posto nam je potrebno dvostruko povezivanje, pravimo (i odrzavamo) i reverse katalog
        private Dictionary<header, DataGridView> viewCatalog = new Dictionary<header, DataGridView>();
        private Dictionary<DataGridView, header> viewReverseCatalog = new Dictionary<DataGridView, header>();

        // ovo je string koji cuva kompletan tekst sadrzaj html stranice
        // koristi se da bi smo proverili da je bilo nekakve promene na stranici od poslednjeg parsiranja
        private string htmlBody = null;

        // f-ja koju koristimo za osvezavanje korisnickog interfejsa
        // u njoj se prave potrebne datagridview kontrole, smestaju gde treba i osvezavaju se podaci u postojecim grid kontrolama
        private void UpdateView()
        {
            foreach (header h in itemCatalog.Keys)
            {
                if (viewCatalog.ContainsKey(h))
                {
                    if (viewCatalog[h].RowCount != itemCatalog[h].Count)
                    {
                        viewCatalog[h].RowCount = itemCatalog[h].Count;
                    }
                    viewCatalog[h].Refresh();
                }
                else
                {
                    DataGridView dataGridView = new DataGridView();

                    TabPage tabPage = new TabPage();

                    tabControl.SuspendLayout();
                    tabPage.SuspendLayout();
                    tabPage.Controls.Add(dataGridView);
                    tabPage.Location = new System.Drawing.Point(4, 22);
                    tabPage.Name = "tabPage" + h.Event;
                    tabPage.Padding = new System.Windows.Forms.Padding(3);
                    tabPage.Size = new System.Drawing.Size(639, 484);
                    tabPage.TabIndex = 0;
                    tabPage.Text = h.Event;
                    tabPage.UseVisualStyleBackColor = true;

                    tabControl.Controls.Add(tabPage);

                    dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
                    dataGridView.Location = new System.Drawing.Point(3, 3);
                    dataGridView.Name = "dataGridView" + h.Event;
                    dataGridView.Size = new System.Drawing.Size(633, 478);

                    dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
                    dataGridView.Name = "dataGridView" + h.Event;

                    tabPage.ResumeLayout(false);
                    tabControl.ResumeLayout(false);

                    dataGridView.ColumnCount = h.Lenght + 1;
                    dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    dataGridView.AllowUserToAddRows = false;
                    dataGridView.AllowUserToDeleteRows = false;

                    dataGridView.Columns[0].Name = "Id";
                    for (int i = 0; i < h.Lenght; i++)
                    {
                        dataGridView.Columns[i + 1].Name = h[i];
                    }

                    dataGridView.CellValueNeeded += new DataGridViewCellValueEventHandler(dataGridView_CellValueNeeded);
                    dataGridView.CellValuePushed += new DataGridViewCellValueEventHandler(dataGridView_CellValuePushed);
                    dataGridView.VirtualMode = true;

                    viewCatalog.Add(h, dataGridView);
                    viewReverseCatalog.Add(dataGridView, h);

                    viewCatalog[h].RowCount = itemCatalog[h].Count;
                }
            }
        }

        // ovo je f-ja koju pozivamo da bismo inicirali ponovno parsiranje html stranice, ako je potrebno
        // takodje, iz ove f-je se radi i sve sto bi bila posledica tog parsiranja (osvezavanje podataka, slanje update-a...)
        private bool soundPlayed = false;
        private void ParseHtmlDocument(bool forceParse)
        {
            //try
            {
                HtmlDocument document = browserForm.webBrowser.Document;

                if (document != null)
                {
                    document.InvokeScript("handleMove");

                    if (document.Body != null && document.Body.InnerText != null && (forceParse || document.Body.InnerText != htmlBody))
                    {
                        forceUpdate = false;
                        soundPlayed = false;
                        lastUpdateTime = DateTime.Now;

                        htmlBody = document.Body.InnerText;

                        parser.tipico tipico = new tipico();
                        tipico.PrepareHtml(document);
                        if (!tipico.ReadCompleted)
                        {
                            return;
                        }

                        tipico.InitAll(itemCatalog, SendUpdate, textBoxToken.Text, listOfIds);
                        tipico.DoAll();

                        UpdateView();

                        //System.Threading.Thread thread = new System.Threading.Thread(tipico.DoAll);
                        //thread.Start();
                    }

                    TimeSpan difference = DateTime.Now.Subtract(lastUpdateTime);
                    labelTimeSinceLastUpdate.Text = difference.Hours.ToString() + ":" + difference.Minutes.ToString("d2") + ":" + difference.Seconds.ToString("d2");
                    labelTimeSinceLastUpdate.ForeColor = difference.Minutes != 0 ? Color.Red : (difference.Seconds < 30 ? Color.Black : Color.Blue);

                    if (soundPlayed == false && difference.TotalSeconds > soundInterval)
                    {
                        soundPlayed = true;
                        try
                        {
                            new SoundPlayer(soundToPlay).Play();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    // SendUpdate(ex.Message, string.Empty);
            //    LogError(ex, false);
            //}
        }

        #region Grid - virtual mode

        void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (dataGridView != null && e.RowIndex >= 0
                && e.ColumnIndex == 0
                && viewReverseCatalog.ContainsKey(dataGridView)
                && itemCatalog.ContainsKey(viewReverseCatalog[dataGridView])
                && itemCatalog[viewReverseCatalog[dataGridView]].Count > e.RowIndex)
            {
                string previousID = itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].Id;

                if (!string.IsNullOrEmpty(previousID))
                {
                    listOfIds.Remove(previousID);
                }

                string newID = e.Value != null ? e.Value.ToString() : string.Empty;

                if (listOfIds.ContainsKey(newID))
                {
                    e.Value = previousID;
                }
                else
                {
                    itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].Id = newID;

                    if (!string.IsNullOrEmpty(newID))
                    {
                        listOfIds.Add(newID, itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].ToString());
                        SendUpdate(itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].JSON(textBoxToken.Text), itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].Id);
                    }
                }

                SaveIds();
            }
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (dataGridView != null && e.RowIndex >= 0
                && e.ColumnIndex >= 0
                && viewReverseCatalog.ContainsKey(dataGridView)
                && itemCatalog.ContainsKey(viewReverseCatalog[dataGridView])
                && itemCatalog[viewReverseCatalog[dataGridView]].Count > e.RowIndex
                && itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].Lenght > e.ColumnIndex - 1)
            {
                if (e.ColumnIndex == 0)
                {
                    e.Value = itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].Id;
                }
                else
                {
                    e.Value = itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex][e.ColumnIndex - 1];

                    if (e.ColumnIndex > 4 && itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex][e.ColumnIndex - 1] != itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].OldData[e.ColumnIndex - 1])
                    {
                        e.Value += " (" + itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].OldData[e.ColumnIndex - 1] + ")";
                    }
                }
            }
        }

        #endregion 

        #endregion

        #region Load/Save ID list

        void LoadIds()
        {
            logPath = Application.StartupPath;
            string filename = Path.Combine(Application.StartupPath, "premier.cfg");

            if (!File.Exists(filename))
            {
                filename = Path.Combine(Application.StartupPath, "id.txt");
            }

            if (File.Exists(filename))
            {
                TextReader file = new StreamReader(filename);

                string line = null;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("token#"))
                    {
                        textBoxToken.Text = line.Substring("token#".Length);
                    }
                    else if (line.StartsWith("update#"))
                    {
                        textBoxInterval.Text = line.Substring("update#".Length);
                    }
                    else if (line.StartsWith("refresh#"))
                    {
                        textBoxClick.Text = line.Substring("refresh#".Length);
                    }
                    else if (line.StartsWith("serviceURL#"))
                    {
                        serviceURL.Add(line.Substring("serviceURL#".Length));
                    }
                    else if (line.StartsWith("soundToPlay#"))
                    {
                        soundToPlay = line.Substring("soundToPlay#".Length);
                    }
                    else if (line.StartsWith("soundInterval#"))
                    {
                        string sSoundInterval = line.Substring("soundInterval#".Length);
                        int.TryParse(sSoundInterval, out soundInterval);
                    }
                    else if (line.StartsWith("post#"))
                    {
                        string sPost = line.Substring("post#".Length);
                        bool post = checkBoxWebUpdate.Checked;
                        bool.TryParse(sPost, out post);
                        checkBoxWebUpdate.Checked = post;
                    }
                    else if (line.StartsWith("refreshPage#"))
                    {
                        string sRefreshPage = line.Substring("refreshPage#".Length);
                        bool refreshPage = checkBoxAutoClick.Checked;
                        bool.TryParse(sRefreshPage, out refreshPage);
                        checkBoxAutoClick.Checked = refreshPage;
                    }
                    else if (line.StartsWith("logPath#"))
                    {
                        logPath = line.Substring("logPath#".Length);
                    }
                    else
                    {
                        int index = line.IndexOf('#');
                        listOfIds.Add(line.Substring(0, index), line.Substring(index));
                    }
                }

                file.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveIds();

            base.OnClosed(e);
        }

        void SaveIds()
        {
            TextWriter file = new StreamWriter(Path.Combine(Application.StartupPath, "premier.cfg"), false);

            foreach (string url in serviceURL)
                file.WriteLine("serviceURL#" + url);

            file.WriteLine("soundToPlay#" + soundToPlay);
            file.WriteLine("soundInterval#" + soundInterval.ToString());
            file.WriteLine("token#" + textBoxToken.Text);
            file.WriteLine("update#" + textBoxInterval.Text);
            file.WriteLine("refresh#" + textBoxClick.Text);
            file.WriteLine("post#" + checkBoxWebUpdate.Checked.ToString());
            file.WriteLine("refreshPage#" + checkBoxAutoClick.Checked.ToString());
            file.WriteLine("logPath#" + logPath);
            foreach (string id in listOfIds.Keys)
            {
                file.WriteLine(id + "#" + listOfIds[id]);
            }
            file.Close();
        }

        #endregion

        void SendUpdate(string message, string id)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string responseMessage = string.Empty;

                textBoxLog.Invoke(new MethodInvoker(delegate { textBoxLog.AppendText("\r\n" + message + "\r\n"); textBoxLog.SelectionStart = textBoxLog.Text.Length; textBoxLog.ScrollToCaret(); }));

                foreach (string url in serviceURL)
                {
                    if (!string.IsNullOrEmpty(id) && checkBoxWebUpdate.Checked)
                    {
                        HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
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
                        catch (Exception ex)
                        {
                            // textBoxLog.Invoke(new MethodInvoker(delegate { textBoxLog.AppendText("\r\n" + ex.Message + "\r\n"); textBoxLog.SelectionStart = textBoxLog.Text.Length; textBoxLog.ScrollToCaret(); }));
                            LogError(ex, false);
                        }
                    }
                }

                if (checkBoxLogToFile.Checked)
                {
                    TextWriter log = new StreamWriter(Path.Combine(Application.StartupPath, "log.txt"), true);
                    log.WriteLine(message);
                    if (!string.IsNullOrEmpty(responseMessage))
                    {
                        log.WriteLine("\r\nresponse: " + responseMessage + "\r\n");
                    }
                    log.Close();

                    if (!string.IsNullOrEmpty(id))
                    {
                        TextWriter logID = new StreamWriter(Path.Combine(Application.StartupPath, "log" + id.Replace('+', '_') + ".txt"), true);
                        logID.WriteLine(message);
                        if (!string.IsNullOrEmpty(responseMessage))
                        {
                            logID.WriteLine("\r\nresponse: " + responseMessage + "\r\n");
                        }
                        logID.Close();
                    }
                }

                if (!string.IsNullOrEmpty(responseMessage))
                {
                    textBoxLog.Invoke(new MethodInvoker(delegate { textBoxLog.AppendText("\r\nresponse: " + responseMessage + "\r\n"); textBoxLog.SelectionStart = textBoxLog.Text.Length; textBoxLog.ScrollToCaret(); }));
                }
            }
        }

        private void LogError(Exception ex, bool innerEx)
        {
            TextWriter log = new StreamWriter(Path.Combine(logPath, "ErrorLog.txt"), true);
            if (innerEx)
            {
                log.WriteLine("Inner Exception");
            }
            else
            {
                log.WriteLine("Exception occurred!");
            }
            log.WriteLine("Message: ");
            log.WriteLine(ex.Message);
            log.WriteLine("StackTrace: ");
            log.WriteLine(ex.StackTrace);
            log.WriteLine("StackTrace: ");
            log.WriteLine();
            log.Close();

            if (ex.InnerException != null)
            {
                LogError(ex.InnerException, true);
            }
        }

        private void buttonReloadPage_Click(object sender, EventArgs e)
        {
            updateTimer.Stop();
            // browserForm.webBrowser.Refresh();
            browserForm.webBrowser.Url = new Uri(BrowserForm.URL);
        }
    }
}
