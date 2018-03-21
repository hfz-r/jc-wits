using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using ESD.WITS.Model;
using ESD.WITS.Helper;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace ESD.WITS
{
    public partial class frmMain : Form
    {
        private class GI
        {
            public int ID { get; set; }     
            public string SAPNo { get; set; }
            public string Qty { get; set; }
            public string ENDesc { get; set; }
        }

        private class AHUFCU
        {
            public int ID { get; set; }
            public string Project { get; set; }
            public string UnitTag { get; set; }
            public string PartNo { get; set; }
            public string Model { get; set; }
            public string Item { get; set; }
            public int Section { get; set; }
            public int Qty { get; set; }
            public int QtyRcvd { get; set; }
            public int? Country { get; set; }
            public int? Location { get; set; }
        }

        private class Location
        {
            public int ID { get; set; }
            public string Loc { get; set; }
        }

        public enum TransferType
        {
            TRANSFER_PROD = 0,
            TRANSFER_POST = 1
        }

        #region Variable

        private Color placeHolderDefaultColor = Color.Black;
        private Color defaultColor = Color.Black;
        private const string placeHolder = "Scan...";       
        private string userID = string.Empty;
        private string gStrProgPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\";
        private string gStrSQLServer = string.Empty;
        private string gStrDBName = string.Empty;
        private string gStrSQLUser = string.Empty;
        private string gStrSQLPwd = string.Empty;
        private string connectionString = "Data Source=192.168.56.1,5050;Initial Catalog=ESD_WITS;Trusted_Connection=Yes;User ID=sa;Password=p@ssw0rd;Persist Security Info=False;Integrated Security=False;";
        //private string connectionString = @"Data Source=172.20.31.218,1433;Initial Catalog=ESD_WITS;Trusted_Connection=Yes;User ID=sa;Password=p@ssw0rd;Persist Security Info=False;Integrated Security=False;";
        private int GRID = 0;
        private bool isPartialTxn = false;
        private List<GI> GIList = new List<GI>();
        private List<Location> LocationList = new List<Location>();
        private string SelectedSAP = string.Empty;
        private string SelectedQty = string.Empty;
        private TransferType transferType = new TransferType();
        private string SLoc = string.Empty;
        private string ENMatShortText = string.Empty;
        private int FGQtyBal = 0;
        private AHUFCU AHUFCURec = new AHUFCU();
        private bool isShip = false;
        #endregion

        #region Initialization

        public frmMain()
        {
            InitializeComponent();
            GetXMLSetting();
            pnlLogin.Visible = true;
            pnlLogin.Dock = DockStyle.Fill;
            lblVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            txtUserID.Focus();
            txtUserID.SelectAll();
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Get Connection string by xml file
        /// </summary>
        private void GetXMLSetting()
        {
            if (File.Exists(gStrProgPath + @"\Config.XML") == false)
            {
                return;
            }

            FileStream filePath = new FileStream(gStrProgPath + @"\Config.XML", FileMode.Open);
            System.Xml.XmlTextReader configReader = new System.Xml.XmlTextReader(filePath);
            string strElementName = "";

            while (configReader.Read())
            {
                if (configReader.NodeType==XmlNodeType.Element)
                {
                    strElementName = configReader.Name;
                }
                else if (configReader.NodeType == XmlNodeType.Text)
                {
                    switch (strElementName.ToUpper().ToString().Trim())
                    {
                        case "SQLSERVERNAME":
                            gStrSQLServer = configReader.Value;
                            break;
                        case "SQLDATABASENAME":
                            gStrDBName = configReader.Value;
                            break;
                        case "SQLUSERID":
                            gStrSQLUser = configReader.Value;
                            break;
                        case "SQLPASSWORD":
                            gStrSQLPwd = configReader.Value;
                            break;
                    }
                }                
            }
            configReader.Close();
            filePath.Close();

            connectionString = @"Data Source=" + gStrSQLServer + ";Initial Catalog=" + gStrDBName + ";Trusted_Connection=Yes;User ID=" + gStrSQLUser + ";Password=" + gStrSQLPwd + ";Persist Security Info=False;Integrated Security=False;";
        }

        /// <summary>
        /// Encrypt password to MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);

            bs = x.ComputeHash(bs);

            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        #endregion

        #region Event Handler

        #region Key Event

        /// <summary>
        /// Allow only numeric input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
                return;
            }

            if (!Char.IsDigit(e.KeyChar))
            {
                if ((e.KeyChar != '.') &&
                    (e.KeyChar != Convert.ToChar(Keys.Back)))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        ///// <summary>
        ///// Click Enter to submit transaction
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void txtQtyOut_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
        //    {
        //        if (!string.IsNullOrEmpty(txtQtyOut.Text))
        //        {
        //            btnStockOutSubmit_Click(sender, e);
        //        }
        //    }
        //}

        #endregion

        #region Login Page

        /// <summary>
        /// Click Enter to login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUserID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    btnSignIn_Click(sender, e);
                }
                else if (!string.IsNullOrEmpty(txtUserID.Text))
                {
                    txtPassword.Focus();
                }
            }
        }

        /// <summary>
        /// Click Enter to login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                btnSignIn_Click(sender, e);
            }

        }

        /// <summary>
        /// Close application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want exit?", "Application Terminate", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                Application.Exit();
        }

        /// <summary>
        /// Sign in to application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            bool isPass = false;
            string username = txtUserID.Text;
            string password = txtPassword.Text;
            SqlConnection connection = null;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Enter User ID.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                txtUserID.Focus();
                txtUserID.SelectAll();
                return;
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Enter Password.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                txtPassword.Focus();
                txtPassword.SelectAll();
                return;
            }

            try
            {
                // Create the command 
                string sSQL = string.Empty;
                string RoleID = string.Empty;
                bool? isAllowed = false;
                string pass = HashConverter.CalculateHash(password, username);

                sSQL = "SELECT ID, RoleID ";
                sSQL += "FROM Users ";
                sSQL += "WHERE Username = '" + username + "' AND Password = '" + pass + "';";

                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sSQL, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            isPass = true;
                            userID = reader[0].ToString();
                            RoleID = reader[1].ToString();
                        }
                    }
                    connection.Close();
                }

                if (isPass)
                {
                    sSQL = "SELECT MACT.IsAllow";
                    sSQL += " FROM [ESD_WITS].[dbo].[ModuleAccessCtrl] MAC";
                    sSQL += " INNER JOIN [dbo].[ModuleAccessCtrlTransaction] MACT";
                    sSQL += " ON MAC.ID = MACT.ModuleID";
                    sSQL += " WHERE Module LIKE '%Handheld%'";
                    sSQL += " AND RoleID = " + RoleID;

                    using (connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sSQL, connection);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.Read())
                            {
                                isPass = true;
                                isAllowed = (bool?)reader[0];
                            }
                        }
                        connection.Close();
                    }

                    if (isAllowed ?? true)
                    {
                        pnlLogin.Visible = false;
                        pnlSelection.Visible = true;
                        pnlSelection.Dock = DockStyle.Fill;
                    }
                    else
                    {
                        MessageBox.Show("You have insufficient privilege to access this application.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);                    
                    }
                }
                else
                {
                    MessageBox.Show("Invalid login", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    txtUserID.Focus();
                    txtUserID.SelectAll();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                txtUserID.Focus();
                txtUserID.SelectAll();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Sign out from application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignOut_Click(object sender, EventArgs e)
        {
            pnlSelection.Visible = false;
            pnlLogin.Visible = true;
            pnlSelection.Dock = DockStyle.Fill;
            txtUserID.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtUserID.Focus();
        }

        #endregion

        #region Main Menu Page

        /// <summary>
        /// Dashboard selection to Goods Receipt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGR_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            GetReason();
            pnlSelection.Visible = false;
            pnlGdReceipt.Visible = true;
            pnlGdReceipt.Dock = DockStyle.Fill;
            SetGRPlaceholder();
            txtGRSAPNo.Focus();
            txtGRSAPNo.SelectAll();
            txtGRQty.Text = "0";
            ResetNewRecord();//if new record
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Dashboard selection to Goods Issue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGI_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            GetReason();
            pnlSelection.Visible = false;
            pnlGdIssue.Visible = true;
            pnlGdIssue.Dock = DockStyle.Fill;
            SetGIPlaceholder();
            txtGISAPNo.Focus();
            txtGISAPNo.SelectAll();
            btnGINext.Enabled = false;
            btnGIDelete.Enabled = false;
            rdBtnGITfrtoProd.Checked = true;
            rdBtnGITfrPosting.Checked = false;
            Cursor.Current = Cursors.Default;
        }

        #endregion

        #region Good Receive Page

        /// <summary>
        /// Validate respective field before submitting to DB
        /// </summary>
        /// <returns></returns>
        private bool ValidateGR()
        {
            if (string.IsNullOrEmpty(txtGRSAPNo.Text) || txtGRSAPNo.Text == placeHolder)
            {
                MessageBox.Show("SAP No. cannot be empty.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                txtGRSAPNo.Focus();
                txtGRSAPNo.SelectAll();
                return false;
            }
            else if(string.IsNullOrEmpty(txtGRQty.Text))
            {
                MessageBox.Show("Enter Qty", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                txtGRQty.Focus();
                txtGRQty.SelectAll();
                return false;
            }
            else if (!string.IsNullOrEmpty(txtGRQty.Text) && Convert.ToDouble(txtGRQty.Text) == 0)
            {
                MessageBox.Show("Qty must be more than 0", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                txtGRQty.Focus();
                txtGRQty.SelectAll();
                return false;
            }
            else if (isPartialTxn && Convert.ToDouble(txtGRQtyRcvd.Text) < Convert.ToDouble(txtGRQtyOrdered.Text))
            {
                double remaining = Convert.ToDouble(txtGRQtyOrdered.Text) - Convert.ToDouble(txtGRQtyRcvd.Text);
                if (Convert.ToDouble(txtGRQty.Text) > remaining)
                {
                    MessageBox.Show("Qty exceeded", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    txtGRQty.Focus();
                    txtGRQty.SelectAll();
                    return false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtGRQtyOrdered.Text) && Convert.ToDouble(txtGRQty.Text) > Convert.ToDouble(txtGRQtyOrdered.Text))
                {
                    MessageBox.Show("Qty exceeded", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    txtGRQty.Focus();
                    txtGRQty.SelectAll();
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(txtGRQtyOrdered.Text) && !string.IsNullOrEmpty(txtGRQty.Text))
            {
                if (isPartialTxn)
                {
                    double remaining = Convert.ToDouble(txtGRQty.Text) + Convert.ToDouble(txtGRQtyRcvd.Text);
                    if (Convert.ToDouble(txtGRQty.Text) > 0 &&
                        remaining < Convert.ToDouble(txtGRQtyOrdered.Text) &&
                        string.IsNullOrEmpty(cmbBoxGRReason.Text))
                    {
                        MessageBox.Show("Select Reason", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        cmbBoxGRReason.Focus();
                        return false;
                    }
                }
                else
                {
                    if (Convert.ToDouble(txtGRQty.Text) > 0 &&
                        Convert.ToDouble(txtGRQty.Text) < Convert.ToDouble(txtGRQtyOrdered.Text) &&
                        string.IsNullOrEmpty(cmbBoxGRReason.Text))
                    {
                        MessageBox.Show("Select Reason", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        cmbBoxGRReason.Focus();
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Get reason for gr transaction
        /// </summary>
        private void GetReason()
        {
            string sSQL = string.Empty;

            sSQL = "SELECT [ID], [ReasonDesc] ";
            sSQL += "FROM [dbo].[Reason] ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sSQL, connection);

                using (SqlDataAdapter sda = new SqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    cmbBoxGRReason.DataSource = dt;
                    cmbBoxGRReason.DisplayMember = "ReasonDesc";
                    cmbBoxGRReason.ValueMember = "ID";
                    cmbBoxGRReason.SelectedIndex = -1;
                }
                
                connection.Close();
            }
        }

        /// <summary>
        /// Get details by SAP No
        /// </summary>
        private void GetGoodsReceipt()
        {
            Cursor.Current = Cursors.WaitCursor; // set the wait cursor
            string[] temp = null;
            if (!string.IsNullOrEmpty(txtGRSAPNo.Text))
            {
                temp = txtGRSAPNo.Text.Split(';');
                txtGRSAPNo.Text = temp[0];
            }

            txtGRSAPNo.SelectAll();
            isPartialTxn = false;
            string sSQL = string.Empty;
            byte? isTxnCompleted = null;
            string qtyRcvd = string.Empty;
            string qtyOrd = string.Empty; 

            sSQL = "SELECT GR.[ID] ";
            sSQL += ", GR.[MaterialShortText]";
            sSQL += ", ISNULL(GR.[Ok],0)";
            sSQL += ", GR.[Quantity]";
            sSQL += ", GR.[Eun]";
            sSQL += ", ISNULL(SUM(GRT.Quantity),0) AS QtyOrdered";
            sSQL += ", GR.[DeliveryNote]";
            sSQL += ", GR.[BillOfLading]";
            sSQL += ", GR.[PurchaseOrder]";
            sSQL += " FROM [dbo].[GoodsReceive] GR";
            sSQL += " LEFT OUTER JOIN [dbo].[GRTransaction] GRT";
            sSQL += " ON GR.ID = GRT.GRID";
            sSQL += " WHERE GR.[Material] = '" + txtGRSAPNo.Text + "'";
            sSQL += " GROUP BY GR.[ID], GR.[MaterialShortText], GR.[Ok], GR.[Quantity], GR.[Eun], GR.[DeliveryNote], GR.[BillOfLading], GR.[PurchaseOrder]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sSQL, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null && reader.Read())
                    {
                        GRID = Convert.ToInt32(reader[0]);
                        txtGRMSDesc.Text = reader[1].ToString();
                        isTxnCompleted = Convert.ToByte(reader[2]); //var
                        txtGREun.Text = reader[4].ToString();
                        qtyOrd = reader[3].ToString() == "0.00" ? "0" : reader[3].ToString();
                        txtGRQtyOrdered.Text = txtGROrderedEun.Text != "KG" ?
                            (Convert.ToInt64(Math.Floor(Convert.ToDouble(qtyOrd)))).ToString() : qtyOrd.ToString();
                        txtGROrderedEun.Text = reader[4].ToString();
                        txtGRRcvdEun.Text = txtGROrderedEun.Text;
                        qtyRcvd = reader[5].ToString() == "0.00" ? "0" : reader[5].ToString();
                        txtGRQtyRcvd.Text = txtGRRcvdEun.Text != "KG" ? 
                            (Convert.ToInt64(Math.Floor(Convert.ToDouble(qtyRcvd)))).ToString() : qtyRcvd.ToString();
                        txtGRDelNote.Text = reader[6].ToString();
                        txtGRBillLading.Text = reader[7].ToString();
                        txtGRPurchaseOrder.Text = reader[8].ToString();
                        txtGRMSDesc.BackColor = Color.Black;
                        txtGRQtyOrdered.BackColor = Color.Black;
                        txtGROrderedEun.BackColor = Color.Black;
                        txtGRQtyRcvd.BackColor = Color.Black;
                        txtGRRcvdEun.BackColor = Color.Black;
                        txtGREun.BackColor = Color.Black;

                        if (isTxnCompleted != null && Convert.ToBoolean(isTxnCompleted)) //complete
                        {
                            btnGRNext.Enabled = false;
                            labelGRQty.Visible = false;
                            btnGRMinusQty.Visible = false;
                            txtGRQty.Visible = false;
                            btnGRAddQty.Visible = false;
                            txtGREun.Visible = false;
                            labelGRReason.Visible = false;
                            cmbBoxGRReason.Visible = false;
                            labelGRQtyRcvd.Visible = true;
                            txtGRQtyRcvd.Visible = true;
                            txtGRRcvdEun.Visible = true;
                            MessageBox.Show("GR has been completed");
                        }
                        else if (isTxnCompleted == 0) //not complete
                        {
                            if (!string.IsNullOrEmpty(qtyRcvd) && Convert.ToDouble(qtyRcvd) == 0)
                            {
                                isPartialTxn = true;
                                btnGRNext.Enabled = true;
                                labelGRQty.Visible = true;
                                btnGRMinusQty.Visible = true;
                                txtGRQty.Visible = true;
                                btnGRAddQty.Visible = true;
                                txtGREun.Visible = true;
                                labelGRReason.Visible = true;
                                cmbBoxGRReason.Visible = true;
                                labelGRQtyRcvd.Visible = false;
                                txtGRQtyRcvd.Visible = false;
                                txtGRRcvdEun.Visible = false;
                                labelGRQty.Location = new Point(7, 161);
                                btnGRMinusQty.Location = new Point(104, 157);
                                txtGRQty.Location = new Point(126, 157);
                                btnGRAddQty.Location = new Point(170, 157);
                                txtGREun.Location = new Point(197, 157);
                                labelGRReason.Location = new Point(8, 190);
                                cmbBoxGRReason.Location = new Point(103, 187);
                            }
                            else if (!string.IsNullOrEmpty(qtyRcvd) && Convert.ToDouble(qtyRcvd) < Convert.ToDouble(txtGRQtyOrdered.Text))
                            {
                                isPartialTxn = true;
                                btnGRNext.Enabled = true;
                                labelGRQty.Visible = true;
                                btnGRMinusQty.Visible = true;
                                txtGRQty.Visible = true;
                                btnGRAddQty.Visible = true;
                                txtGREun.Visible = true;
                                labelGRReason.Visible = true;
                                cmbBoxGRReason.Visible = true;
                                labelGRQtyRcvd.Visible = true;
                                txtGRQtyRcvd.Visible = true;
                                txtGRRcvdEun.Visible = true;
                                labelGRQty.Location = new Point(7, 190);
                                btnGRMinusQty.Location = new Point(104, 187);
                                txtGRQty.Location = new Point(126, 187);
                                btnGRAddQty.Location = new Point(170, 187);
                                txtGREun.Location = new Point(197, 187);
                                labelGRReason.Location = new Point(8, 219);
                                cmbBoxGRReason.Location = new Point(103, 216);
                            }
                        }

                        if (temp.Length > 1)
                        {
                            txtGRQty.Text = temp[1];
                        }
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        Cursor.Current = Cursors.Default;
                        //if new record
                        ClearCache(false);
                        ResetNewRecord();
                        MessageBox.Show("SAP No does not exist.");
                    }
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Reset label and textbox
        /// </summary>
        private void ResetNewRecord()
        {
            txtGRQty.Text = "0";
            btnGRNext.Enabled = true;
            labelGRQty.Visible = true;
            labelGRQty.Location = new Point(7, 161);
            btnGRMinusQty.Visible = true;
            btnGRMinusQty.Location = new Point(104, 157);
            txtGRQty.Visible = true;
            txtGRQty.Location = new Point(126, 157);
            btnGRAddQty.Visible = true;
            btnGRAddQty.Location = new Point(170, 157);
            txtGREun.Visible = true;
            txtGREun.Location = new Point(197, 157);
            labelGRReason.Visible = true;
            labelGRReason.Location = new Point(8, 190);
            cmbBoxGRReason.Visible = true;
            cmbBoxGRReason.Location = new Point(103, 187);
            labelGRQtyRcvd.Visible = false;
            txtGRQtyRcvd.Visible = false;
            txtGRRcvdEun.Visible = false;
            txtGRSAPNo.Focus();
        }

        /// <summary>
        /// clear state to initial value
        /// </summary>
        /// <param name="clearSAPNo"></param>
        private void ClearCache(bool clearSAPNo)
        {
            txtGRQty.Text = string.Empty;
            txtGREun.Text = string.Empty;
            cmbBoxGRReason.SelectedValue = 0;
            cmbBoxGRReason.SelectedValue = -1;
            txtGRQtyRcvd.Text = string.Empty;
            txtGRRcvdEun.Text = string.Empty;
            txtGRSAPNo.Text = clearSAPNo ? string.Empty : txtGRSAPNo.Text;
            isPartialTxn = false;
            GRID = 0;
            txtGRMSDesc.Text = string.Empty;
            txtGRQtyOrdered.Text = string.Empty;
            txtGROrderedEun.Text = string.Empty;
            txtGRDelNote.Text = string.Empty;
            txtGRBillLading.Text = string.Empty;
        }

        /// <summary>
        /// Set placeholder text on textbox
        /// </summary>
        private void SetGRPlaceholder()
        {
            this.txtGRSAPNo.ForeColor = placeHolderDefaultColor;
            this.txtGRSAPNo.Text = placeHolder;
        }

        /// <summary>
        /// When user scan, automatically load details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSAPNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e != null && e.KeyCode == Keys.Enter)
            {
                ResetNewRecord();
                GetGoodsReceipt();
            }
        }

        /// <summary>
        /// Quantity property change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGRQty_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGRQty.Text) && !string.IsNullOrEmpty(txtGRQtyOrdered.Text))
            {
                if (isPartialTxn)
                {
                    double remaining = Convert.ToDouble(txtGRQty.Text) + Convert.ToDouble(txtGRQtyRcvd.Text);

                    if (remaining == Convert.ToDouble(txtGRQtyOrdered.Text))
                    {
                        cmbBoxGRReason.Visible = false;
                        labelGRReason.Visible = false;
                    }
                    else if (remaining < Convert.ToDouble(txtGRQtyOrdered.Text))
                    {
                        cmbBoxGRReason.Visible = true;
                        labelGRReason.Visible = true;
                    }
                }
                else
                {
                    if (Convert.ToDouble(txtGRQty.Text) < Convert.ToDouble(txtGRQtyOrdered.Text))
                    {
                        cmbBoxGRReason.Visible = true;
                        labelGRReason.Visible = true;
                    }
                    else if (Convert.ToDouble(txtGRQty.Text) == Convert.ToDouble(txtGRQtyOrdered.Text))
                    {
                        cmbBoxGRReason.Visible = false;
                        labelGRReason.Visible = false;
                    }
                }
            }
            else
            {
                cmbBoxGRReason.Visible = false;
                labelGRReason.Visible = false;
            }
        }

        /// <summary>
        /// Reduce quantity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMinusIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGRQty.Text))
            {
                txtGRQty.Text = "0";
            }

            if (txtGREun.Text == "KG")
            {
                double decreasedVal = double.Parse(txtGRQty.Text) - 0.01;

                if (decreasedVal >= 0)
                {
                    txtGRQty.Text = decreasedVal.ToString();
                }
            }
            else
            {
                int decreasedVal = int.Parse(txtGRQty.Text) - 1;

                if (decreasedVal >= 0)
                {
                    txtGRQty.Text = decreasedVal.ToString();
                }
            }
        }

        /// <summary>
        /// Increase Quantity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGRQty.Text))
            {
                txtGRQty.Text = "0";
            }
            if (txtGREun.Text == "KG")
            {
                double increasedVal = double.Parse(txtGRQty.Text) + 0.01;
                if (isPartialTxn && Convert.ToDouble(txtGRQtyRcvd.Text) < Convert.ToDouble(txtGRQtyOrdered.Text))
                {
                    double remaining = Convert.ToDouble(txtGRQtyOrdered.Text) - Convert.ToDouble(txtGRQtyRcvd.Text);
                    if (increasedVal <= remaining)
                    {
                        txtGRQty.Text = increasedVal.ToString();
                    }
                }
                else if (increasedVal <= Convert.ToDouble(txtGRQtyOrdered.Text))
                {
                    txtGRQty.Text = increasedVal.ToString();
                }
            }
            else
            {
                int increasedVal = int.Parse(txtGRQty.Text) + 1;
                if (isPartialTxn && Convert.ToInt32(txtGRQtyRcvd.Text) < Convert.ToInt32(txtGRQtyOrdered.Text))
                {
                    int remaining = Convert.ToInt32(txtGRQtyOrdered.Text) - Convert.ToInt32(txtGRQtyRcvd.Text);
                    if (increasedVal <= remaining)
                    {
                        txtGRQty.Text = increasedVal.ToString();
                    }
                }
                else if (increasedVal <= Convert.ToInt32(txtGRQtyOrdered.Text))
                {
                    txtGRQty.Text = increasedVal.ToString();
                }
            }
        }

        /// <summary>
        /// Clear Cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGRClear_Click(object sender, EventArgs e)
        {
            ClearCache(true);
            ResetNewRecord();
            txtGRSAPNo.Focus();
            txtGRSAPNo.SelectAll();
        }

        /// <summary>
        /// Submit to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGRSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateGR())
            {
                if (string.IsNullOrEmpty(txtGRQtyOrdered.Text))
                {
                    txtSAPNo_KeyDown(null, null);
                }

                if (!string.IsNullOrEmpty(txtGRQtyOrdered.Text) && Convert.ToDouble(txtGRQtyOrdered.Text) > 0)
                {
                    string ReasonID = Convert.ToDouble(txtGRQty.Text) == Convert.ToDouble(txtGRQtyOrdered.Text) ? null : cmbBoxGRReason.Text;
                    if (MessageBox.Show("Confirm to post?", "Goods Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        txtGRDelNote.Text = txtGRDelNote.Text.Replace("'", "''");
                        txtGRBillLading.Text = txtGRBillLading.Text.Replace("'", "''"); 

                        Cursor.Current = Cursors.WaitCursor; // set the wait cursor
                        string sSQL = string.Empty;
                        sSQL += "DECLARE";
                        sSQL += "    @GRID BIGINT = " + GRID;

                        sSQL += " INSERT INTO [dbo].[GRTransaction]";
                        sSQL += " ([ReasonID]";
                        sSQL += " ,[Quantity]";
                        sSQL += " ,[CreatedOn]";
                        sSQL += " ,[CreatedBy]";
                        sSQL += " ,[GRID])";
                        sSQL += " VALUES";
                        sSQL += cmbBoxGRReason.SelectedValue == null ? " (NULL" : " ('" + cmbBoxGRReason.SelectedValue + "'";
                        sSQL += " ,'" + txtGRQty.Text + "'";
                        sSQL += " ,'" + DateTime.Now + "'";
                        sSQL += " ,'" + userID + "'";
                        sSQL += " ," + GRID + ")";

                        sSQL += " UPDATE [dbo].[GoodsReceive]";
                        sSQL += " SET [DocumentDate] = '" + DateTime.Now + "', ";
                        sSQL += " [PostingDate] = '" + dtPickerGRPostingDate.Value + "',";
                        sSQL += " [DeliveryNote] = '" + txtGRDelNote.Text + "', ";
                        sSQL += " [BillOfLading] = '" + txtGRBillLading.Text + "' ";
                        sSQL += " WHERE ID = @GRID";

                        sSQL += " UPDATE [dbo].[GoodsReceive]";
                        sSQL += " SET Ok = 1";
                        sSQL += " WHERE Quantity = (";
                        sSQL += " SELECT SUM([Quantity]) AS TotalQty";
                        sSQL += " FROM [dbo].[GRTransaction]";
                        sSQL += " WHERE GRID = @GRID)";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(sSQL, connection);
                            command.ExecuteReader();
                            Cursor.Current = Cursors.Default;
                            MessageBox.Show("GR Posted");
                            connection.Close();
                        }
                        ResetNewRecord();
                        ClearCache(true);
                        pnlGdReceiptCont.Visible = false;
                        pnlGdReceipt.Visible = true;
                        pnlGdReceipt.Dock = DockStyle.Fill;
                    }
                }
            }
        }

        /// <summary>
        /// Navigate back to home page of Stock In
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGRHome_Click(object sender, EventArgs e)
        {
            txtGRSAPNo.Text = string.Empty;
            txtGRQty.Text = "0";
            pnlGdReceipt.Visible = false;
            pnlSelection.Visible = true;
            pnlSelection.Dock = DockStyle.Fill;
        }

        private void btnGRContClear_Click(object sender, EventArgs e)
        {
            ClearCache(true);
        }

        private void btnGRBack_Click(object sender, EventArgs e)
        {
            pnlGdReceiptCont.Visible = false;
            pnlGdReceipt.Visible = true;
            pnlGdReceipt.Dock = DockStyle.Fill;
        }

        private void btnGRNext_Click(object sender, EventArgs e)
        {
            if (ValidateGR())
            {
                Cursor.Current = Cursors.WaitCursor;
                pnlGdReceipt.Visible = false;
                pnlGdReceiptCont.Visible = true;
                pnlGdReceiptCont.Dock = DockStyle.Fill;
                txtGRDelNote.Focus();
                txtGRDelNote.SelectAll();
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnGRHomeCont_Click(object sender, EventArgs e)
        {
            ClearCache(true);
            pnlGdReceiptCont.Visible = false;
            pnlSelection.Visible = true;
            pnlSelection.Dock = DockStyle.Fill;
        }

        #endregion

        #region Goods Issue

        /// <summary>
        /// Set placeholder text on textbox
        /// </summary>
        private void SetGIPlaceholder()
        {
            this.txtGISAPNo.ForeColor = placeHolderDefaultColor;
            this.txtGISAPNo.Text = placeHolder;
        }

        /// <summary>
        /// get all data
        /// </summary>
        private void GetGoodsIssueForGI()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (!string.IsNullOrEmpty(txtGISAPNo.Text))
                {
                    int ind = txtGISAPNo.Text.IndexOf(";");
                    if (ind != -1)
                    {
                        txtGISAPNo.Text = txtGISAPNo.Text.Substring(0, txtGISAPNo.Text.IndexOf(";"));
                    }
                }

                txtGISAPNo.SelectAll();
                string sSQL = string.Empty;
                txtGIQtyAvbl.Text = string.Empty;
                txtGIQtyAvblEun.Text = string.Empty;
                txtGIQtyEun.Text = string.Empty;

                sSQL = " SELECT GR.[ID], GR.[MaterialShortText], GR.[Quantity], GR.[Eun], GR.[StorageLoc],";
                sSQL += " ISNULL(SUM(GRT.Quantity),0) AS QtyAvailable, ISNULL(SUM(GRT.Quantity),0) -";
                sSQL += " (SELECT ISNULL(SUM(GIT.Quantity),0) AS QtyAvailableGI";
                sSQL += " FROM [dbo].[GoodsReceive] GR ";
                sSQL += " INNER JOIN [dbo].[GITransaction] GIT   ON GR.ID = GIT.GRID ";
                sSQL += " WHERE GR.[Material] = '" + txtGISAPNo.Text + "') AS QtyRemaining, GR.[ENMaterialShortText]";
                sSQL += " FROM [dbo].[GoodsReceive] GR ";
                sSQL += " LEFT OUTER JOIN [dbo].[GRTransaction] GRT   ON GR.ID = GRT.GRID ";
                sSQL += " WHERE GR.[Material] = '" + txtGISAPNo.Text + "' ";
                sSQL += " GROUP BY GR.[ID], GR.[MaterialShortText], GR.[Quantity], GR.[Eun], GR.[StorageLoc], GR.[ENMaterialShortText]";
                sSQL += " HAVING SUM(GRT.Quantity) > 0 ";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sSQL, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Cursor.Current = Cursors.Default;
                        if (reader != null && reader.Read())
                        {
                            GRID = Convert.ToInt32(reader[0]);
                            txtGIQtyAvblEun.Text = reader[3].ToString();
                            txtGIQtyEun.Text = reader[3].ToString();
                            txtGIQty.Text = "0";
                            SLoc = reader[4].ToString();
                            txtGIQtyAvbl.Text = Convert.ToDouble(reader[6]).ToString();
                            ENMatShortText = reader[7].ToString();
                        }
                        else
                        {
                            txtGIQtyAvbl.Text = string.Empty;
                            txtGIQtyAvblEun.Text = string.Empty;
                            txtGIQtyEun.Text = string.Empty;
                            dataGrdGI.DataSource = null;
                            GIList = new List<GI>();
                            MessageBox.Show("Invalid SAP No/GR not complete");
                        }
                    }
                    connection.Close();
                }

                if (!string.IsNullOrEmpty(txtGIQtyAvbl.Text) && Convert.ToDouble(txtGIQtyAvbl.Text) == 0)
                {
                    MessageBox.Show("No Qty available");
                }
            }
            catch(Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// validate submit page
        /// </summary>
        /// <returns></returns>
        private bool ValidateGI()
        {
            if (string.IsNullOrEmpty(txtText.Text))
            {
                MessageBox.Show("Enter Text");
                txtText.Focus();
                return false;
            }
            else if (rdBtnGITfrtoProd.Checked && string.IsNullOrEmpty(txtGIProdNo.Text))
            {
                MessageBox.Show("Enter Production No.");
                txtGIProdNo.Focus();
                return false;
            }
            else if (rdBtnGITfrPosting.Checked && cmbBoxGILocTo.SelectedIndex == -1)
            {
                MessageBox.Show("Select Location To");
                cmbBoxGILocTo.Focus();
                return false;
            }
            else if (rdBtnGITfrPosting.Checked && cmbBoxGILocTo.Text == cmbBoxGILocFrom.Text)
            {
                MessageBox.Show("Location To and From must not be the same");
                cmbBoxGILocTo.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// load location to
        /// </summary>
        private DataTable LoadLocation()
        {
            DataTable dtTo = new DataTable();
            string sSQL = string.Empty;
            sSQL = "SELECT [ID] ";
            sSQL += " ,[LocationDesc] ";
            sSQL += "FROM [dbo].[Location] ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sSQL, connection);
            
                using (SqlDataAdapter sda = new SqlDataAdapter(command))
                {
                    sda.Fill(dtTo);

                    //cmbBoxGILocTo.DataSource = dtTo;
                    //cmbBoxGILocTo.DisplayMember = "LocationDesc";
                    //cmbBoxGILocTo.ValueMember = "ID";
                    //cmbBoxGILocTo.SelectedIndex = -1;

                    //DataTable dtFrom = new DataTable();
                    //sda.Fill(dtFrom);
                    //cmbBoxGILocFrom.DataSource = dtFrom;
                    //cmbBoxGILocFrom.DisplayMember = "LocationDesc";
                    //cmbBoxGILocFrom.ValueMember = "ID";
                    //cmbBoxGILocFrom.SelectedIndex = -1;

                    connection.Close();
                }
            }
            return dtTo;
        }

        private void LoadLocationGITo()
        {
            DataTable dt = LoadLocation();
            cmbBoxGILocTo.DataSource = dt;
            cmbBoxGILocTo.DisplayMember = "LocationDesc";
            cmbBoxGILocTo.ValueMember = "ID";
            cmbBoxGILocTo.SelectedIndex = -1;
        }

        private void LoadLocationGIFrom()
        {
            DataTable dt = LoadLocation();
            cmbBoxGILocFrom.DataSource = dt;
            cmbBoxGILocFrom.DisplayMember = "LocationDesc";
            cmbBoxGILocFrom.ValueMember = "ID";
            cmbBoxGILocFrom.SelectedIndex = -1;
        }

        private void LoadLocationFG()
        {
            DataTable dt = LoadLocation();
            cmbBoxFGLocation.DataSource = dt;
            cmbBoxFGLocation.DisplayMember = "LocationDesc";
            cmbBoxFGLocation.ValueMember = "ID";
            cmbBoxFGLocation.SelectedIndex = -1;
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        private void ClearGICache()
        {
            txtGIQty.Text = string.Empty;
            txtGISAPNo.Text = string.Empty;
            dataGrdGI.DataSource = null;
            txtGIQtyAvbl.Text = string.Empty;
            txtGIQtyAvblEun.Text = string.Empty;
            txtGIQtyEun.Text = string.Empty;
            dataGrdGI.DataSource = null;
            GIList = new List<GI>();
            txtGIProdNo.Text = string.Empty;
            cmbBoxGILocFrom.DataSource = null;
            cmbBoxGILocTo.DataSource = null;
            SLoc = string.Empty;
            rdBtnGITfrtoProd.Checked = true;
            rdBtnGITfrPosting.Checked = false;
            btnGIDelete.Enabled = false;
            btnGINext.Enabled = false;
            txtText.Text = string.Empty;
        }

        /// <summary>
        /// set enable visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnGITfrtoProd_CheckedChanged(object sender, EventArgs e)
        {
            lblGIProdNo.Enabled = true;
            txtGIProdNo.Enabled = true;
            transferType = TransferType.TRANSFER_PROD;
            lblGILocFrom.Enabled = false;
            cmbBoxGILocFrom.Enabled = false;
            lblGILocTo.Enabled = false;
            cmbBoxGILocTo.Enabled = false;
        }

        /// <summary>
        /// set enable visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnGITfrPosting_CheckedChanged(object sender, EventArgs e)
        {
            lblGILocFrom.Enabled = true;
            cmbBoxGILocFrom.Enabled = true;
            lblGILocTo.Enabled = true;
            cmbBoxGILocTo.Enabled = true;
            transferType = TransferType.TRANSFER_POST;
            lblGIProdNo.Enabled = false;
            txtGIProdNo.Enabled = false;
        }

        /// <summary>
        /// Get GI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGISAPNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e != null && e.KeyCode == Keys.Enter)
            {
                GetGoodsIssueForGI();
            }
        }

        /// <summary>
        /// Add quantity by 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGIAddQty_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGIQty.Text))
            {
                txtGIQty.Text = "0";
            }

            if (!string.IsNullOrEmpty(txtGIQtyAvbl.Text))
            {
                if (txtGIQtyAvblEun.Text == "KG")
                {
                    double increasedVal = double.Parse(txtGIQty.Text) + 0.01;

                    if (increasedVal <= Convert.ToDouble(txtGIQtyAvbl.Text))
                    {
                        txtGIQty.Text = increasedVal.ToString();
                    }
                }
                else
                {
                    int increasedVal = int.Parse(txtGIQty.Text) + 1;

                    if (increasedVal <= Convert.ToInt32(txtGIQtyAvbl.Text))
                    {
                        txtGIQty.Text = increasedVal.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Minus quantity by 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btGIMinusQty_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGIQty.Text))
            {
                txtGIQty.Text = "0";
            }

            if (!string.IsNullOrEmpty(txtGIQtyAvbl.Text))
            {
                if (txtGIQtyAvblEun.Text == "KG")
                {
                    double decreasedVal = double.Parse(txtGIQty.Text) - 0.01;

                    if (decreasedVal >= 0)
                    {
                        txtGIQty.Text = decreasedVal.ToString();
                    }
                }
                else
                {
                    int decreasedVal = int.Parse(txtGIQty.Text) - 1;

                    if (decreasedVal >= 0)
                    {
                        txtGIQty.Text = decreasedVal.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Go back menu home page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGIHome_Click(object sender, EventArgs e)
        {
            pnlGdIssue.Visible = false;
            pnlSelection.Visible = true;
            pnlSelection.Dock = DockStyle.Fill;
            ClearGICache();
        }

        /// <summary>
        /// Add to list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGIAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGIQtyAvbl.Text) && !string.IsNullOrEmpty(txtGIQty.Text) && GRID != 0)
            {
                if (Convert.ToDouble(txtGIQty.Text) > Convert.ToDouble(txtGIQtyAvbl.Text))
                {
                    MessageBox.Show("Qty exceeded", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    txtGIQty.Focus();
                    txtGIQty.SelectAll();
                }
                else
                {
                    if (Convert.ToDouble(txtGIQty.Text) > 0)
                    {
                        if (GIList != null && GIList.Count > 0)
                        {
                            bool isExist = false;
                            foreach (var item in GIList)
                            {
                                if (item.SAPNo == txtGISAPNo.Text)
                                {
                                    item.Qty = (Convert.ToDouble(item.Qty) + Convert.ToDouble(txtGIQty.Text)).ToString();
                                    isExist = true;
                                    break;
                                }
                            }
                            if (!isExist)
                            {
                                GIList.Add(new GI
                                { 
                                    ID = GRID,
                                    SAPNo = txtGISAPNo.Text,
                                    Qty = txtGIQty.Text,
                                    ENDesc = ENMatShortText
                                });
                            }
                        }
                        else
                        {
                            GIList.Add(new GI
                            {
                                ID = GRID,
                                SAPNo = txtGISAPNo.Text,
                                Qty = txtGIQty.Text,
                                ENDesc = ENMatShortText
                            });
                        }

                        DataTable t = new DataTable("INVENTORY");
                        DataColumn dc1 = new DataColumn("ENDesc", typeof(string));
                        DataColumn dc2 = new DataColumn("ID", typeof(string));
                        DataColumn dc3 = new DataColumn("SAPNo", typeof(string));
                        DataColumn dc4 = new DataColumn("Qty", typeof(string));
                        t.Columns.Add(dc1);
                        t.Columns.Add(dc2);
                        t.Columns.Add(dc3);
                        t.Columns.Add(dc4);

                        foreach (var item in GIList)
                        {
                            DataRow newRow = t.NewRow();
                            newRow["ENDesc"] = item.ENDesc;
                            newRow["ID"] = item.ID;
                            newRow["SAPNo"] = item.SAPNo;
                            newRow["Qty"] = item.Qty;
                            t.Rows.Add(newRow);
                        }

                        DataGridTableStyle tableStyle = new DataGridTableStyle();
                        //take note of this mapping name, it's very important
                        tableStyle.MappingName = "INVENTORY";

                        //second column
                        DataGridTextBoxColumn gridColumn1 = new DataGridTextBoxColumn();
                        gridColumn1.Width = -1;
                        gridColumn1.MappingName = "ID";
                        gridColumn1.HeaderText = "ID";
                        tableStyle.GridColumnStyles.Add(gridColumn1);

                        //third column
                        DataGridTextBoxColumn gridColumn2 = new DataGridTextBoxColumn();
                        gridColumn2.Width = 80;
                        gridColumn2.MappingName = "SAPNo";
                        gridColumn2.HeaderText = "SAPNo";
                        tableStyle.GridColumnStyles.Add(gridColumn2);

                        //forth column
                        DataGridTextBoxColumn gridColumn3 = new DataGridTextBoxColumn();
                        gridColumn3.Width = 50;
                        gridColumn3.MappingName = "Qty";
                        gridColumn3.HeaderText = "Qty";
                        tableStyle.GridColumnStyles.Add(gridColumn3);

                        //our first column which is the ID
                        DataGridTextBoxColumn gridColumn4 = new DataGridTextBoxColumn();
                        gridColumn4.Width = 300;
                        gridColumn4.MappingName = "ENDesc";
                        gridColumn4.HeaderText = "ENDesc";
                        tableStyle.GridColumnStyles.Add(gridColumn4);

                        dataGrdGI.TableStyles.Clear();
                        dataGrdGI.TableStyles.Add(tableStyle);
                        dataGrdGI.DataSource = t;

                        if (GIList != null && GIList.Count > 0)
                        {
                            btnGINext.Enabled = true;
                            btnGIDelete.Enabled = true;
                        }

                        txtGISAPNo.Text = string.Empty;
                        txtGIQtyAvbl.Text = string.Empty;
                        txtGIQtyAvblEun.Text = string.Empty;
                        txtGIQtyEun.Text = string.Empty;
                        txtGIQty.Text = string.Empty;
                        txtGISAPNo.Focus();
                        txtGISAPNo.SelectAll();
                    }
                    else
                    {
                        MessageBox.Show("Qty must be more than 0");
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtGISAPNo.Text))
                {
                    MessageBox.Show("Enter SAP No");
                }
            }
        }

        /// <summary>
        /// Delete specific row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGIDelete_Click(object sender, EventArgs e)
        {
            if (GIList.Count > 0)
            {
                Point pt = dataGrdGI.PointToClient(Control.MousePosition);
                DataGrid.HitTestInfo info = dataGrdGI.HitTest(pt.X, pt.Y);
                int row;

                if (info.Row < 0)
                    row = 0;
                else
                    row = info.Row;

                object cellData = dataGrdGI[row, 0];
                SelectedSAP = cellData != null ? cellData.ToString() : string.Empty;


                cellData = dataGrdGI[row, 1];
                SelectedQty = cellData != null ? cellData.ToString() : string.Empty;

                if (MessageBox.Show("Confirm to delete?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    int i = dataGrdGI.CurrentRowIndex;

                    //to remove and bind again
                    GIList.RemoveAt(i);
                    dataGrdGI.DataSource = null;
                    dataGrdGI.DataSource = GIList;
                    if (GIList.Count > 0)
                    {
                        dataGrdGI.CurrentRowIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Submit GI to GITransaction db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGISubmit_Click(object sender, EventArgs e)
        {
            if (ValidateGI())
            {
                if (GIList != null && GIList.Count > 0)
                {
                    if (MessageBox.Show("Confirm to submit?", "Good Receive", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        txtGIProdNo.Text = txtGIProdNo.Text.Replace("'", "''");
                        txtText.Text = txtText.Text.Replace("'", "''"); 
                        Cursor.Current = Cursors.WaitCursor;
                        string sSQL = string.Empty;

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            foreach (var item in GIList)
                            {
                                sSQL += " INSERT INTO [dbo].[GITransaction]";
                                sSQL += " ([Text]";
                                sSQL += " ,[Quantity]";
                                sSQL += " ,[CreatedOn]";
                                sSQL += " ,[CreatedBy]";
                                sSQL += " ,[GRID]";
                                sSQL += " ,[TransferType]";
                                sSQL += " ,[ProductionNo]";
                                sSQL += " ,[LocationToID]";
                                sSQL += " ,[LocationFromID])";
                                sSQL += " VALUES";
                                sSQL += " ('" + txtText.Text + "'";
                                sSQL += " ,'" + item.Qty + "'";
                                sSQL += " ,'" + DateTime.Now + "'";
                                sSQL += " ,'" + userID + "'";
                                sSQL += " ,'" + item.ID + "'";
                                sSQL += " ,'" + transferType + "'";
                                sSQL += !string.IsNullOrEmpty(txtGIProdNo.Text) ? " ,'" + txtGIProdNo.Text + "'" : " ,NULL";
                                sSQL += !string.IsNullOrEmpty(cmbBoxGILocTo.Text) ? " ,'" + cmbBoxGILocTo.SelectedValue + "'" : " ,NULL";
                                sSQL += !string.IsNullOrEmpty(cmbBoxGILocFrom.Text) ? " ,'" + cmbBoxGILocFrom.SelectedValue + "')" : " ,NULL)";
                            }
                            connection.Open();
                            SqlCommand command = new SqlCommand(sSQL, connection);
                            command.ExecuteReader();
                            connection.Close();
                        }

                        Cursor.Current = Cursors.Default;
                        MessageBox.Show(txtText.Text + " Submitted");
                    }

                    ClearGICache();
                    pnlGdIssueSubmit.Visible = false;
                    pnlGdIssue.Visible = true;
                    pnlGdIssue.Dock = DockStyle.Fill;
                    txtGISAPNo.Focus();
                    txtGISAPNo.SelectAll();
                }
                else
                {
                    MessageBox.Show("Empty record");
                }
            }
        }
        
        /// <summary>
        /// Return to previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGISubmitBack_Click(object sender, EventArgs e)
        {
            pnlGdIssueSubmit.Visible = false;
            pnlGdIssue.Visible = true;
            pnlGdIssue.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Go to GI Submit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGINext_Click(object sender, EventArgs e)
        {
            if (GIList != null && GIList.Count > 0)
            {
                if (!string.IsNullOrEmpty(txtGIQty.Text) && Convert.ToInt32(txtGIQty.Text) > 0)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    pnlGdIssue.Visible = false;
                    pnlGdIssueSubmit.Visible = true;
                    pnlGdIssueSubmit.Dock = DockStyle.Fill;
                    LoadLocationGITo();
                    LoadLocationGIFrom();
                    txtText.Focus();
                    txtText.SelectAll();
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    if (string.IsNullOrEmpty(txtGIQty.Text))
                    {
                        MessageBox.Show("Enter Qty");
                        txtGIQty.Focus();
                    }
                    else if (!string.IsNullOrEmpty(txtGIQty.Text) && Convert.ToInt32(txtGIQty.Text) == 0)
                    {
                        MessageBox.Show("Qty must be more than 0");
                        txtGIQty.Focus();
                    }
                    else if (!string.IsNullOrEmpty(txtGIQty.Text) && !string.IsNullOrEmpty(txtGIQtyAvbl.Text)
                        && Convert.ToInt32(txtGIQty.Text) > Convert.ToInt32(txtGIQtyAvbl.Text))
                    {
                        MessageBox.Show("Qty exceeded.\nQuantity available: " + Convert.ToInt32(txtGIQtyAvbl.Text));
                        txtGIQty.Focus();
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtGISAPNo.Text))
                {
                    MessageBox.Show("Enter SAP No");
                    txtGISAPNo.Focus();
                }
            }
        }

        #endregion

        #region Outbound Delivery

        /// <summary>
        /// Load country list
        /// </summary>
        private void LoadCountry()
        {
            string sSQL = string.Empty;
            sSQL = "SELECT [ID] ";
            sSQL += " ,[CountryDesc] ";
            sSQL += "FROM [dbo].[Country] ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sSQL, connection);

                using (SqlDataAdapter sda = new SqlDataAdapter(command))
                {
                    DataTable dtCountry = new DataTable();
                    sda.Fill(dtCountry);
                    cmbBoxFGCountry.DataSource = dtCountry;
                    cmbBoxFGCountry.DisplayMember = "CountryDesc";
                    cmbBoxFGCountry.ValueMember = "ID";
                    cmbBoxFGCountry.SelectedIndex = -1;
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Clear FG text
        /// </summary>
        private void ClearFGCache(bool isClear)
        {
            btFGMinusQty.Enabled = true;
            btFGAddQty.Enabled = true;
            txtFGQty.Enabled = true;
            cmbBoxFGLocation.Enabled = true;
            txtFGSerial.Text = string.Empty;
            txtFGQty.Text = string.Empty;
            if (cmbBoxFGCountry.SelectedItem != null)
            {
                cmbBoxFGCountry.SelectedIndex = 0;
                cmbBoxFGCountry.SelectedIndex = -1;
            }
            if (cmbBoxFGLocation.SelectedItem != null)
            {
                cmbBoxFGLocation.SelectedIndex = 0;
                cmbBoxFGLocation.SelectedIndex = -1;
            }
            dataGrdFG.DataSource = null;
            rdBtnAHU.Enabled = true;
            rdBtnFCU.Enabled = true;
            btnFGNext.Enabled = true;
            rdBtnFGTfrtoWarehse.Checked = true;
            rdBtnFGTfrtoWarehse.Enabled = true;
            lblFGLocation.Enabled = true;
            rdBtnFGTfrtoCustomer.Checked = false;
            rdBtnFGTfrtoCustomer.Enabled = false;
            lblFGCountry.Enabled = false;
            FGQtyBal = 0;
            txtFGSerial.Focus();
            if (isClear)
            {
                rdBtnAHU.Checked = true;
                rdBtnFCU.Checked = false;
            }
        }

        /// <summary>
        /// Get FG based on Serial No
        /// </summary>
        private void GetFG()
        {
            Cursor.Current = Cursors.WaitCursor; // set the wait cursor           
            string sSQL = string.Empty;
            txtFGQty.Text = string.Empty;
            if (cmbBoxFGCountry.SelectedItem != null)
            {
                cmbBoxFGCountry.SelectedIndex = 0;
                cmbBoxFGCountry.SelectedIndex = -1;
            }
            if (cmbBoxFGLocation.SelectedItem != null)
            {
                cmbBoxFGLocation.SelectedIndex = 0;
                cmbBoxFGLocation.SelectedIndex = -1;
            }
            cmbBoxFGCountry.Enabled = false;
            lblFGCountry.Enabled = false;
            dataGrdFG.DataSource = null;            

            if (rdBtnAHU.Checked)
            {
                sSQL = "SELECT AHU.[ID]";
                sSQL += ", AHU.[Project]";
                sSQL += ", AHU.[UnitTag]";
                sSQL += ", AHU.[PartNo]";
                sSQL += ", AHU.[Model]";
                sSQL += ", AHU.[Item]";
                sSQL += ", AHU.[Section]";
                sSQL += ", ISNULL(SUM(AHUT.Quantity),0) AS QtyReceived";
                sSQL += ", AHUT.[CountryID]";
                sSQL += ", AHUT.[LocationID]";
                sSQL += " FROM [dbo].[AHU] AHU";
                sSQL += " LEFT OUTER JOIN [dbo].[AHUTransaction] AHUT ON AHU.ID = AHUT.AHUID";
                sSQL += " WHERE [SerialNo] = '" + txtFGSerial.Text + "'";
                sSQL += " GROUP BY AHU.[ID], AHU.[Project], AHU.[UnitTag], AHU.[PartNo], AHU.[Model], AHU.[Item], AHU.[Section], AHUT.[CountryID], AHUT.[LocationID]";
            }
            else if (rdBtnFCU.Checked)
            {
                sSQL = "SELECT FCU.[ID]";
                sSQL += ", FCU.[Project]";
                sSQL += ", FCU.[UnitTag]";
                sSQL += ", FCU.[PartNo]";
                sSQL += ", FCU.[Model]";
                sSQL += ", FCU.[Item]";
                sSQL += ", FCU.[Qty]";
                sSQL += ", ISNULL(SUM(FCUT.Quantity),0) AS QtyReceived";
                sSQL += ", FCUT.[CountryID]";
                sSQL += ", FCUT.[LocationID]";
                sSQL += " FROM [dbo].[FCU] FCU";
                sSQL += " LEFT OUTER JOIN [dbo].[FCUTransaction] FCUT ON FCU.ID = FCUT.FCUID";
                sSQL += " WHERE [SerialNo] = '" + txtFGSerial.Text + "'";
                sSQL += " GROUP BY FCU.[ID], FCU.[Project], FCU.[UnitTag], FCU.[PartNo], FCU.[Model], FCU.[Item], FCU.[Qty], FCUT.[CountryID], FCUT.[LocationID]";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sSQL, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null && reader.Read())
                    {
                        AHUFCURec.ID = Convert.ToInt32(reader[0]);
                        AHUFCURec.Project = reader[1].ToString();
                        AHUFCURec.UnitTag = reader[2].ToString();
                        AHUFCURec.PartNo = reader[3].ToString();
                        AHUFCURec.Model = reader[4].ToString();
                        AHUFCURec.Item = reader[5].ToString();
                        AHUFCURec.QtyRcvd = Convert.ToInt32(reader[7]);
                        AHUFCURec.Country = (reader[8] == System.DBNull.Value) ? (int?)null : int.Parse(reader[8].ToString());
                        AHUFCURec.Location = (reader[9] == System.DBNull.Value) ? (int?)null : int.Parse(reader[9].ToString());

                        btFGMinusQty.Enabled = true;
                        btFGAddQty.Enabled = true;
                        txtFGQty.Enabled = true;
                        cmbBoxFGCountry.Enabled = true;
                        rdBtnFCU.Enabled = true;
                        rdBtnAHU.Enabled = true;

                        if (rdBtnAHU.Checked)
                        {
                            AHUFCURec.Section = Convert.ToInt32(reader[6]);
                            FGQtyBal = AHUFCURec.Section - AHUFCURec.QtyRcvd;
                        }
                        else if (rdBtnFCU.Checked)
                        {
                            AHUFCURec.Qty = Convert.ToInt32(reader[6]);
                            FGQtyBal = AHUFCURec.Qty - AHUFCURec.QtyRcvd;
                        }

                        DataTable t = new DataTable("FG");
                        DataColumn dc1 = new DataColumn("ID", typeof(string));
                        DataColumn dc2 = new DataColumn("Project", typeof(string));
                        DataColumn dc3 = new DataColumn("UnitTag", typeof(string));
                        DataColumn dc4 = new DataColumn("PartNo", typeof(string));
                        DataColumn dc5 = new DataColumn("Model", typeof(string));
                        DataColumn dc6 = new DataColumn("Item", typeof(string));
                        DataColumn dc7 = rdBtnAHU.Checked ? new DataColumn("Section", typeof(string)) : new DataColumn("Qty", typeof(string));
                        DataColumn dc8 = new DataColumn("QtyRcvd", typeof(string));

                        t.Columns.Add(dc1);
                        t.Columns.Add(dc2);
                        t.Columns.Add(dc3);
                        t.Columns.Add(dc4);
                        t.Columns.Add(dc5);
                        t.Columns.Add(dc6);
                        t.Columns.Add(dc7);
                        t.Columns.Add(dc8);

                        DataRow newRow = t.NewRow();
                        newRow["ID"] = AHUFCURec.ID;
                        newRow["Project"] = AHUFCURec.Project;
                        newRow["UnitTag"] = AHUFCURec.UnitTag;
                        newRow["PartNo"] = AHUFCURec.PartNo;
                        newRow["Model"] = AHUFCURec.Model;
                        newRow["Item"] = AHUFCURec.Item;
                        newRow["QtyRcvd"] = AHUFCURec.QtyRcvd;

                        if (rdBtnAHU.Checked)
                        {
                            newRow["Section"] = AHUFCURec.Section;
                        }
                        else if (rdBtnFCU.Checked)
                        {
                            newRow["Qty"] = AHUFCURec.Qty;
                        }
                        t.Rows.Add(newRow);

                        DataGridTableStyle tableStyle = new DataGridTableStyle();
                        tableStyle.MappingName = "FG";

                        DataGridTextBoxColumn gridColumn1 = new DataGridTextBoxColumn();
                        gridColumn1.Width = 0;
                        gridColumn1.MappingName = "ID";
                        gridColumn1.HeaderText = "ID";
                        tableStyle.GridColumnStyles.Add(gridColumn1);

                        DataGridTextBoxColumn gridColumn2 = new DataGridTextBoxColumn();
                        gridColumn2.Width = 200;
                        gridColumn2.MappingName = "Project";
                        gridColumn2.HeaderText = "Project";
                        tableStyle.GridColumnStyles.Add(gridColumn2);

                        DataGridTextBoxColumn gridColumn3 = new DataGridTextBoxColumn();
                        gridColumn3.Width = 150;
                        gridColumn3.MappingName = "UnitTag";
                        gridColumn3.HeaderText = "Unit Tag";
                        tableStyle.GridColumnStyles.Add(gridColumn3);

                        DataGridTextBoxColumn gridColumn4 = new DataGridTextBoxColumn();
                        gridColumn4.Width = 60;
                        gridColumn4.MappingName = "PartNo";
                        gridColumn4.HeaderText = "Part No";
                        tableStyle.GridColumnStyles.Add(gridColumn4);

                        DataGridTextBoxColumn gridColumn5 = new DataGridTextBoxColumn();
                        gridColumn5.Width = 100;
                        gridColumn5.MappingName = "Model";
                        gridColumn5.HeaderText = "Model";
                        tableStyle.GridColumnStyles.Add(gridColumn5);

                        DataGridTextBoxColumn gridColumn6 = new DataGridTextBoxColumn();
                        gridColumn6.Width = 40;
                        gridColumn6.MappingName = "Item";
                        gridColumn6.HeaderText = "Item";
                        tableStyle.GridColumnStyles.Add(gridColumn6);

                        DataGridTextBoxColumn gridColumn7 = new DataGridTextBoxColumn();
                        gridColumn7.Width = 50;

                        if (rdBtnAHU.Checked)
                        {
                            gridColumn7.MappingName = "Section";
                            gridColumn7.HeaderText = "Section";
                        }
                        else
                        {
                            gridColumn7.MappingName = "Qty";
                            gridColumn7.HeaderText = "Qty";
                        }
                        tableStyle.GridColumnStyles.Add(gridColumn7);

                        DataGridTextBoxColumn gridColumn8 = new DataGridTextBoxColumn();
                        gridColumn8.Width = 80;
                        gridColumn8.MappingName = "QtyRcvd";
                        gridColumn8.HeaderText = "Qty Received";
                        tableStyle.GridColumnStyles.Add(gridColumn8);

                        dataGrdFG.TableStyles.Clear();
                        dataGrdFG.TableStyles.Add(tableStyle);
                        dataGrdFG.DataSource = t;
                        Cursor.Current = Cursors.Default;

                        if ((rdBtnAHU.Checked && AHUFCURec.QtyRcvd == AHUFCURec.Section) ||
                            (rdBtnFCU.Checked && AHUFCURec.QtyRcvd == AHUFCURec.Qty))
                        {
                            LoadLocationFG();
                            LoadCountry();
                            btFGMinusQty.Enabled = false;
                            btFGAddQty.Enabled = false;
                            txtFGQty.Text = AHUFCURec.QtyRcvd.ToString();
                            txtFGQty.Enabled = false;
                            rdBtnFGTfrtoCustomer.Checked = AHUFCURec.Country == null ? false : true;
                            rdBtnFGTfrtoWarehse.Checked = AHUFCURec.Location == null ? false : true;
                            cmbBoxFGCountry.SelectedValue = AHUFCURec.Country == null ? -1 : AHUFCURec.Country;
                            cmbBoxFGLocation.SelectedValue = AHUFCURec.Location == null ? -1 : AHUFCURec.Location;
                            cmbBoxFGLocation.Enabled = false;
                            cmbBoxFGCountry.Enabled = false;
                            rdBtnFGTfrtoCustomer.Enabled = false;
                            rdBtnFGTfrtoWarehse.Enabled = false;
                            btnFGShip.Enabled = false;
                            lblFGQty.Enabled = false;
                            lblFGLocation.Enabled = false;
                            lblFGCountry.Enabled = false;
                            MessageBox.Show("All package has been shipped.");
                            isShip = true;
                        }
                        else
                        {
                            btFGMinusQty.Enabled = true;
                            btFGAddQty.Enabled = true;
                            txtFGQty.Enabled = true;
                            rdBtnFGTfrtoWarehse.Checked = true;
                            cmbBoxFGLocation.Enabled = true;
                            rdBtnFGTfrtoWarehse.Enabled = true;
                            rdBtnFGTfrtoCustomer.Checked = false;
                            rdBtnFGTfrtoCustomer.Enabled = true;
                            cmbBoxFGCountry.Enabled = false;
                            btnFGShip.Enabled = true;
                            lblFGQty.Enabled = true;
                            lblFGLocation.Enabled = true;
                            lblFGCountry.Enabled = false;
                            isShip = false;
                        }
                    }
                    else
                    {
                        Cursor.Current = Cursors.Default;
                        ClearFGCache(false);
                        MessageBox.Show("Serial No. does not exist.");
                        isShip = false;
                    }
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Text field validation upon submission
        /// </summary>
        /// <returns></returns>
        private bool ValidateFG()
        {
            if (rdBtnFGTfrtoWarehse.Checked && cmbBoxFGLocation.SelectedIndex == -1)
            {
                MessageBox.Show("Select Location");
                cmbBoxFGLocation.Focus();
                return false;
            }
            else if (rdBtnFGTfrtoCustomer.Checked && cmbBoxFGCountry.SelectedIndex == -1)
            {
                MessageBox.Show("Select Country");
                cmbBoxFGCountry.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Navigate from Main menu to Outbound Delivery page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutbound_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            pnlSelection.Visible = false;
            pnlFG.Visible = true;
            pnlFG.Dock = DockStyle.Fill;
            txtFGSerial.Focus();
            rdBtnAHU.Checked = true;
            rdBtnFCU.Checked = false;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Clear Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFGClear_Click(object sender, EventArgs e)
        {
            ClearFGCache(true);
        }

        /// <summary>
        /// Ship FG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFGShip_Click(object sender, EventArgs e)
        {
            if (ValidateFG())
            {
                if (MessageBox.Show("Confirm to submit?", "Outbound Delivery", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    string sSQL = string.Empty;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        if (AHUFCURec.ID > 0)
                        {
                            if (rdBtnAHU.Checked)
                            {
                                sSQL = " INSERT INTO [dbo].[AHUTransaction]";
                                sSQL += " ([AHUID]";
                            }
                            else if (rdBtnFCU.Checked)
                            {
                                sSQL = " INSERT INTO [dbo].[FCUTransaction]";
                                sSQL += " ([FCUID]";
                            }

                            sSQL += " ,[Quantity]";
                            sSQL += " ,[CountryID]";
                            sSQL += " ,[LocationID]";
                            sSQL += " ,[CreatedOn]";
                            sSQL += " ,[CreatedBy])";
                            sSQL += " VALUES";
                            sSQL += " (" + AHUFCURec.ID;
                            sSQL += " ," + txtFGQty.Text;
                            sSQL += cmbBoxFGCountry.SelectedValue != null ? " ," + cmbBoxFGCountry.SelectedValue + "" : " ,NULL";
                            sSQL += cmbBoxFGLocation.SelectedValue != null ? " ," + cmbBoxFGLocation.SelectedValue + "" : " ,NULL";
                            sSQL += " ,'" + DateTime.Now + "'";
                            sSQL += " ,'" + userID + "')";

                            connection.Open();
                            SqlCommand command = new SqlCommand(sSQL, connection);
                            command.ExecuteReader();
                            connection.Close();
                            Cursor.Current = Cursors.Default;
                            MessageBox.Show(txtFGSerial.Text + " Shipped");
                        }
                        else
                        {
                            Cursor.Current = Cursors.Default;
                            MessageBox.Show("Serial No. does not exist.");
                        }
                    }

                    ClearFGCache(false);
                    txtFGSerial.Focus();
                    txtFGSerial.SelectAll();
                    ResetNewRecord();
                    pnlFGSubmit.Visible = false;
                    pnlFG.Visible = true;
                    pnlFG.Dock = DockStyle.Fill;
                }
            }
        }

        /// <summary>
        /// Return to Main Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFGHome_Click(object sender, EventArgs e)
        {
            pnlFG.Visible = false;
            pnlSelection.Visible = true;
            pnlSelection.Dock = DockStyle.Fill;
            ClearFGCache(true);
        }

        /// <summary>
        /// Search for Serial in DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFGSerial_KeyDown(object sender, KeyEventArgs e)
        {
            if (e != null && e.KeyCode == Keys.Enter)
            {
                GetFG();
            }
        }

        /// <summary>
        /// Is AHU Txn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnAHU_Click(object sender, EventArgs e)
        {
            rdBtnAHU.Checked = true;
            rdBtnFCU.Checked = false;
        }

        /// <summary>
        /// Is FCU Txn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnFCU_Click(object sender, EventArgs e)
        {
            rdBtnAHU.Checked = false;
            rdBtnFCU.Checked = true;
        }

        /// <summary>
        /// Minus quantity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFGMinusQty_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFGQty.Text))
            {
                txtFGQty.Text = "0";
            }

            if (FGQtyBal > 0)
            {
                int decreasedVal = int.Parse(txtFGQty.Text) - 1;

                if (decreasedVal >= 0)
                {
                    txtFGQty.Text = decreasedVal.ToString();
                }
            }
        }

        /// <summary>
        /// Add quantity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFGAddQty_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFGQty.Text))
            {
                txtFGQty.Text = "0";
            }

            if (FGQtyBal > 0)
            {
                int increasedVal = int.Parse(txtFGQty.Text) + 1;

                if (increasedVal <= FGQtyBal)
                {
                    txtFGQty.Text = increasedVal.ToString();
                }
            }
        }

        /// <summary>
        /// Navigate to next page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFGNext_Click(object sender, EventArgs e)
        {
            if (!isShip)
            {
                if (dataGrdFG != null && dataGrdFG.DataSource != null)
                {
                    if (!string.IsNullOrEmpty(txtFGQty.Text) && Convert.ToInt32(txtFGQty.Text) > 0)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        LoadLocationFG();
                        LoadCountry();
                        pnlFG.Visible = false;
                        pnlFGSubmit.Visible = true;
                        pnlFGSubmit.Dock = DockStyle.Fill;
                        txtText.Focus();
                        txtText.SelectAll();
                        rdBtnFGTfrtoCustomer.Checked = false;
                        lblFGCountry.Enabled = false;
                        cmbBoxFGCountry.Enabled = false;
                        rdBtnFGTfrtoWarehse.Checked = true;
                        lblFGLocation.Enabled = true;
                        cmbBoxFGLocation.Enabled = true;
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtFGQty.Text))
                        {
                            MessageBox.Show("Enter Qty");
                            txtFGQty.Focus();
                        }
                        else if (!string.IsNullOrEmpty(txtFGQty.Text) && Convert.ToInt32(txtFGQty.Text) == 0)
                        {
                            MessageBox.Show("Qty must be more than 0");
                            txtFGQty.Focus();
                        }
                        else if (!string.IsNullOrEmpty(txtFGQty.Text) && FGQtyBal > 0)
                        {
                            if (Convert.ToDouble(txtFGQty.Text) > FGQtyBal)
                            {
                                MessageBox.Show("Qty exceeded.\nQuantity available: " + FGQtyBal);
                                txtFGQty.Focus();
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtFGSerial.Text))
                    {
                        MessageBox.Show("Enter Serial No");
                        txtFGSerial.Focus();
                    }
                }
            }
            else
            {
                pnlFG.Visible = false;
                pnlFGSubmit.Visible = true;
                pnlFGSubmit.Dock = DockStyle.Fill;
            }
        }

        /// <summary>
        /// Display field visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnFGTfrtoWarehse_CheckedChanged(object sender, EventArgs e)
        {
            lblFGLocation.Enabled = true;
            cmbBoxFGLocation.Enabled = true;
            lblFGCountry.Enabled = false;
            cmbBoxFGCountry.Enabled = false;
        }

        /// <summary>
        /// Display field visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnFGTfrtoCustomer_CheckedChanged(object sender, EventArgs e)
        {
            lblFGLocation.Enabled = false;
            cmbBoxFGLocation.Enabled = false;
            lblFGCountry.Enabled = true;
            cmbBoxFGCountry.Enabled = true;
        }

        /// <summary>
        /// Return to FG Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFGBack_Click(object sender, EventArgs e)
        {
            pnlFGSubmit.Visible = false;
            pnlFG.Visible = true;
            pnlFG.Dock = DockStyle.Fill;
        }

        #endregion

        #endregion
    }
}