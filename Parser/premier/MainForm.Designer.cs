namespace premier
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonParseData = new System.Windows.Forms.Button();
            this.buttonBrowser = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonSavePage = new System.Windows.Forms.Button();
            this.checkBoxLogToFile = new System.Windows.Forms.CheckBox();
            this.checkBoxWebUpdate = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoUpdate = new System.Windows.Forms.CheckBox();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.textBoxTest = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.textBoxToken = new System.Windows.Forms.TextBox();
            this.labelToken = new System.Windows.Forms.Label();
            this.checkBoxAutoClick = new System.Windows.Forms.CheckBox();
            this.textBoxClick = new System.Windows.Forms.TextBox();
            this.labelTimeSinceLastUpdateTitle = new System.Windows.Forms.Label();
            this.labelTimeSinceLastUpdate = new System.Windows.Forms.Label();
            this.buttonReloadPage = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(665, 499);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(150, 23);
            this.buttonClose.TabIndex = 17;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonParseData
            // 
            this.buttonParseData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonParseData.Location = new System.Drawing.Point(665, 63);
            this.buttonParseData.Name = "buttonParseData";
            this.buttonParseData.Size = new System.Drawing.Size(150, 23);
            this.buttonParseData.TabIndex = 2;
            this.buttonParseData.Text = "Parse Data";
            this.buttonParseData.UseVisualStyleBackColor = true;
            this.buttonParseData.Click += new System.EventHandler(this.buttonParseData_Click);
            // 
            // buttonBrowser
            // 
            this.buttonBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowser.Location = new System.Drawing.Point(665, 34);
            this.buttonBrowser.Name = "buttonBrowser";
            this.buttonBrowser.Size = new System.Drawing.Size(150, 23);
            this.buttonBrowser.TabIndex = 1;
            this.buttonBrowser.Text = "Show Browser";
            this.buttonBrowser.UseVisualStyleBackColor = true;
            this.buttonBrowser.Click += new System.EventHandler(this.buttonBrowser_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageLog);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(647, 510);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.textBoxLog);
            this.tabPageLog.Location = new System.Drawing.Point(4, 22);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(639, 484);
            this.tabPageLog.TabIndex = 0;
            this.tabPageLog.Text = "Log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(3, 3);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.Size = new System.Drawing.Size(633, 478);
            this.textBoxLog.TabIndex = 0;
            // 
            // buttonSavePage
            // 
            this.buttonSavePage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSavePage.Location = new System.Drawing.Point(665, 336);
            this.buttonSavePage.Name = "buttonSavePage";
            this.buttonSavePage.Size = new System.Drawing.Size(150, 23);
            this.buttonSavePage.TabIndex = 14;
            this.buttonSavePage.Text = " Save Page";
            this.buttonSavePage.UseVisualStyleBackColor = true;
            this.buttonSavePage.Click += new System.EventHandler(this.buttonSavePage_Click);
            // 
            // checkBoxLogToFile
            // 
            this.checkBoxLogToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxLogToFile.AutoSize = true;
            this.checkBoxLogToFile.Checked = true;
            this.checkBoxLogToFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLogToFile.Location = new System.Drawing.Point(665, 92);
            this.checkBoxLogToFile.Name = "checkBoxLogToFile";
            this.checkBoxLogToFile.Size = new System.Drawing.Size(72, 17);
            this.checkBoxLogToFile.TabIndex = 3;
            this.checkBoxLogToFile.Text = "Log to file";
            this.checkBoxLogToFile.UseVisualStyleBackColor = true;
            // 
            // checkBoxWebUpdate
            // 
            this.checkBoxWebUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxWebUpdate.AutoSize = true;
            this.checkBoxWebUpdate.Checked = true;
            this.checkBoxWebUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWebUpdate.Location = new System.Drawing.Point(665, 115);
            this.checkBoxWebUpdate.Name = "checkBoxWebUpdate";
            this.checkBoxWebUpdate.Size = new System.Drawing.Size(119, 17);
            this.checkBoxWebUpdate.TabIndex = 4;
            this.checkBoxWebUpdate.Text = "Post to web service";
            this.checkBoxWebUpdate.UseVisualStyleBackColor = true;
            this.checkBoxWebUpdate.CheckedChanged += new System.EventHandler(this.checkBoxWebUpdate_CheckedChanged);
            // 
            // checkBoxAutoUpdate
            // 
            this.checkBoxAutoUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoUpdate.AutoSize = true;
            this.checkBoxAutoUpdate.Checked = true;
            this.checkBoxAutoUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoUpdate.Location = new System.Drawing.Point(665, 138);
            this.checkBoxAutoUpdate.Name = "checkBoxAutoUpdate";
            this.checkBoxAutoUpdate.Size = new System.Drawing.Size(83, 17);
            this.checkBoxAutoUpdate.TabIndex = 5;
            this.checkBoxAutoUpdate.Text = "Update (ms)";
            this.checkBoxAutoUpdate.UseVisualStyleBackColor = true;
            this.checkBoxAutoUpdate.CheckedChanged += new System.EventHandler(this.checkBoxAutoUpdate_CheckedChanged);
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInterval.Location = new System.Drawing.Point(760, 136);
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(55, 20);
            this.textBoxInterval.TabIndex = 6;
            this.textBoxInterval.Text = "500";
            this.textBoxInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxInterval.TextChanged += new System.EventHandler(this.textBoxInterval_TextChanged);
            // 
            // textBoxTest
            // 
            this.textBoxTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTest.Location = new System.Drawing.Point(665, 393);
            this.textBoxTest.Name = "textBoxTest";
            this.textBoxTest.Size = new System.Drawing.Size(150, 20);
            this.textBoxTest.TabIndex = 15;
            this.textBoxTest.Visible = false;
            // 
            // buttonTest
            // 
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTest.Location = new System.Drawing.Point(740, 419);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(75, 23);
            this.buttonTest.TabIndex = 16;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Visible = false;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // textBoxToken
            // 
            this.textBoxToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxToken.Location = new System.Drawing.Point(709, 162);
            this.textBoxToken.Name = "textBoxToken";
            this.textBoxToken.Size = new System.Drawing.Size(106, 20);
            this.textBoxToken.TabIndex = 8;
            this.textBoxToken.LostFocus += new System.EventHandler(this.textBoxToken_LostFocus);
            // 
            // labelToken
            // 
            this.labelToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelToken.AutoSize = true;
            this.labelToken.Location = new System.Drawing.Point(665, 165);
            this.labelToken.Name = "labelToken";
            this.labelToken.Size = new System.Drawing.Size(38, 13);
            this.labelToken.TabIndex = 7;
            this.labelToken.Text = "Token";
            // 
            // checkBoxAutoClick
            // 
            this.checkBoxAutoClick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoClick.AutoSize = true;
            this.checkBoxAutoClick.Checked = true;
            this.checkBoxAutoClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoClick.Location = new System.Drawing.Point(665, 188);
            this.checkBoxAutoClick.Name = "checkBoxAutoClick";
            this.checkBoxAutoClick.Size = new System.Drawing.Size(89, 17);
            this.checkBoxAutoClick.TabIndex = 9;
            this.checkBoxAutoClick.Text = "Refresh (sec)";
            this.checkBoxAutoClick.UseVisualStyleBackColor = true;
            this.checkBoxAutoClick.CheckedChanged += new System.EventHandler(this.checkBoxAutoClick_CheckedChanged);
            // 
            // textBoxClick
            // 
            this.textBoxClick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClick.Location = new System.Drawing.Point(764, 186);
            this.textBoxClick.Name = "textBoxClick";
            this.textBoxClick.Size = new System.Drawing.Size(51, 20);
            this.textBoxClick.TabIndex = 10;
            this.textBoxClick.Text = "500";
            this.textBoxClick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxClick.TextChanged += new System.EventHandler(this.textBoxClick_TextChanged);
            // 
            // labelTimeSinceLastUpdateTitle
            // 
            this.labelTimeSinceLastUpdateTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTimeSinceLastUpdateTitle.AutoSize = true;
            this.labelTimeSinceLastUpdateTitle.Location = new System.Drawing.Point(665, 224);
            this.labelTimeSinceLastUpdateTitle.Name = "labelTimeSinceLastUpdateTitle";
            this.labelTimeSinceLastUpdateTitle.Size = new System.Drawing.Size(119, 13);
            this.labelTimeSinceLastUpdateTitle.TabIndex = 11;
            this.labelTimeSinceLastUpdateTitle.Text = "Time since last change:";
            // 
            // labelTimeSinceLastUpdate
            // 
            this.labelTimeSinceLastUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTimeSinceLastUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTimeSinceLastUpdate.Location = new System.Drawing.Point(665, 237);
            this.labelTimeSinceLastUpdate.Name = "labelTimeSinceLastUpdate";
            this.labelTimeSinceLastUpdate.Size = new System.Drawing.Size(150, 35);
            this.labelTimeSinceLastUpdate.TabIndex = 12;
            this.labelTimeSinceLastUpdate.Text = "0:00:00";
            this.labelTimeSinceLastUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonReloadPage
            // 
            this.buttonReloadPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReloadPage.Location = new System.Drawing.Point(665, 275);
            this.buttonReloadPage.Name = "buttonReloadPage";
            this.buttonReloadPage.Size = new System.Drawing.Size(150, 23);
            this.buttonReloadPage.TabIndex = 13;
            this.buttonReloadPage.Text = "Reload Page";
            this.buttonReloadPage.UseVisualStyleBackColor = true;
            this.buttonReloadPage.Click += new System.EventHandler(this.buttonReloadPage_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 534);
            this.Controls.Add(this.buttonReloadPage);
            this.Controls.Add(this.labelTimeSinceLastUpdate);
            this.Controls.Add(this.labelTimeSinceLastUpdateTitle);
            this.Controls.Add(this.labelToken);
            this.Controls.Add(this.textBoxToken);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.textBoxTest);
            this.Controls.Add(this.textBoxClick);
            this.Controls.Add(this.textBoxInterval);
            this.Controls.Add(this.checkBoxAutoClick);
            this.Controls.Add(this.checkBoxAutoUpdate);
            this.Controls.Add(this.checkBoxWebUpdate);
            this.Controls.Add(this.checkBoxLogToFile);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonBrowser);
            this.Controls.Add(this.buttonSavePage);
            this.Controls.Add(this.buttonParseData);
            this.Controls.Add(this.buttonClose);
            this.Name = "MainForm";
            this.Text = "premier - tipico";
            this.tabControl.ResumeLayout(false);
            this.tabPageLog.ResumeLayout(false);
            this.tabPageLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonParseData;
        private System.Windows.Forms.Button buttonBrowser;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button buttonSavePage;
        private System.Windows.Forms.CheckBox checkBoxLogToFile;
        private System.Windows.Forms.CheckBox checkBoxWebUpdate;
        private System.Windows.Forms.CheckBox checkBoxAutoUpdate;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.TextBox textBoxTest;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.TextBox textBoxToken;
        private System.Windows.Forms.Label labelToken;
        private System.Windows.Forms.CheckBox checkBoxAutoClick;
        private System.Windows.Forms.TextBox textBoxClick;
        private System.Windows.Forms.Label labelTimeSinceLastUpdateTitle;
        private System.Windows.Forms.Label labelTimeSinceLastUpdate;
        private System.Windows.Forms.Button buttonReloadPage;
    }
}

