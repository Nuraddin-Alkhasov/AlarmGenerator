namespace AlarmGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.visiwinalabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.PLCName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.visiwinvlabel = new System.Windows.Forms.Label();
            this.hmialarmslabel = new System.Windows.Forms.Label();
            this.hmitagslabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.connection = new System.Windows.Forms.TextBox();
            this.Set = new System.Windows.Forms.GroupBox();
            this.DBList = new System.Windows.Forms.ListBox();
            this.btn_Browse_DB = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.EWPrefix = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.path = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.errorsID = new System.Windows.Forms.TextBox();
            this.btn_Convert = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Browse_UDT = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.UDTList = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.Set.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // visiwinalabel
            // 
            resources.ApplyResources(this.visiwinalabel, "visiwinalabel");
            this.visiwinalabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.visiwinalabel.Name = "visiwinalabel";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // PLCName
            // 
            resources.ApplyResources(this.PLCName, "PLCName");
            this.PLCName.Name = "PLCName";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // visiwinvlabel
            // 
            resources.ApplyResources(this.visiwinvlabel, "visiwinvlabel");
            this.visiwinvlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.visiwinvlabel.Name = "visiwinvlabel";
            // 
            // hmialarmslabel
            // 
            resources.ApplyResources(this.hmialarmslabel, "hmialarmslabel");
            this.hmialarmslabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.hmialarmslabel.Name = "hmialarmslabel";
            // 
            // hmitagslabel
            // 
            resources.ApplyResources(this.hmitagslabel, "hmitagslabel");
            this.hmitagslabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.hmitagslabel.Name = "hmitagslabel";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // connection
            // 
            resources.ApplyResources(this.connection, "connection");
            this.connection.Name = "connection";
            this.connection.TextChanged += new System.EventHandler(this.connection_TextChanged);
            // 
            // Set
            // 
            this.Set.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.Set.Controls.Add(this.DBList);
            this.Set.Controls.Add(this.btn_Browse_DB);
            resources.ApplyResources(this.Set, "Set");
            this.Set.Name = "Set";
            this.Set.TabStop = false;
            // 
            // DBList
            // 
            resources.ApplyResources(this.DBList, "DBList");
            this.DBList.FormattingEnabled = true;
            this.DBList.Name = "DBList";
            // 
            // btn_Browse_DB
            // 
            resources.ApplyResources(this.btn_Browse_DB, "btn_Browse_DB");
            this.btn_Browse_DB.Name = "btn_Browse_DB";
            this.btn_Browse_DB.UseVisualStyleBackColor = true;
            this.btn_Browse_DB.Click += new System.EventHandler(this.btn_Browse_DB_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xlsx";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.Multiselect = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.EWPrefix);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.path);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.errorsID);
            this.groupBox2.Controls.Add(this.visiwinalabel);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.PLCName);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.visiwinvlabel);
            this.groupBox2.Controls.Add(this.hmialarmslabel);
            this.groupBox2.Controls.Add(this.hmitagslabel);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.connection);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btn_Convert);
            this.groupBox2.Controls.Add(this.progressBar1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBox2
            // 
            resources.ApplyResources(this.checkBox2, "checkBox2");
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // EWPrefix
            // 
            resources.ApplyResources(this.EWPrefix, "EWPrefix");
            this.EWPrefix.Name = "EWPrefix";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // path
            // 
            resources.ApplyResources(this.path, "path");
            this.path.Name = "path";
            this.path.TextChanged += new System.EventHandler(this.path_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // errorsID
            // 
            resources.ApplyResources(this.errorsID, "errorsID");
            this.errorsID.Name = "errorsID";
            // 
            // btn_Convert
            // 
            resources.ApplyResources(this.btn_Convert, "btn_Convert");
            this.btn_Convert.Name = "btn_Convert";
            this.btn_Convert.UseVisualStyleBackColor = true;
            this.btn_Convert.Click += new System.EventHandler(this.btn_Convert_Click);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // btn_Browse_UDT
            // 
            resources.ApplyResources(this.btn_Browse_UDT, "btn_Browse_UDT");
            this.btn_Browse_UDT.Name = "btn_Browse_UDT";
            this.btn_Browse_UDT.UseVisualStyleBackColor = true;
            this.btn_Browse_UDT.Click += new System.EventHandler(this.btn_Browse_UDT_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.groupBox3.Controls.Add(this.UDTList);
            this.groupBox3.Controls.Add(this.btn_Browse_UDT);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // UDTList
            // 
            resources.ApplyResources(this.UDTList, "UDTList");
            this.UDTList.FormattingEnabled = true;
            this.UDTList.Name = "UDTList";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(47)))));
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "xlsx";
            resources.ApplyResources(this.openFileDialog2, "openFileDialog2");
            this.openFileDialog2.Multiselect = true;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Set);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Set.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label visiwinalabel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox PLCName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label visiwinvlabel;
        private System.Windows.Forms.Label hmialarmslabel;
        private System.Windows.Forms.Label hmitagslabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox connection;
        private System.Windows.Forms.GroupBox Set;
        private System.Windows.Forms.ListBox DBList;
        private System.Windows.Forms.Button btn_Browse_DB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_Convert;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_Browse_UDT;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox UDTList;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox errorsID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox path;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox EWPrefix;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

