using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using premier.parser;

namespace premier.tipico.resutls
{
    public partial class MainForm : Form
    {
        private BrowserForm browserForm = new BrowserForm();

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonBrowser_Click(object sender, EventArgs e)
        {
            if (browserForm.Visible) browserForm.Hide(); else browserForm.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            browserForm.Show();
        }

        private void buttonParse_Click(object sender, EventArgs e)
        {
            //Dictionary<string, string> sports = new Dictionary<string, string>();

            //sports.Add("EURO 2012", "2597858802227162236");
            //sports.Add("Football", "1101");
            //sports.Add("Tennis", "4101");
            //sports.Add("Basketball", "5101");
            //sports.Add("Handball", "2101");
            //sports.Add("Voleyball", "3201");

            //MyInvokeScript("viewResults", "2597858802227162236", 3);

            //foreach (string sport in sports.Keys)
            //{
            //    MyInvokeScript("viewResults", sports[sport], 3);
            //    System.Threading.Thread.Sleep(5000);
            //    ParseResultsPage(sport);
            //}

            ParseResultsPage();
        }

        private Dictionary<header, DataGridView> viewCatalog = new Dictionary<header, DataGridView>();
        private Dictionary<DataGridView, header> viewReverseCatalog = new Dictionary<DataGridView, header>();

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

        private Dictionary<string, string> listOfIds = new Dictionary<string, string>();
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
                        SendUpdate(itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].JSON(string.Empty), itemCatalog[viewReverseCatalog[dataGridView]][e.RowIndex].Id);
                    }
                }

                // SaveIds();
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

        private Dictionary<header, List<item>> itemCatalog = new Dictionary<header, List<item>>();
        private void ParseResultsPage()
        {
            HtmlDocument document = browserForm.webBrowser.Document;

            if (document != null && document.Body != null && document.Body.InnerText != null)
            {
                parser.results results = new parser.results();
                results.ParseHtml(document, sport, itemCatalog, listOfIds, SendUpdate);

                UpdateView();
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            MyInvokeScript("viewResults", "5101", 3);
        }

        private object MyInvokeScript(string name, params object[] args)
        {
            return browserForm.webBrowser.Document.InvokeScript(name, args);
        }

        string sport = "Live";
        private void buttonEuro_Click(object sender, EventArgs e)
        {
            sport = "EURO 2012";
            MyInvokeScript("viewResults", "2597858802227162236", 3);
        }

        private void buttonFootball_Click(object sender, EventArgs e)
        {
            sport = "Football";
            MyInvokeScript("viewResults", "1101", 3);
        }

        private void buttonBasketball_Click(object sender, EventArgs e)
        {
            sport = "Basketball";
            MyInvokeScript("viewResults", "2101", 3);
        }

        private void buttonHandball_Click(object sender, EventArgs e)
        {
            sport = "Handball";
            MyInvokeScript("viewResults", "6101", 3);
        }

        void SendUpdate(string message, string id)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string responseMessage = string.Empty;

                textBoxLog.Invoke(new MethodInvoker(delegate { textBoxLog.AppendText("\r\n" + message + "\r\n"); textBoxLog.SelectionStart = textBoxLog.Text.Length; textBoxLog.ScrollToCaret(); }));

                //if (!string.IsNullOrEmpty(id) && checkBoxWebUpdate.Checked)
                //{
                //    HttpWebRequest webRequest = WebRequest.Create(serviceURL) as HttpWebRequest;
                //    webRequest.ContentType = "application/json";
                //    webRequest.Method = "POST";

                //    using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                //    {
                //        streamWriter.Write(message);
                //        streamWriter.Close();
                //    }

                //    try
                //    {
                //        HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                //        Stream responseStream = response.GetResponseStream();
                //        StreamReader streamReader = new StreamReader(responseStream);
                //        responseMessage = streamReader.ReadToEnd();
                //    }
                //    catch (Exception ex)
                //    {
                //        // textBoxLog.Invoke(new MethodInvoker(delegate { textBoxLog.AppendText("\r\n" + ex.Message + "\r\n"); textBoxLog.SelectionStart = textBoxLog.Text.Length; textBoxLog.ScrollToCaret(); }));
                //        LogError(ex, false);
                //    }
                //}

                //if (checkBoxLogToFile.Checked)
                //{
                //    TextWriter log = new StreamWriter(Path.Combine(Application.StartupPath, "log.txt"), true);
                //    log.WriteLine(message);
                //    if (!string.IsNullOrEmpty(responseMessage))
                //    {
                //        log.WriteLine("\r\nresponse: " + responseMessage + "\r\n");
                //    }
                //    log.Close();

                //    if (!string.IsNullOrEmpty(id))
                //    {
                //        TextWriter logID = new StreamWriter(Path.Combine(Application.StartupPath, "log" + id + ".txt"), true);
                //        logID.WriteLine(message);
                //        if (!string.IsNullOrEmpty(responseMessage))
                //        {
                //            logID.WriteLine("\r\nresponse: " + responseMessage + "\r\n");
                //        }
                //        logID.Close();
                //    }
                //}

                //if (!string.IsNullOrEmpty(responseMessage))
                //{
                //    textBoxLog.Invoke(new MethodInvoker(delegate { textBoxLog.AppendText("\r\nresponse: " + responseMessage + "\r\n"); textBoxLog.SelectionStart = textBoxLog.Text.Length; textBoxLog.ScrollToCaret(); }));
                //}
            }
        }

    }
}
