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
            this.simpleButtonShowBrowser = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.contextMenuStripGrid = new System.Windows.Forms.ContextMenuStrip();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hTMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xLSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sportGameBindingSource = new System.Windows.Forms.BindingSource();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSport = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLeague = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHome = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAway = new DevExpress.XtraGrid.Columns.GridColumn();
            this.sportEventBindingSource = new System.Windows.Forms.BindingSource();
            this.simpleButtonSaveMarked = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonSaveAll = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonLoad = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonClearList = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonClose = new DevExpress.XtraEditors.SimpleButton();
            this.memoEditLog = new DevExpress.XtraEditors.MemoEdit();
            this.simpleButtonParseData = new DevExpress.XtraEditors.SimpleButton();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colWinningOdds = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            this.contextMenuStripGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sportGameBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sportEventBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEditLog.Properties)).BeginInit();
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
            this.colAway,
            this.colStatus,
            this.colWinningOdds});
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
            // memoEditLog
            // 
            this.memoEditLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memoEditLog.Location = new System.Drawing.Point(12, 302);
            this.memoEditLog.Name = "memoEditLog";
            this.memoEditLog.Size = new System.Drawing.Size(579, 128);
            this.memoEditLog.TabIndex = 1;
            // 
            // simpleButtonParseData
            // 
            this.simpleButtonParseData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButtonParseData.Location = new System.Drawing.Point(597, 273);
            this.simpleButtonParseData.Name = "simpleButtonParseData";
            this.simpleButtonParseData.Size = new System.Drawing.Size(175, 23);
            this.simpleButtonParseData.TabIndex = 15;
            this.simpleButtonParseData.Text = "Parse Data";
            this.simpleButtonParseData.Click += new System.EventHandler(this.simpleButtonParseData_Click);
            // 
            // colStatus
            // 
            this.colStatus.FieldName = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 7;
            // 
            // colWinningOdds
            // 
            this.colWinningOdds.FieldName = "WinningOdds";
            this.colWinningOdds.Name = "colWinningOdds";
            this.colWinningOdds.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            this.colWinningOdds.Visible = true;
            this.colWinningOdds.VisibleIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 442);
            this.Controls.Add(this.simpleButtonParseData);
            this.Controls.Add(this.memoEditLog);
            this.Controls.Add(this.simpleButtonClose);
            this.Controls.Add(this.gridControl);
            this.Controls.Add(this.simpleButtonClearList);
            this.Controls.Add(this.simpleButtonLoad);
            this.Controls.Add(this.simpleButtonSaveAll);
            this.Controls.Add(this.simpleButtonSaveMarked);
            this.Controls.Add(this.simpleButtonShowBrowser);
            this.Name = "MainForm";
            this.Text = "tipico - results";
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            this.contextMenuStripGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sportGameBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sportEventBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEditLog.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButtonShowBrowser;
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
        private DevExpress.XtraEditors.SimpleButton simpleButtonSaveMarked;
        private DevExpress.XtraEditors.SimpleButton simpleButtonSaveAll;
        private DevExpress.XtraEditors.SimpleButton simpleButtonLoad;
        private DevExpress.XtraEditors.SimpleButton simpleButtonClearList;
        private DevExpress.XtraEditors.SimpleButton simpleButtonClose;
        private DevExpress.XtraEditors.MemoEdit memoEditLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGrid;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hTMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xLSToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLayoutToolStripMenuItem;
        private DevExpress.XtraEditors.SimpleButton simpleButtonParseData;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
        private DevExpress.XtraGrid.Columns.GridColumn colWinningOdds;
    }
}