namespace NV10Connector
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnInitNV10 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxCOMPort = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxDeviceID = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblAppVer = new System.Windows.Forms.Label();
            this.cbEncrypt = new System.Windows.Forms.CheckBox();
            this.btnTestServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 13);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(452, 409);
            this.textBox1.TabIndex = 0;
            // 
            // btnInitNV10
            // 
            this.btnInitNV10.Location = new System.Drawing.Point(475, 142);
            this.btnInitNV10.Name = "btnInitNV10";
            this.btnInitNV10.Size = new System.Drawing.Size(121, 23);
            this.btnInitNV10.TabIndex = 1;
            this.btnInitNV10.Text = "Init NV 10 SC";
            this.btnInitNV10.UseVisualStyleBackColor = true;
            this.btnInitNV10.Click += new System.EventHandler(this.btnInitNV10_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(472, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select COM port";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cbxCOMPort
            // 
            this.cbxCOMPort.Enabled = false;
            this.cbxCOMPort.FormattingEnabled = true;
            this.cbxCOMPort.Location = new System.Drawing.Point(475, 74);
            this.cbxCOMPort.Name = "cbxCOMPort";
            this.cbxCOMPort.Size = new System.Drawing.Size(121, 21);
            this.cbxCOMPort.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(472, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Device ID";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cbxDeviceID
            // 
            this.cbxDeviceID.Enabled = false;
            this.cbxDeviceID.FormattingEnabled = true;
            this.cbxDeviceID.Location = new System.Drawing.Point(475, 115);
            this.cbxDeviceID.Name = "cbxDeviceID";
            this.cbxDeviceID.Size = new System.Drawing.Size(121, 21);
            this.cbxDeviceID.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(475, 171);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Start/Stop Poll SC";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(475, 200);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Reset SC";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.Location = new System.Drawing.Point(495, 405);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(102, 17);
            this.cbDebug.TabIndex = 10;
            this.cbDebug.Text = "Log level debug";
            this.cbDebug.UseVisualStyleBackColor = true;
            this.cbDebug.CheckedChanged += new System.EventHandler(this.cbDebug_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(472, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "App version:";
            // 
            // lblAppVer
            // 
            this.lblAppVer.AutoSize = true;
            this.lblAppVer.Location = new System.Drawing.Point(475, 33);
            this.lblAppVer.Name = "lblAppVer";
            this.lblAppVer.Size = new System.Drawing.Size(35, 13);
            this.lblAppVer.TabIndex = 12;
            this.lblAppVer.Text = "label4";
            // 
            // cbEncrypt
            // 
            this.cbEncrypt.AutoSize = true;
            this.cbEncrypt.Location = new System.Drawing.Point(495, 382);
            this.cbEncrypt.Name = "cbEncrypt";
            this.cbEncrypt.Size = new System.Drawing.Size(93, 17);
            this.cbEncrypt.TabIndex = 13;
            this.cbEncrypt.Text = "Encrypt comm";
            this.cbEncrypt.UseVisualStyleBackColor = true;
            this.cbEncrypt.CheckedChanged += new System.EventHandler(this.cbEncrypt_CheckedChanged);
            // 
            // btnTestServer
            // 
            this.btnTestServer.Location = new System.Drawing.Point(475, 353);
            this.btnTestServer.Name = "btnTestServer";
            this.btnTestServer.Size = new System.Drawing.Size(121, 23);
            this.btnTestServer.TabIndex = 14;
            this.btnTestServer.Text = "Test server";
            this.btnTestServer.UseVisualStyleBackColor = true;
            this.btnTestServer.Click += new System.EventHandler(this.btnTestServer_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 434);
            this.Controls.Add(this.btnTestServer);
            this.Controls.Add(this.cbEncrypt);
            this.Controls.Add(this.lblAppVer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbDebug);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cbxDeviceID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbxCOMPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnInitNV10);
            this.Controls.Add(this.textBox1);
            this.Name = "MainForm";
            this.Text = "NV10Connector";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnInitNV10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxCOMPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxDeviceID;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblAppVer;
        private System.Windows.Forms.CheckBox cbEncrypt;
        private System.Windows.Forms.Button btnTestServer;
    }
}

