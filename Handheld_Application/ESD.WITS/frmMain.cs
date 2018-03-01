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
        private string connectionString = "Data Source=192.168.56.1,5050;Initial Catalog=INVENTORY;Trusted_Connection=Yes;User ID=sa;Password=p@ssw0rd;Persist Security Info=False;Integrated Security=False;";        
        private int GRID = 0;
        private bool isPartialTxn = false;
        private List<GI> GIList = new List<GI>();
        private List<Location> LocationList = new List<Location>();
        private string SelectedSAP = string.Empty;
        private string SelectedQty = string.Empty;
        private TransferType transferType = new TransferType();
        private string SLoc = string.Empty;
        private string ENMatShortText = string.Empty;
        #endregion

        #region Initialization

        public frmMain()
        {
            InitializeComponent();
            GetXMLSetting();
            pnlLogin.Visible = true;
            pnlLogin.Dock = DockStyle.Fill;
            lblVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
                string pass = HashConverter.CalculateHash(password, username);

                sSQL = "SELECT ID ";
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
                        }
                    }
                }

                if (isPass)
                {
                    pnlLogin.Visible = false;
                    pnlSelection.Visible = true;
                    pnlSelection.Dock = DockStyle.Fill;
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
            GetReason();
            pnlSelection.Visible = false;
            pnlGdReceipt.Visible = true;
            pnlGdReceipt.Dock = DockStyle.Fill;
            SetGRPlaceholder();
            txtGRSAPNo.Focus();
            txtGRSAPNo.SelectAll();
            txtGRQty.Text = "0";
            ResetNewRecord();//if new record
        }

        /// <summary>
        /// Dashboard selection to Goods Issue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGI_Click(object sender, EventArgs e)
        {
            GetReason();
            pnlSelection.Visible = false;
            pnlGdIssue.Visible = true;
            pnlGdIssue.Dock = DockStyle.Fill;
            SetGIPlaceholder();
            txtGISAPNo.Focus();
            txtGISAPNo.SelectAll();
            btnGINext.Enabled = false;
            btnGIDelete.Enabled = false;
            LoadLocation();
            rdBtnGITfrtoProd.Checked = true;
            rdBtnGITfrPosting.Checked = false;
        }

        #endregion

        #region Goods Receipt Page

        /// <summary>
        /// Validate respective field before submitting to DB
        /// </summary>
        /// <returns></returns>
        private bool ValidationGR()
        {
            if (string.IsNullOrEmpty(txtGRSAPNo.Text) || txtGRSAPNo.Text == placeHolder)
            {
                MessageBox.Show("SAP No. cannot be empty.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                txtGRSAPNo.Focus();
                txtGRSAPNo.SelectAll();
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
                    connection.Close();
                }
                
                connection.Close();
            }
        }

        /// <summary>
        /// Get details by SAP No
        /// </summary>
        private void GetGoodsReceipt()
        {
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

            sSQL = "SELECT GR.[ID]";
            sSQL += " ,GR.[MaterialShortText]";
            sSQL += " ,ISNULL(GR.[Ok],0)";
            sSQL += " ,GR.[Quantity]";
            sSQL += " ,GR.[Eun]";
            sSQL += " ,ISNULL(SUM(GRT.Quantity),0) AS QtyOrdered ";
            sSQL += "FROM [INVENTORY].[dbo].[GoodsReceive] GR ";
            sSQL += "LEFT OUTER JOIN [dbo].[GRTransaction] GRT  ";
            sSQL += " ON GR.ID = GRT.GRID ";
            sSQL += "WHERE GR.[Material] = '" + txtGRSAPNo.Text + "' ";
            sSQL += "GROUP BY GR.[ID], GR.[MaterialShortText], GR.[Ok], GR.[Quantity], GR.[Eun] ";

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
                        txtGRQtyOrdered.Text = reader[3].ToString();
                        txtGREun.Text = reader[4].ToString();
                        txtGROrderedEun.Text = reader[4].ToString();
                        txtGRRcvdEun.Text = txtGROrderedEun.Text;
                        qtyRcvd = reader[5].ToString() == "0.00" ? "0" : reader[5].ToString();
                        txtGRQtyRcvd.Text = txtGRRcvdEun.Text != "KG" ? 
                            (Convert.ToInt64(Math.Floor(Convert.ToDouble(qtyRcvd)))).ToString() : qtyRcvd.ToString();
                        txtGRMSDesc.BackColor = Color.Black;
                        txtGRQtyOrdered.BackColor = Color.Black;
                        txtGROrderedEun.BackColor = Color.Black;
                        txtGRQtyRcvd.BackColor = Color.Black;
                        txtGRRcvdEun.BackColor = Color.Black;
                        txtGREun.BackColor = Color.Black;

                        if (isTxnCompleted != null && Convert.ToBoolean(isTxnCompleted)) //complete
                        {
                            btnGRSubmit.Enabled = false;
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
                                btnGRSubmit.Enabled = true;
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
                                btnGRSubmit.Enabled = true;
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
                    }
                    else
                    {
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
            btnGRSubmit.Enabled = true;
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
            cmbBoxGRReason.SelectedValue = -1;
            txtGRQtyRcvd.Text = string.Empty;
            txtGRRcvdEun.Text = string.Empty;
            txtGRSAPNo.Text = clearSAPNo ? string.Empty : txtGRSAPNo.Text;
            isPartialTxn = false;
            GRID = 0;
            txtGRMSDesc.Text = string.Empty;
            txtGRQtyOrdered.Text = string.Empty;
            txtGROrderedEun.Text = string.Empty;
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
            if (ValidationGR())
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
                        sSQL += " [PostingDate] = '" + dtPickerGRPostingDate.Value + "'";
                        sSQL += " WHERE ID = @GRID";

                        sSQL += " UPDATE [dbo].[GoodsReceive]";
                        sSQL += " SET Ok = 1";
                        sSQL += " WHERE Quantity = (";
                        sSQL += " SELECT SUM([Quantity]) AS TotalQty";
                        sSQL += " FROM [INVENTORY].[dbo].[GRTransaction]";
                        sSQL += " WHERE GRID = @GRID)";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(sSQL, connection);
                            command.ExecuteReader();
                            MessageBox.Show("GR Posted");
                            connection.Close();
                        }
                        ResetNewRecord();
                        ClearCache(true);
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
            string sSQL = string.Empty;
            txtGIQtyAvbl.Text = string.Empty;
            txtGIQtyAvblEun.Text = string.Empty;
            txtGIQtyEun.Text = string.Empty;

            sSQL = " SELECT GR.[ID], GR.[MaterialShortText], GR.[Quantity], GR.[Eun], GR.[StorageLoc],";
            sSQL += " ISNULL(SUM(GRT.Quantity),0) AS QtyAvailable, ISNULL(SUM(GRT.Quantity),0) -";
            sSQL += " (SELECT ISNULL(SUM(GIT.Quantity),0) AS QtyAvailableGI";
            sSQL += " FROM [INVENTORY].[dbo].[GoodsReceive] GR ";
            sSQL += " INNER JOIN [dbo].[GITransaction] GIT   ON GR.ID = GIT.GRID ";
            sSQL += " WHERE GR.[Material] = '" + txtGISAPNo.Text + "') AS QtyRemaining, GR.[ENMaterialShortText]";
            sSQL += " FROM [INVENTORY].[dbo].[GoodsReceive] GR ";
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
            }

            if (!string.IsNullOrEmpty(txtGIQtyAvbl.Text) && Convert.ToDouble(txtGIQtyAvbl.Text) == 0)
            {
                MessageBox.Show("No Qty available");
            }
        }

        /// <summary>
        /// validate submit page
        /// </summary>
        /// <returns></returns>
        private bool ValidateGISubmit()
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
        private void LoadLocation()
        {
            string sSQL = string.Empty;
            sSQL = "SELECT [ID] ";
            sSQL += " ,[Location] ";
            sSQL += "FROM [INVENTORY].[dbo].[Location] ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sSQL, connection);

                using (SqlDataAdapter sda = new SqlDataAdapter(command))
                {
                    DataTable dtTo = new DataTable();
                    sda.Fill(dtTo);

                    cmbBoxGILocTo.DataSource = dtTo;
                    cmbBoxGILocTo.DisplayMember = "Location";
                    cmbBoxGILocTo.ValueMember = "ID";
                    cmbBoxGILocTo.SelectedIndex = -1;

                    DataTable dtFrom = new DataTable();
                    sda.Fill(dtFrom);
                    cmbBoxGILocFrom.DataSource = dtFrom;
                    cmbBoxGILocFrom.DisplayMember = "Location";
                    cmbBoxGILocFrom.ValueMember = "ID";
                    cmbBoxGILocFrom.SelectedIndex = -1;

                    /////Get ID
                    //int? defaultID = null;
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    if ((dr["ID"] != DBNull.Value) && ((string)dr["Location"] == SLoc))
                    //    {
                    //        defaultID = Convert.ToInt32(dr["ID"].ToString());
                    //    }
                    //}
                    //if (defaultID != null)
                    //{
                    //    SLocID = defaultID;
                    //}

                    connection.Close();
                }
            }
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
            if (ValidateGISubmit())
            {
                if (GIList != null && GIList.Count > 0)
                {
                    if (MessageBox.Show("Confirm to submit?", "Goods Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
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
                                sSQL += " ,[LocationTo]";
                                sSQL += " ,[LocationFrom])";
                                sSQL += " VALUES";
                                sSQL += " ('" + txtText.Text + "'";
                                sSQL += " ,'" + item.Qty + "'";
                                sSQL += " ,'" + DateTime.Now + "'";
                                sSQL += " ,'" + userID + "'";
                                sSQL += " ,'" + item.ID + "'";
                                sSQL += " ,'" + transferType + "'";
                                sSQL += !string.IsNullOrEmpty(txtGIProdNo.Text) ? " ,'" + txtGIProdNo.Text + "'" : " ,NULL";
                                sSQL += !string.IsNullOrEmpty(cmbBoxGILocTo.Text) ? " ,'" + cmbBoxGILocTo.Text + "'" : " ,NULL";
                                sSQL += !string.IsNullOrEmpty(cmbBoxGILocFrom.Text) ? " ,'" + cmbBoxGILocFrom.Text + "')" : " ,NULL)";
                            }
                            connection.Open();
                            SqlCommand command = new SqlCommand(sSQL, connection);
                            command.ExecuteReader();
                            connection.Close();
                        }

                        MessageBox.Show(txtText.Text + " Submitted");
                    }

                    ClearGICache();
                    pnlGdIssueSubmit.Visible = false;
                    pnlGdIssue.Visible = true;
                    pnlGdIssue.Dock = DockStyle.Fill;
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
                pnlGdIssue.Visible = false;
                pnlGdIssueSubmit.Visible = true;
                pnlGdIssueSubmit.Dock = DockStyle.Fill;
            }
        }

        #endregion

        private void btnOutbound_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In progress.");
        }

        #endregion
    }
}