using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using premier.parser;
using System.IO;
using premier.parser.serialization;
using premier.basic.Atuomation;

namespace premier.basic
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        #region Fields

        public List<SportEvent> SportEvents { get; set; }

        #endregion

        #region Contruction and initialization

        public MainForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = SportEvents = new List<SportEvent>();
            gridControl.DataSource = bindingSource;

#if DEBUG
            simpleButtonParsePage.Visible = simpleButtonShowAllGames.Visible = true;
#endif
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            browserForm.Show();

            string defaultLayout = "DefaultLayout.xml";
            if (File.Exists(defaultLayout))
            {
                gridView.RestoreLayoutFromXml(defaultLayout);
            }

            LoadIDs();
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveIDs();

            base.OnClosed(e);
        }

        #endregion

        #region Browser form

        private BrowserForm browserForm = new BrowserForm("http://williamhill.com/");
        private void simpleButtonShowBrowser_Click(object sender, EventArgs e)
        {
            if (browserForm.Visible) browserForm.Hide(); else browserForm.Show();
        }

        #endregion

        #region Buttons

        private void simpleButtonParseData_Click(object sender, EventArgs e)
        {
            DataParser parser = new DataParser();
            parser.ParseHtml(browserForm.Browser.Document);

            AddList(parser.Events);
        }

        private void simpleButtonShowAllGames_Click(object sender, EventArgs e)
        {
            foreach (HtmlElement tr in browserForm.Browser.Document.Body.GetElementsByTagName("div"))
            {
                string outerHTML = string.IsNullOrEmpty(tr.OuterHtml) ? string.Empty : tr.OuterHtml.TrimStart();
                if (outerHTML.StartsWith("<DIV id=block"))
                {
                    tr.InvokeMember("onclick");
                }
            }
        }

        private void simpleButtonSaveMarkedItems_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void simpleButtonSaveAll_Click(object sender, EventArgs e)
        {
            Save(false);
        }

        private void simpleButtonLoad_Click(object sender, EventArgs e)
        {
            List<SportEvent> list = LoadJSON();
            if (list != null)
            {
                AddList(list);
            }
        }

        private void simpleButtonClearList_Click(object sender, EventArgs e)
        {
            ClearList();
        }

        private void simpleButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void simpleButtonSetIds_Click(object sender, EventArgs e)
        {
            if (idMap.ContainsKey(textEditStartingID.Text))
            {
                MessageBox.Show("ID already exists!" + Environment.NewLine + Environment.NewLine + idMap.GetValue(textEditStartingID.Text).Replace(SportEvent.Separator, Environment.NewLine));
                return;
            }

            List<SportEvent> updateList = new List<SportEvent>();

            foreach (int i in gridView.GetSelectedRows())
            {
                updateList.Add(SportEvents[gridView.GetDataSourceRowIndex(i)] as SportEvent);
            }
            SetIDs(textEditStartingID.Text, updateList); 

            gridView.RefreshData();
        }

        #endregion

        #region Logic

        private void AddList(List<SportEvent> list)
        {
            foreach (SportEvent se in list)
            {
                if (SportEvents.Contains(se))
                {
                    SportEvents[SportEvents.IndexOf(se)].Update(se);
                }
                else
                {
                    SportEvents.Add(se);
                }

                if (idMap.ContainsValue(se.ToString()))
                {
                    se.Id = idMap.GetKey(se.ToString());
                }
            }
            gridView.RefreshData();
        }

        private void ClearList()
        {
            SportEvents.Clear();
            gridView.RefreshData();
        }

        private void Save(bool markedOnly)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextWriter textWriter = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);
                textWriter.WriteLine(Serialize.CreateJSON(SportEvents, markedOnly).Replace("{", "\r\n\t{").Replace("]", "\r\n\t]").Replace("[", "\r\n\t["));
                textWriter.Close();
            }
        }

        public List<SportEvent> LoadJSON()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            List<SportEvent> list = null;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextReader textReader = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                list = Serialize.LoadJSON(textReader.ReadToEnd());
                textReader.Close();
            }
            return list;
        }

        private void simpleButtonGO_Click(object sender, EventArgs e)
        {
            Manager.Instance.Browser = browserForm.Browser;

            int interval;
            if (!int.TryParse(textEditInterval.Text, out interval))
            {
                interval = 1000;
            }

            Manager.Instance.AddTask(new Task(TaskType.OpanAllGames, "Opening all games on current page", interval, null, browserForm.Browser.Document, AddList, LogData));
            Manager.Instance.StartTasks();
        }

        public void LogData(string message)
        {
            memoEditLog.Invoke(new MethodInvoker(delegate { memoEditLog.Text += (DateTime.Now.ToLongTimeString() + ": " + message + "\r\n"); memoEditLog.SelectionStart = memoEditLog.Text.Length; memoEditLog.ScrollToCaret(); }));
        }

        #endregion

        #region Grid

        private void gridView_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            gridView.BeginUpdate();
            try
            {
                GridView parent = (sender as GridView);

                (parent.GetDetailView(e.RowHandle, e.RelationIndex) as GridView).Columns[1].Visible = false;
            }
            finally
            {
                gridView.EndUpdate();
            }
        }

        #region Export data

        private void pDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true; 
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                gridView.GridControl.ExportToPdf(saveFileDialog.FileName);
            }
        }

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                gridView.GridControl.ExportToHtml(saveFileDialog.FileName);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        private void xLSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                gridView.GridControl.ExportToXls(saveFileDialog.FileName);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
        }

        #endregion

        #region Grid layout

        private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.RestoreDirectory = false;

            saveFileDialog.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                gridView.SaveLayoutToXml(saveFileDialog.FileName);
            }
        }

        private void loadLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                gridView.RestoreLayoutFromXml(openFileDialog.FileName);
            }
        }

        #endregion

        #endregion

        #region ID handling

        private BidirectionalDictionary<string, string> idMap = new BidirectionalDictionary<string, string>();
        private static readonly string IDsFilename = "IDs.txt";

        private void SaveIDs()
        {
            TextWriter file = new StreamWriter(Path.Combine(Application.StartupPath, IDsFilename), false);

            foreach (string id in idMap.Keys)
            {
                file.WriteLine(id + "=" + idMap.GetValue(id));
            }
            file.Close();
        }

        private void LoadIDs()
        {
            string filename = Path.Combine(Application.StartupPath, IDsFilename);

            if (File.Exists(filename))
            {
                TextReader file = new StreamReader(filename);

                string line = null;
                while ((line = file.ReadLine()) != null)
                {
                    int index = line.IndexOf('=');
                    idMap.Add(line.Substring(0, index), line.Substring(index + 1));
                }

                file.Close();
            }
        }

        private void SetIDs(string startingID, List<SportEvent> updateList)
        {
            if (string.IsNullOrEmpty(startingID))
            {
                foreach (SportEvent sportEvent in updateList)
                {
                    if (!string.IsNullOrEmpty(sportEvent.Id))
                    {
                        idMap.RemoveValue(sportEvent.ToString());
                        sportEvent.Id = string.Empty;
                    }
                }
            }
            else
            {
                int counter = 0;
                int length = startingID.Length;

                if (!int.TryParse(startingID, out counter))
                {
                    counter = 0;
                }

                foreach (SportEvent sportEvent in updateList)
                {
                    string nextID = (counter++).ToString("D" + length.ToString());
                    while (idMap.ContainsKey(nextID))
                        nextID = (counter++).ToString("D" + length.ToString());

                    sportEvent.Id = nextID;

                    if (idMap.ContainsValue(sportEvent.ToString()))
                        idMap.RemoveValue(sportEvent.ToString());

                    idMap.Add(nextID, sportEvent.ToString());
                }
            }
        }

        #endregion 
    }
}