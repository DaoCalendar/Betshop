namespace premier.tipico.resutls
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
            this.buttonParse = new System.Windows.Forms.Button();
            this.buttonBrowser = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonEuro = new System.Windows.Forms.Button();
            this.buttonFootball = new System.Windows.Forms.Button();
            this.buttonBasketball = new System.Windows.Forms.Button();
            this.buttonHandball = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonParse
            // 
            this.buttonParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonParse.Location = new System.Drawing.Point(665, 492);
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Size = new System.Drawing.Size(150, 23);
            this.buttonParse.TabIndex = 0;
            this.buttonParse.Text = "Parse";
            this.buttonParse.UseVisualStyleBackColor = true;
            this.buttonParse.Click += new System.EventHandler(this.buttonParse_Click);
            // 
            // buttonBrowser
            // 
            this.buttonBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowser.Location = new System.Drawing.Point(665, 34);
            this.buttonBrowser.Name = "buttonBrowser";
            this.buttonBrowser.Size = new System.Drawing.Size(150, 23);
            this.buttonBrowser.TabIndex = 2;
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
            this.tabControl.TabIndex = 15;
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
            // buttonEuro
            // 
            this.buttonEuro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEuro.Location = new System.Drawing.Point(665, 89);
            this.buttonEuro.Name = "buttonEuro";
            this.buttonEuro.Size = new System.Drawing.Size(150, 23);
            this.buttonEuro.TabIndex = 14;
            this.buttonEuro.Text = "EURO 2012";
            this.buttonEuro.UseVisualStyleBackColor = true;
            this.buttonEuro.Click += new System.EventHandler(this.buttonEuro_Click);
            // 
            // buttonFootball
            // 
            this.buttonFootball.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFootball.Location = new System.Drawing.Point(665, 118);
            this.buttonFootball.Name = "buttonFootball";
            this.buttonFootball.Size = new System.Drawing.Size(150, 23);
            this.buttonFootball.TabIndex = 14;
            this.buttonFootball.Text = "Football";
            this.buttonFootball.UseVisualStyleBackColor = true;
            this.buttonFootball.Click += new System.EventHandler(this.buttonFootball_Click);
            // 
            // buttonBasketball
            // 
            this.buttonBasketball.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBasketball.Location = new System.Drawing.Point(665, 147);
            this.buttonBasketball.Name = "buttonBasketball";
            this.buttonBasketball.Size = new System.Drawing.Size(150, 23);
            this.buttonBasketball.TabIndex = 14;
            this.buttonBasketball.Text = "Basketball";
            this.buttonBasketball.UseVisualStyleBackColor = true;
            this.buttonBasketball.Click += new System.EventHandler(this.buttonBasketball_Click);
            // 
            // buttonHandball
            // 
            this.buttonHandball.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonHandball.Location = new System.Drawing.Point(665, 176);
            this.buttonHandball.Name = "buttonHandball";
            this.buttonHandball.Size = new System.Drawing.Size(150, 23);
            this.buttonHandball.TabIndex = 14;
            this.buttonHandball.Text = "Handball";
            this.buttonHandball.UseVisualStyleBackColor = true;
            this.buttonHandball.Click += new System.EventHandler(this.buttonHandball_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 534);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonEuro);
            this.Controls.Add(this.buttonHandball);
            this.Controls.Add(this.buttonBasketball);
            this.Controls.Add(this.buttonFootball);
            this.Controls.Add(this.buttonBrowser);
            this.Controls.Add(this.buttonParse);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageLog.ResumeLayout(false);
            this.tabPageLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonParse;
        private System.Windows.Forms.Button buttonBrowser;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button buttonEuro;
        private System.Windows.Forms.Button buttonFootball;
        private System.Windows.Forms.Button buttonBasketball;
        private System.Windows.Forms.Button buttonHandball;
    }
}