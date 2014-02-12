namespace premier.basic
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
            this.components = new System.ComponentModel.Container();
            this.simpleButtonShowBrowser = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonParsePage = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.contextMenuStripGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hTMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xLSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sportGameBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSport = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLeague = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHome = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAway = new DevExpress.XtraGrid.Columns.GridColumn();
            this.sportEventBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.simpleButtonShowAllGames = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonSaveMarked = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonSaveAll = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonLoad = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonClearList = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonClose = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonGO = new DevExpress.XtraEditors.SimpleButton();
            this.memoEditLog = new DevExpress.XtraEditors.MemoEdit();
            this.textEditInterval = new DevExpress.XtraEditors.TextEdit();
            this.labelControlInterval = new DevExpress.XtraEditors.LabelControl();
            this.simpleButtonSetIds = new DevExpress.XtraEditors.SimpleButton();
            this.textEditStartingID = new DevExpress.XtraEditors.TextEdit();
            this.labelControlStartingID = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            this.contextMenuStripGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sportGameBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sportEventBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEditLog.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditInterval.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditStartingID.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButtonShowBrowser
            // 
            this.simpleButtonShowBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonShowBrowser.Location = new System.Drawing.Point(597, 12);
            this.simpleButtonShowBrowser.Name = "simpleButtonShowBrowser";
            this.simpleButtonShowBrowser.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonShowBrowser.TabIndex = 2;
            this.simpleButtonShowBrowser.Text = "Show / Hide Browser Form";
            this.simpleButtonShowBrowser.Click += new System.EventHandler(this.simpleButtonShowBrowser_Click);
            // 
            // simpleButtonParsePage
            // 
            this.simpleButtonParsePage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonParsePage.Location = new System.Drawing.Point(597, 70);
            this.simpleButtonParsePage.Name = "simpleButtonParsePage";
            this.simpleButtonParsePage.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonParsePage.TabIndex = 4;
            this.simpleButtonParsePage.Text = "Parse Data";
            this.simpleButtonParsePage.Visible = false;
            this.simpleButtonParsePage.Click += new System.EventHandler(this.simpleButtonParseData_Click);
            // 
            // gridControl
            // 
            this.gridControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl.ContextMenuStrip = this.contextMenuStripGrid;
            this.gridControl.DataSource = this.sportGameBindingSource;
            this.gridControl.Location = new System.Drawing.Point(12, 12);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(579, 284);
            this.gridControl.TabIndex = 0;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // contextMenuStripGrid
            // 
            this.contextMenuStripGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveLayoutToolStripMenuItem,
            this.loadLayoutToolStripMenuItem});
            this.contextMenuStripGrid.Name = "contextMenuStripGrid";
            this.contextMenuStripGrid.Size = new System.Drawing.Size(140, 76);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pDFToolStripMenuItem,
            this.hTMLToolStripMenuItem,
            this.xLSToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // pDFToolStripMenuItem
            // 
            this.pDFToolStripMenuItem.Name = "pDFToolStripMenuItem";
            this.pDFToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.pDFToolStripMenuItem.Text = "PDF";
            this.pDFToolStripMenuItem.Click += new System.EventHandler(this.pDFToolStripMenuItem_Click);
            // 
            // hTMLToolStripMenuItem
            // 
            this.hTMLToolStripMenuItem.Name = "hTMLToolStripMenuItem";
            this.hTMLToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.hTMLToolStripMenuItem.Text = "HTML";
            this.hTMLToolStripMenuItem.Click += new System.EventHandler(this.hTMLToolStripMenuItem_Click);
            // 
            // xLSToolStripMenuItem
            // 
            this.xLSToolStripMenuItem.Name = "xLSToolStripMenuItem";
            this.xLSToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.xLSToolStripMenuItem.Text = "XLS";
            this.xLSToolStripMenuItem.Click += new System.EventHandler(this.xLSToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(136, 6);
            // 
            // saveLayoutToolStripMenuItem
            // 
            this.saveLayoutToolStripMenuItem.Name = "saveLayoutToolStripMenuItem";
            this.saveLayoutToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.saveLayoutToolStripMenuItem.Text = "Save Layout";
            this.saveLayoutToolStripMenuItem.Click += new System.EventHandler(this.saveLayoutToolStripMenuItem_Click);
            // 
            // loadLayoutToolStripMenuItem
            // 
            this.loadLayoutToolStripMenuItem.Name = "loadLayoutToolStripMenuItem";
            this.loadLayoutToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.loadLayoutToolStripMenuItem.Text = "Load Layout";
            this.loadLayoutToolStripMenuItem.Click += new System.EventHandler(this.loadLayoutToolStripMenuItem_Click);
            // 
            // sportGameBindingSource
            // 
            this.sportGameBindingSource.DataSource = typeof(premier.parser.SportGame);
            // 
            // gridView
            // 
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colId,
            this.colSport,
            this.colDate,
            this.colTime,
            this.colLeague,
            this.colHome,
            this.colAway});
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.ReadOnly = true;
            this.gridView.OptionsSelection.MultiSelect = true;
            this.gridView.OptionsView.ShowAutoFilterRow = true;
            this.gridView.MasterRowExpanded += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gridView_MasterRowExpanded);
            // 
            // colId
            // 
            this.colId.FieldName = "Id";
            this.colId.Name = "colId";
            this.colId.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colId.Visible = true;
            this.colId.VisibleIndex = 0;
            // 
            // colSport
            // 
            this.colSport.FieldName = "Sport";
            this.colSport.Name = "colSport";
            this.colSport.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colSport.Visible = true;
            this.colSport.VisibleIndex = 1;
            // 
            // colDate
            // 
            this.colDate.FieldName = "Date";
            this.colDate.Name = "colDate";
            this.colDate.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colDate.Visible = true;
            this.colDate.VisibleIndex = 5;
            // 
            // colTime
            // 
            this.colTime.FieldName = "Time";
            this.colTime.Name = "colTime";
            this.colTime.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colTime.Visible = true;
            this.colTime.VisibleIndex = 6;
            // 
            // colLeague
            // 
            this.colLeague.FieldName = "League";
            this.colLeague.Name = "colLeague";
            this.colLeague.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colLeague.Visible = true;
            this.colLeague.VisibleIndex = 2;
            // 
            // colHome
            // 
            this.colHome.FieldName = "Home";
            this.colHome.Name = "colHome";
            this.colHome.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colHome.Visible = true;
            this.colHome.VisibleIndex = 3;
            // 
            // colAway
            // 
            this.colAway.FieldName = "Away";
            this.colAway.Name = "colAway";
            this.colAway.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colAway.Visible = true;
            this.colAway.VisibleIndex = 4;
            // 
            // sportEventBindingSource
            // 
            this.sportEventBindingSource.DataSource = typeof(premier.parser.SportEvent);
            // 
            // simpleButtonShowAllGames
            // 
            this.simpleButtonShowAllGames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonShowAllGames.Location = new System.Drawing.Point(597, 41);
            this.simpleButtonShowAllGames.Name = "simpleButtonShowAllGames";
            this.simpleButtonShowAllGames.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonShowAllGames.TabIndex = 3;
            this.simpleButtonShowAllGames.Text = "Show / Hide All Games";
            this.simpleButtonShowAllGames.Visible = false;
            this.simpleButtonShowAllGames.Click += new System.EventHandler(this.simpleButtonShowAllGames_Click);
            // 
            // simpleButtonSaveMarked
            // 
            this.simpleButtonSaveMarked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonSaveMarked.Location = new System.Drawing.Point(597, 99);
            this.simpleButtonSaveMarked.Name = "simpleButtonSaveMarked";
            this.simpleButtonSaveMarked.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonSaveMarked.TabIndex = 5;
            this.simpleButtonSaveMarked.Text = "Save items with marked IDs";
            this.simpleButtonSaveMarked.Click += new System.EventHandler(this.simpleButtonSaveMarkedItems_Click);
            // 
            // simpleButtonSaveAll
            // 
            this.simpleButtonSaveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonSaveAll.Location = new System.Drawing.Point(597, 128);
            this.simpleButtonSaveAll.Name = "simpleButtonSaveAll";
            this.simpleButtonSaveAll.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonSaveAll.TabIndex = 6;
            this.simpleButtonSaveAll.Text = "Save All";
            this.simpleButtonSaveAll.Click += new System.EventHandler(this.simpleButtonSaveAll_Click);
            // 
            // simpleButtonLoad
            // 
            this.simpleButtonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonLoad.Location = new System.Drawing.Point(597, 157);
            this.simpleButtonLoad.Name = "simpleButtonLoad";
            this.simpleButtonLoad.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonLoad.TabIndex = 7;
            this.simpleButtonLoad.Text = "Load JSON";
            this.simpleButtonLoad.Click += new System.EventHandler(this.simpleButtonLoad_Click);
            // 
            // simpleButtonClearList
            // 
            this.simpleButtonClearList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonClearList.Location = new System.Drawing.Point(597, 186);
            this.simpleButtonClearList.Name = "simpleButtonClearList";
            this.simpleButtonClearList.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonClearList.TabIndex = 8;
            this.simpleButtonClearList.Text = "Clear List";
            this.simpleButtonClearList.Click += new System.EventHandler(this.simpleButtonClearList_Click);
            // 
            // simpleButtonClose
            // 
            this.simpleButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonClose.Location = new System.Drawing.Point(597, 407);
            this.simpleButtonClose.Name = "simpleButtonClose";
            this.simpleButtonClose.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonClose.TabIndex = 14;
            this.simpleButtonClose.Text = "Close";
            this.simpleButtonClose.Click += new System.EventHandler(this.simpleButtonClose_Click);
            // 
            // simpleButtonGO
            // 
            this.simpleButtonGO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonGO.Location = new System.Drawing.Point(597, 338);
            this.simpleButtonGO.Name = "simpleButtonGO";
            this.simpleButtonGO.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonGO.TabIndex = 13;
            this.simpleButtonGO.Text = "Go!";
            this.simpleButtonGO.Click += new System.EventHandler(this.simpleButtonGO_Click);
            // 
            // memoEditLog
            // 
            this.memoEditLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memoEditLog.Location = new System.Drawing.Point(12, 302);
            this.memoEditLog.Name = "memoEditLog";
            this.memoEditLog.Size = new System.Drawing.Size(579, 128);
            this.memoEditLog.TabIndex = 1;
            // 
            // textEditInterval
            // 
            this.textEditInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textEditInterval.EditValue = "1000";
            this.textEditInterval.Location = new System.Drawing.Point(688, 312);
            this.textEditInterval.Name = "textEditInterval";
            this.textEditInterval.Properties.Appearance.Options.UseTextOptions = true;
            this.textEditInterval.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.textEditInterval.Size = new System.Drawing.Size(84, 20);
            this.textEditInterval.TabIndex = 12;
            // 
            // labelControlInterval
            // 
            this.labelControlInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControlInterval.Location = new System.Drawing.Point(597, 315);
            this.labelControlInterval.Name = "labelControlInterval";
            this.labelControlInterval.Size = new System.Drawing.Size(66, 13);
            this.labelControlInterval.TabIndex = 11;
            this.labelControlInterval.Text = "Interval (ms):";
            // 
            // simpleButtonSetIds
            // 
            this.simpleButtonSetIds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonSetIds.Location = new System.Drawing.Point(597, 273);
            this.simpleButtonSetIds.Name = "simpleButtonSetIds";
            this.simpleButtonSetIds.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonSetIds.TabIndex = 8;
            this.simpleButtonSetIds.Text = "Set IDs";
            this.simpleButtonSetIds.Click += new System.EventHandler(this.simpleButtonSetIds_Click);
            // 
            // textEditStartingID
            // 
            this.textEditStartingID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textEditStartingID.EditValue = "0000";
            this.textEditStartingID.Location = new System.Drawing.Point(688, 247);
            this.textEditStartingID.Name = "textEditStartingID";
            this.textEditStartingID.Properties.Appearance.Options.UseTextOptions = true;
            this.textEditStartingID.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.textEditStartingID.Size = new System.Drawing.Size(84, 20);
            this.textEditStartingID.TabIndex = 12;
            // 
            // labelControlStartingID
            // 
            this.labelControlStartingID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControlStartingID.Location = new System.Drawing.Point(597, 250);
            this.labelControlStartingID.Name = "labelControlStartingID";
            this.labelControlStartingID.Size = new System.Drawing.Size(56, 13);
            this.labelControlStartingID.TabIndex = 11;
            this.labelControlStartingID.Text = "Starting ID:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 442);
            this.Controls.Add(this.memoEditLog);
            this.Controls.Add(this.simpleButtonClose);
            this.Controls.Add(this.labelControlInterval);
            this.Controls.Add(this.labelControlStartingID);
            this.Controls.Add(this.textEditInterval);
            this.Controls.Add(this.textEditStartingID);
            this.Controls.Add(this.gridControl);
            this.Controls.Add(this.simpleButtonClearList);
            this.Controls.Add(this.simpleButtonLoad);
            this.Controls.Add(this.simpleButtonSetIds);
            this.Controls.Add(this.simpleButtonGO);
            this.Controls.Add(this.simpleButtonSaveAll);
            this.Controls.Add(this.simpleButtonSaveMarked);
            this.Controls.Add(this.simpleButtonParsePage);
            this.Controls.Add(this.simpleButtonShowAllGames);
            this.Controls.Add(this.simpleButtonShowBrowser);
            this.Name = "MainForm";
            this.Text = "tipico - basic";
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            this.contextMenuStripGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sportGameBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sportEventBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEditLog.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditInterval.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditStartingID.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButtonShowBrowser;
        private DevExpress.XtraEditors.SimpleButton simpleButtonParsePage;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private System.Windows.Forms.BindingSource sportEventBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colSport;
        private DevExpress.XtraGrid.Columns.GridColumn colDate;
        private DevExpress.XtraGrid.Columns.GridColumn colTime;
        private DevExpress.XtraGrid.Columns.GridColumn colLeague;
        private DevExpress.XtraGrid.Columns.GridColumn colHome;
        private DevExpress.XtraGrid.Columns.GridColumn colAway;
        private System.Windows.Forms.BindingSource sportGameBindingSource;
        private DevExpress.XtraEditors.SimpleButton simpleButtonShowAllGames;
        private DevExpress.XtraEditors.SimpleButton simpleButtonSaveMarked;
        private DevExpress.XtraEditors.SimpleButton simpleButtonSaveAll;
        private DevExpress.XtraEditors.SimpleButton simpleButtonLoad;
        private DevExpress.XtraEditors.SimpleButton simpleButtonClearList;
        private DevExpress.XtraEditors.SimpleButton simpleButtonClose;
        private DevExpress.XtraEditors.SimpleButton simpleButtonGO;
        private DevExpress.XtraEditors.MemoEdit memoEditLog;
        private DevExpress.XtraEditors.TextEdit textEditInterval;
        private DevExpress.XtraEditors.LabelControl labelControlInterval;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGrid;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hTMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xLSToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLayoutToolStripMenuItem;
        private DevExpress.XtraEditors.SimpleButton simpleButtonSetIds;
        private DevExpress.XtraEditors.TextEdit textEditStartingID;
        private DevExpress.XtraEditors.LabelControl labelControlStartingID;
    }
}