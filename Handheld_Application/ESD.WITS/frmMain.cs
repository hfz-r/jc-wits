using System;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
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
        #region Variable

        private Color placeHolderDefaultColor = Color.Gray;
        private Color defaultColor = Color.Black;
        private const string placeHolder = "Scan...";       
        private string userID = string.Empty;
        private string gStrProgPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\";
        private string gStrSQLServer = string.Empty;
        private string gStrDBName = string.Empty;
        private string gStrSQLUser = string.Empty;
        private string gStrSQLPwd = string.Empty;
        private string connectionString = "Data Source=192.168.56.1,5050;Initial Catalog=INVENTORY;Trusted_Connection=Yes;User ID=sa;Password=p@ssw0rd;Persist Security Info=False;Integrated Security=False;";        
        private static string databasePath = @"\Application\";
        private static string databaseName = "InventoryDB.sdf";
        private string localConnectionString = "Data Source=" + databasePath + databaseName;
        private int GRID = 0;
        private bool isPartialTxn = false;

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
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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
            SqlCeConnection localConnection = null;
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
                //string pass = GetMD5Hash(password).ToUpper();
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
                if (localConnection != null)
                {
                    localConnection.Close();
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
            isPartialTxn = false;
            string sSQL = string.Empty;
            byte? isTxnCompleted = null;
            string qtyRcvd = string.Empty;

            sSQL = "SELECT GR.[ID]";
            sSQL += " ,GR.[MaterialShortText]";
            sSQL += " ,ISNULL(GR.[Ok],0)";
            sSQL += " ,GR.[Quantity]";
            sSQL += " ,GR.[Eun]";
            sSQL += " ,SUM(GRT.Quantity) AS QtyOrdered ";
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
                        qtyRcvd = reader[5].ToString(); //var
                        txtGRQtyRcvd.Text = qtyRcvd;
                        txtGRRcvdEun.Text = txtGROrderedEun.Text;

                        if (isTxnCompleted != null && Convert.ToBoolean(isTxnCompleted)) //complete
                        {
                            btnGRSubmit.Enabled = false;
                            labelGRQty.Visible = false;
                            btnMinusIn.Visible = false;
                            txtGRQty.Visible = false;
                            btnAddIn.Visible = false;
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
                            if (!string.IsNullOrEmpty(qtyRcvd) && Convert.ToInt32(qtyRcvd) < Convert.ToInt32(txtGRQtyOrdered.Text))
                            {
                                isPartialTxn = true;
                                btnGRSubmit.Enabled = true;
                                labelGRQty.Visible = true;
                                btnMinusIn.Visible = true;
                                txtGRQty.Visible = true;
                                btnAddIn.Visible = true;
                                txtGREun.Visible = true;
                                labelGRReason.Visible = true;
                                cmbBoxGRReason.Visible = true;
                                labelGRQtyRcvd.Visible = true;
                                txtGRQtyRcvd.Visible = true;
                                txtGRRcvdEun.Visible = true;
                                labelGRQty.Location = new Point(7, 178);
                                btnMinusIn.Location = new Point(104, 175);
                                txtGRQty.Location = new Point(126, 175);
                                btnAddIn.Location = new Point(170, 175);
                                txtGREun.Location = new Point(197, 175);
                                labelGRReason.Location = new Point(8, 207);
                                cmbBoxGRReason.Location = new Point(103, 204);
                            }
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
            btnGRSubmit.Enabled = true;
            labelGRQty.Visible = true;
            labelGRQty.Location = new Point(7, 149);
            btnMinusIn.Visible = true;
            btnMinusIn.Location = new Point(104, 149);
            txtGRQty.Visible = true;
            txtGRQty.Location = new Point(126, 149);
            btnAddIn.Visible = true;
            btnAddIn.Location = new Point(170, 149);
            txtGREun.Visible = true;
            txtGREun.Location = new Point(197, 149);
            labelGRReason.Visible = true;
            labelGRReason.Location = new Point(8, 178);
            cmbBoxGRReason.Visible = true;
            cmbBoxGRReason.Location = new Point(103, 175);
            labelGRQtyRcvd.Visible = false;
            txtGRQtyRcvd.Visible = false;
            txtGRRcvdEun.Visible = false;
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
                if (Convert.ToInt32(txtGRQty.Text) < Convert.ToInt32(txtGRQtyOrdered.Text))
                {
                    cmbBoxGRReason.Visible = true;
                    labelGRReason.Visible = true;
                }
                else if (Convert.ToInt32(txtGRQty.Text) == Convert.ToInt32(txtGRQtyOrdered.Text))
                {
                    cmbBoxGRReason.Visible = false;
                    labelGRReason.Visible = false;
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

            int decreasedVal = int.Parse(txtGRQty.Text) - 1;

            if (decreasedVal >= 0)
            {
                txtGRQty.Text = decreasedVal.ToString();
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

                if (!string.IsNullOrEmpty(txtGRQtyOrdered.Text) && Convert.ToInt32(txtGRQtyOrdered.Text) > 0)
                {
                    string ReasonID = Convert.ToInt32(txtGRQty.Text) == Convert.ToInt32(txtGRQtyOrdered.Text) ? null : cmbBoxGRReason.Text;
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
                        sSQL += " SET PostingDate = '" + DateTime.Now + "'";
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

        private void btnGIHome_Click(object sender, EventArgs e)
        {
            txtGISAPNo.Text = string.Empty;
            pnlGdIssue.Visible = false;
            pnlSelection.Visible = true;
            pnlSelection.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Set placeholder text on textbox
        /// </summary>
        private void SetGIPlaceholder()
        {
            this.txtGISAPNo.ForeColor = placeHolderDefaultColor;
            this.txtGISAPNo.Text = placeHolder;
        }

        #endregion

        #endregion
    }
}