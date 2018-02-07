namespace ESD.WITS
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnSignIn = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.pnlLogin = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnHeader = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGdIssue = new System.Windows.Forms.PictureBox();
            this.btnGdReceive = new System.Windows.Forms.PictureBox();
            this.pnlSelection = new System.Windows.Forms.Panel();
            this.btnOutbound = new System.Windows.Forms.PictureBox();
            this.btnSignOut = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.pnlGdReceipt = new System.Windows.Forms.Panel();
            this.btnGRClear = new System.Windows.Forms.Button();
            this.txtGRRcvdEun = new System.Windows.Forms.TextBox();
            this.txtGRQtyRcvd = new System.Windows.Forms.TextBox();
            this.labelGRQtyRcvd = new System.Windows.Forms.Label();
            this.txtGROrderedEun = new System.Windows.Forms.TextBox();
            this.cmbBoxGRReason = new System.Windows.Forms.ComboBox();
            this.labelGRReason = new System.Windows.Forms.Label();
            this.txtGREun = new System.Windows.Forms.TextBox();
            this.txtGRQtyOrdered = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtGRMSDesc = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnAddIn = new System.Windows.Forms.Button();
            this.btnMinusIn = new System.Windows.Forms.Button();
            this.txtGRQty = new System.Windows.Forms.TextBox();
            this.labelGRQty = new System.Windows.Forms.Label();
            this.btnGRSubmit = new System.Windows.Forms.Button();
            this.txtGRSAPNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnGRHome = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlGdIssue = new System.Windows.Forms.Panel();
            this.txtGISAPNo = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGIHome = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlLogin.SuspendLayout();
            this.pnlSelection.SuspendLayout();
            this.pnlGdReceipt.SuspendLayout();
            this.pnlGdIssue.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSignIn
            // 
            this.btnSignIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnSignIn.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnSignIn.Location = new System.Drawing.Point(12, 255);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(105, 30);
            this.btnSignIn.TabIndex = 5;
            this.btnSignIn.Text = "Login";
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(9, 126);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(221, 24);
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnExit.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnExit.Location = new System.Drawing.Point(123, 255);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(105, 30);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.AcceptsReturn = true;
            this.txtPassword.AcceptsTab = true;
            this.txtPassword.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.txtPassword.Location = new System.Drawing.Point(96, 200);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(136, 26);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(6, 203);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.Text = "Password:";
            // 
            // txtUserID
            // 
            this.txtUserID.AcceptsReturn = true;
            this.txtUserID.AcceptsTab = true;
            this.txtUserID.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.txtUserID.Location = new System.Drawing.Point(96, 165);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(136, 26);
            this.txtUserID.TabIndex = 2;
            this.txtUserID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUserID_KeyUp);
            // 
            // pnlLogin
            // 
            this.pnlLogin.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pnlLogin.Controls.Add(this.btnSignIn);
            this.pnlLogin.Controls.Add(this.lblVersion);
            this.pnlLogin.Controls.Add(this.btnExit);
            this.pnlLogin.Controls.Add(this.txtPassword);
            this.pnlLogin.Controls.Add(this.label2);
            this.pnlLogin.Controls.Add(this.txtUserID);
            this.pnlLogin.Controls.Add(this.label1);
            this.pnlLogin.Controls.Add(this.btnHeader);
            this.pnlLogin.Controls.Add(this.label3);
            this.pnlLogin.Location = new System.Drawing.Point(3, 3);
            this.pnlLogin.Name = "pnlLogin";
            this.pnlLogin.Size = new System.Drawing.Size(240, 295);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(6, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.Text = "User ID:";
            // 
            // btnHeader
            // 
            this.btnHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnHeader.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.btnHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnHeader.Location = new System.Drawing.Point(0, 0);
            this.btnHeader.Name = "btnHeader";
            this.btnHeader.Size = new System.Drawing.Size(240, 27);
            this.btnHeader.TabIndex = 0;
            this.btnHeader.TabStop = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label3.Location = new System.Drawing.Point(10, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(221, 84);
            this.label3.Text = "Warehouse Inventory Tracking System";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(126, 140);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 19);
            this.label9.Text = "Goods Issue";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(6, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 18);
            this.label4.Text = "Goods Receipt";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnGdIssue
            // 
            this.btnGdIssue.Image = ((System.Drawing.Image)(resources.GetObject("btnGdIssue.Image")));
            this.btnGdIssue.Location = new System.Drawing.Point(137, 58);
            this.btnGdIssue.Name = "btnGdIssue";
            this.btnGdIssue.Size = new System.Drawing.Size(80, 80);
            this.btnGdIssue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnGdIssue.Click += new System.EventHandler(this.btnGI_Click);
            // 
            // btnGdReceive
            // 
            this.btnGdReceive.Image = ((System.Drawing.Image)(resources.GetObject("btnGdReceive.Image")));
            this.btnGdReceive.Location = new System.Drawing.Point(23, 58);
            this.btnGdReceive.Name = "btnGdReceive";
            this.btnGdReceive.Size = new System.Drawing.Size(80, 80);
            this.btnGdReceive.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnGdReceive.Click += new System.EventHandler(this.btnGR_Click);
            // 
            // pnlSelection
            // 
            this.pnlSelection.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pnlSelection.Controls.Add(this.btnOutbound);
            this.pnlSelection.Controls.Add(this.btnSignOut);
            this.pnlSelection.Controls.Add(this.label9);
            this.pnlSelection.Controls.Add(this.label4);
            this.pnlSelection.Controls.Add(this.btnGdIssue);
            this.pnlSelection.Controls.Add(this.btnGdReceive);
            this.pnlSelection.Controls.Add(this.label10);
            this.pnlSelection.Controls.Add(this.label16);
            this.pnlSelection.Location = new System.Drawing.Point(249, 3);
            this.pnlSelection.Name = "pnlSelection";
            this.pnlSelection.Size = new System.Drawing.Size(240, 295);
            this.pnlSelection.Visible = false;
            // 
            // btnOutbound
            // 
            this.btnOutbound.Image = ((System.Drawing.Image)(resources.GetObject("btnOutbound.Image")));
            this.btnOutbound.Location = new System.Drawing.Point(85, 169);
            this.btnOutbound.Name = "btnOutbound";
            this.btnOutbound.Size = new System.Drawing.Size(80, 80);
            this.btnOutbound.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            // 
            // btnSignOut
            // 
            this.btnSignOut.Image = ((System.Drawing.Image)(resources.GetObject("btnSignOut.Image")));
            this.btnSignOut.Location = new System.Drawing.Point(202, 4);
            this.btnSignOut.Name = "btnSignOut";
            this.btnSignOut.Size = new System.Drawing.Size(35, 37);
            this.btnSignOut.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSignOut.Click += new System.EventHandler(this.btnSignOut_Click);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label10.Location = new System.Drawing.Point(3, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(236, 28);
            this.label10.Text = "Menu";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(47, 252);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(155, 21);
            this.label16.Text = "Outbound Delivery";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlGdReceipt
            // 
            this.pnlGdReceipt.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pnlGdReceipt.Controls.Add(this.btnGRClear);
            this.pnlGdReceipt.Controls.Add(this.txtGRRcvdEun);
            this.pnlGdReceipt.Controls.Add(this.txtGRQtyRcvd);
            this.pnlGdReceipt.Controls.Add(this.labelGRQtyRcvd);
            this.pnlGdReceipt.Controls.Add(this.txtGROrderedEun);
            this.pnlGdReceipt.Controls.Add(this.cmbBoxGRReason);
            this.pnlGdReceipt.Controls.Add(this.labelGRReason);
            this.pnlGdReceipt.Controls.Add(this.txtGREun);
            this.pnlGdReceipt.Controls.Add(this.txtGRQtyOrdered);
            this.pnlGdReceipt.Controls.Add(this.label19);
            this.pnlGdReceipt.Controls.Add(this.txtGRMSDesc);
            this.pnlGdReceipt.Controls.Add(this.label17);
            this.pnlGdReceipt.Controls.Add(this.btnAddIn);
            this.pnlGdReceipt.Controls.Add(this.btnMinusIn);
            this.pnlGdReceipt.Controls.Add(this.txtGRQty);
            this.pnlGdReceipt.Controls.Add(this.labelGRQty);
            this.pnlGdReceipt.Controls.Add(this.btnGRSubmit);
            this.pnlGdReceipt.Controls.Add(this.txtGRSAPNo);
            this.pnlGdReceipt.Controls.Add(this.label7);
            this.pnlGdReceipt.Controls.Add(this.btnGRHome);
            this.pnlGdReceipt.Controls.Add(this.label5);
            this.pnlGdReceipt.Location = new System.Drawing.Point(495, 3);
            this.pnlGdReceipt.Name = "pnlGdReceipt";
            this.pnlGdReceipt.Size = new System.Drawing.Size(240, 292);
            this.pnlGdReceipt.Visible = false;
            // 
            // btnGRClear
            // 
            this.btnGRClear.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnGRClear.Location = new System.Drawing.Point(97, 255);
            this.btnGRClear.Name = "btnGRClear";
            this.btnGRClear.Size = new System.Drawing.Size(65, 30);
            this.btnGRClear.TabIndex = 7;
            this.btnGRClear.Text = "Clear";
            this.btnGRClear.Click += new System.EventHandler(this.btnGRClear_Click);
            // 
            // txtGRRcvdEun
            // 
            this.txtGRRcvdEun.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtGRRcvdEun.Enabled = false;
            this.txtGRRcvdEun.Location = new System.Drawing.Point(197, 145);
            this.txtGRRcvdEun.Name = "txtGRRcvdEun";
            this.txtGRRcvdEun.ReadOnly = true;
            this.txtGRRcvdEun.Size = new System.Drawing.Size(36, 23);
            this.txtGRRcvdEun.TabIndex = 0;
            this.txtGRRcvdEun.TabStop = false;
            // 
            // txtGRQtyRcvd
            // 
            this.txtGRQtyRcvd.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtGRQtyRcvd.Enabled = false;
            this.txtGRQtyRcvd.Location = new System.Drawing.Point(104, 145);
            this.txtGRQtyRcvd.Name = "txtGRQtyRcvd";
            this.txtGRQtyRcvd.ReadOnly = true;
            this.txtGRQtyRcvd.Size = new System.Drawing.Size(89, 23);
            this.txtGRQtyRcvd.TabIndex = 0;
            this.txtGRQtyRcvd.TabStop = false;
            // 
            // labelGRQtyRcvd
            // 
            this.labelGRQtyRcvd.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelGRQtyRcvd.Location = new System.Drawing.Point(7, 149);
            this.labelGRQtyRcvd.Name = "labelGRQtyRcvd";
            this.labelGRQtyRcvd.Size = new System.Drawing.Size(104, 20);
            this.labelGRQtyRcvd.Text = "Qty Received:";
            // 
            // txtGROrderedEun
            // 
            this.txtGROrderedEun.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtGROrderedEun.Enabled = false;
            this.txtGROrderedEun.Location = new System.Drawing.Point(197, 116);
            this.txtGROrderedEun.Name = "txtGROrderedEun";
            this.txtGROrderedEun.ReadOnly = true;
            this.txtGROrderedEun.Size = new System.Drawing.Size(36, 23);
            this.txtGROrderedEun.TabIndex = 0;
            this.txtGROrderedEun.TabStop = false;
            // 
            // cmbBoxGRReason
            // 
            this.cmbBoxGRReason.Location = new System.Drawing.Point(103, 204);
            this.cmbBoxGRReason.Name = "cmbBoxGRReason";
            this.cmbBoxGRReason.Size = new System.Drawing.Size(130, 23);
            this.cmbBoxGRReason.TabIndex = 5;
            // 
            // labelGRReason
            // 
            this.labelGRReason.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelGRReason.Location = new System.Drawing.Point(8, 207);
            this.labelGRReason.Name = "labelGRReason";
            this.labelGRReason.Size = new System.Drawing.Size(74, 20);
            this.labelGRReason.Text = "Reason:";
            // 
            // txtGREun
            // 
            this.txtGREun.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtGREun.Enabled = false;
            this.txtGREun.Location = new System.Drawing.Point(197, 175);
            this.txtGREun.Name = "txtGREun";
            this.txtGREun.ReadOnly = true;
            this.txtGREun.Size = new System.Drawing.Size(36, 23);
            this.txtGREun.TabIndex = 0;
            this.txtGREun.TabStop = false;
            // 
            // txtGRQtyOrdered
            // 
            this.txtGRQtyOrdered.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtGRQtyOrdered.Enabled = false;
            this.txtGRQtyOrdered.Location = new System.Drawing.Point(104, 116);
            this.txtGRQtyOrdered.Name = "txtGRQtyOrdered";
            this.txtGRQtyOrdered.ReadOnly = true;
            this.txtGRQtyOrdered.Size = new System.Drawing.Size(89, 23);
            this.txtGRQtyOrdered.TabIndex = 0;
            this.txtGRQtyOrdered.TabStop = false;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label19.Location = new System.Drawing.Point(7, 90);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(87, 20);
            this.label19.Text = "MS Desc.:";
            // 
            // txtGRMSDesc
            // 
            this.txtGRMSDesc.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtGRMSDesc.Enabled = false;
            this.txtGRMSDesc.ForeColor = System.Drawing.SystemColors.Window;
            this.txtGRMSDesc.Location = new System.Drawing.Point(104, 86);
            this.txtGRMSDesc.Name = "txtGRMSDesc";
            this.txtGRMSDesc.ReadOnly = true;
            this.txtGRMSDesc.Size = new System.Drawing.Size(129, 23);
            this.txtGRMSDesc.TabIndex = 0;
            this.txtGRMSDesc.TabStop = false;
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(7, 120);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(104, 20);
            this.label17.Text = "Qty Ordered:";
            // 
            // btnAddIn
            // 
            this.btnAddIn.Location = new System.Drawing.Point(170, 175);
            this.btnAddIn.Name = "btnAddIn";
            this.btnAddIn.Size = new System.Drawing.Size(23, 23);
            this.btnAddIn.TabIndex = 4;
            this.btnAddIn.Text = "+";
            this.btnAddIn.Click += new System.EventHandler(this.btnAddIn_Click);
            // 
            // btnMinusIn
            // 
            this.btnMinusIn.Location = new System.Drawing.Point(104, 175);
            this.btnMinusIn.Name = "btnMinusIn";
            this.btnMinusIn.Size = new System.Drawing.Size(23, 23);
            this.btnMinusIn.TabIndex = 2;
            this.btnMinusIn.Text = "-";
            this.btnMinusIn.Click += new System.EventHandler(this.btnMinusIn_Click);
            // 
            // txtGRQty
            // 
            this.txtGRQty.Location = new System.Drawing.Point(126, 175);
            this.txtGRQty.MaxLength = 4;
            this.txtGRQty.Name = "txtGRQty";
            this.txtGRQty.Size = new System.Drawing.Size(45, 23);
            this.txtGRQty.TabIndex = 3;
            this.txtGRQty.TextChanged += new System.EventHandler(this.txtGRQty_TextChanged);
            this.txtGRQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumeric_KeyPress);
            // 
            // labelGRQty
            // 
            this.labelGRQty.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelGRQty.Location = new System.Drawing.Point(7, 178);
            this.labelGRQty.Name = "labelGRQty";
            this.labelGRQty.Size = new System.Drawing.Size(100, 20);
            this.labelGRQty.Text = "Quantity:";
            // 
            // btnGRSubmit
            // 
            this.btnGRSubmit.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnGRSubmit.Location = new System.Drawing.Point(168, 255);
            this.btnGRSubmit.Name = "btnGRSubmit";
            this.btnGRSubmit.Size = new System.Drawing.Size(65, 30);
            this.btnGRSubmit.TabIndex = 6;
            this.btnGRSubmit.Text = "Post";
            this.btnGRSubmit.Click += new System.EventHandler(this.btnGRSubmit_Click);
            // 
            // txtGRSAPNo
            // 
            this.txtGRSAPNo.Location = new System.Drawing.Point(104, 56);
            this.txtGRSAPNo.Name = "txtGRSAPNo";
            this.txtGRSAPNo.Size = new System.Drawing.Size(129, 23);
            this.txtGRSAPNo.TabIndex = 1;
            this.txtGRSAPNo.TabStop = false;
            this.txtGRSAPNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSAPNo_KeyDown);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(7, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 20);
            this.label7.Text = "SAP No:";
            // 
            // btnGRHome
            // 
            this.btnGRHome.Image = ((System.Drawing.Image)(resources.GetObject("btnGRHome.Image")));
            this.btnGRHome.Location = new System.Drawing.Point(11, 248);
            this.btnGRHome.Name = "btnGRHome";
            this.btnGRHome.Size = new System.Drawing.Size(40, 37);
            this.btnGRHome.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnGRHome.Click += new System.EventHandler(this.btnGRHome_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label5.Location = new System.Drawing.Point(3, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(236, 28);
            this.label5.Text = "Goods Receipt";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlGdIssue
            // 
            this.pnlGdIssue.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pnlGdIssue.Controls.Add(this.txtGISAPNo);
            this.pnlGdIssue.Controls.Add(this.label8);
            this.pnlGdIssue.Controls.Add(this.btnGIHome);
            this.pnlGdIssue.Controls.Add(this.label6);
            this.pnlGdIssue.Location = new System.Drawing.Point(741, 3);
            this.pnlGdIssue.Name = "pnlGdIssue";
            this.pnlGdIssue.Size = new System.Drawing.Size(240, 292);
            this.pnlGdIssue.Visible = false;
            // 
            // txtGISAPNo
            // 
            this.txtGISAPNo.Location = new System.Drawing.Point(104, 56);
            this.txtGISAPNo.Name = "txtGISAPNo";
            this.txtGISAPNo.Size = new System.Drawing.Size(129, 23);
            this.txtGISAPNo.TabIndex = 6;
            this.txtGISAPNo.TabStop = false;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(7, 59);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 20);
            this.label8.Text = "SAP No:";
            // 
            // btnGIHome
            // 
            this.btnGIHome.Image = ((System.Drawing.Image)(resources.GetObject("btnGIHome.Image")));
            this.btnGIHome.Location = new System.Drawing.Point(11, 248);
            this.btnGIHome.Name = "btnGIHome";
            this.btnGIHome.Size = new System.Drawing.Size(40, 37);
            this.btnGIHome.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnGIHome.Click += new System.EventHandler(this.btnGIHome_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label6.Location = new System.Drawing.Point(1, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(236, 28);
            this.label6.Text = "Goods Issue";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.MintCream;
            this.ClientSize = new System.Drawing.Size(1247, 610);
            this.ControlBox = false;
            this.Controls.Add(this.pnlGdIssue);
            this.Controls.Add(this.pnlGdReceipt);
            this.Controls.Add(this.pnlSelection);
            this.Controls.Add(this.pnlLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMain";
            this.Text = "Warehouse Inventory Tracking System";
            this.pnlLogin.ResumeLayout(false);
            this.pnlSelection.ResumeLayout(false);
            this.pnlGdReceipt.ResumeLayout(false);
            this.pnlGdIssue.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSignIn;
        internal System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Panel pnlLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox btnGdIssue;
        private System.Windows.Forms.PictureBox btnGdReceive;
        private System.Windows.Forms.Panel pnlSelection;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox btnSignOut;
        private System.Windows.Forms.Panel pnlGdReceipt;
        private System.Windows.Forms.PictureBox btnGRHome;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlGdIssue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnGRSubmit;
        private System.Windows.Forms.TextBox txtGRSAPNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelGRQty;
        private System.Windows.Forms.Button btnAddIn;
        private System.Windows.Forms.Button btnMinusIn;
        private System.Windows.Forms.TextBox txtGRQty;
        private System.Windows.Forms.Button btnHeader;
        private System.Windows.Forms.PictureBox btnOutbound;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbBoxGRReason;
        private System.Windows.Forms.Label labelGRReason;
        private System.Windows.Forms.TextBox txtGREun;
        private System.Windows.Forms.TextBox txtGRQtyOrdered;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtGRMSDesc;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtGROrderedEun;
        private System.Windows.Forms.TextBox txtGRRcvdEun;
        private System.Windows.Forms.TextBox txtGRQtyRcvd;
        private System.Windows.Forms.Label labelGRQtyRcvd;
        private System.Windows.Forms.Button btnGRClear;
        private System.Windows.Forms.PictureBox btnGIHome;
        private System.Windows.Forms.TextBox txtGISAPNo;
        private System.Windows.Forms.Label label8;
    }
}

