using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier
{
    public partial class BrowserForm : Form
    {
        private string URL = string.Empty;

        public BrowserForm()
            : this(@"https://www.tipico.com/en/online-sports-betting/")
        {
        }

        public BrowserForm(string url)
        {
            InitializeComponent();

            Browser.ScriptErrorsSuppressed = true;
            Browser.Url = new Uri(URL = url);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            Hide();
            e.Cancel = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Text = Browser.DocumentTitle;
            Browser.DocumentTitleChanged += new EventHandler(webBrowser_DocumentTitleChanged);
        }

        void webBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            Text = Browser.DocumentTitle;
        }

        private void BrowserForm_Load(object sender, EventArgs e)
        {

        }
    }
}
