using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Data.SqlClient;
using ComponentFactory.Krypton.Toolkit;

namespace CoreApp
{
    /// <summary>
    /// The Utility Class. (By : ABDUL MATEEN KHAN (abdulmateen50@gmail.com))
    /// </summary>
    public class clsUtility
    {
        #region Message Constantas
        /// <summary>
        /// Data Inserted Successfully.
        /// </summary>
        public const string MsgDataInserted = "Data Inserted Successfully.";

        /// <summary>
        /// Data Saved Successfully.
        /// </summary>
        public const string MsgDataSaved = "Data Saved Successfully.";

        /// <summary>
        /// Data not Saved.
        /// </summary>        
        public const string MsgDatanotSaved = "Faild to save the data.";

        /// <summary>
        /// Data Updated Successfully.
        /// </summary>
        public const string MsgDataUpdated = "Data Updated Successfully.";

        /// <summary>
        /// Data not Updated.
        /// </summary>
        public const string MsgDatanotUpdated = "Faild to update the data.";
        /// <summary>
        /// Data Deleted Successfully.
        /// </summary>
        public const string MsgDataDeleted = "Data Deleted Successfully.";

        /// <summary>
        /// Data not Deleted Successfully.
        /// </summary>
        public const string MsgDatanotDeleted = "Faild to delete the data.";

        /// <summary>
        /// Record Inserted Successfully.
        /// </summary>
        public const string MsgRecordInserted = "Record Inserted Successfully.";

        /// <summary>
        /// Record Saved Successfully.
        /// </summary>
        public const string MsgRecordSaved = "Record Saved Successfully.";

        /// <summary>
        /// Record Updated Successfully.
        /// </summary>
        public const string MsgRecordUpdated = "Record Updated Successfully.";

        /// <summary>
        /// Record Deleted Successfully.
        /// </summary>
        public const string MsgRecordDeleted = "Record Deleted Successfully.";

        /// <summary>
        /// Action Delete. Message constant when user going to delete some message.
        /// </summary>
        public const string MsgActionDelete = "Are you sure, you want to delete this record?";

        /// <summary>
        /// Action Cancel. Message constant when user going to cancel some action.
        /// </summary>
        public const string MsgActionCancel = "Are you sure, you want to cancel this action?";

        /// <summary>
        /// Action Close. Message constant when user going to close the window.
        /// </summary>
        public const string MsgActionClose = "Are you sure, you want to close this window?";

        #endregion

        internal static Dictionary<string, Control> ObjPopupControl = new Dictionary<string, Control>();
        clsCommon ObjCommon = new clsCommon();
        //clsConnection_DAL ObjDAL = new clsConnection_DAL(true); stack overflow error has occured if uncomment the line

        /// <summary>
        /// set whether loggon user is Admin or not.
        /// </summary>
        public static bool IsAdmin = false;

        /// <summary>
        /// set the loggon user's ID.
        /// </summary>
        public static int LoginID = 0;

        /// <summary>
        /// set the DataBase Name to pass the dynamic name in required forms.
        /// </summary>
        public static String DBName = String.Empty;

        static bool isFormClosing = false;
        char[] c1 = new char[2];

        /// <summary>
        /// set the title of the project.
        /// </summary>
        public static string strProjectTitle;
        /// <summary>
        /// Get the dgvPopup.
        /// <b>Note :use this variable only after ShowDataPopup() method. </b>
        /// </summary>
        public static DataGridView dgvPoup;
        /// <summary>
        /// set the default width of the popup
        /// </summary>
        public static bool SetDataPoupDefaultWidth { get; set; }
        /// <summary>
        /// Initialize the new instance of clsUtility class. 
        /// </summary>
        /// 

        private DataGridView dgvDataPopup;
        public clsUtility()
        {
            c1[0] = '[';
            c1[1] = ']';
        }
        /// <summary>
        /// Get the image in byte array format.
        /// </summary>
        /// <param name="Pic">Pass the image.</param>
        /// <returns>byte array</returns>
        public byte[] GetImageBytes(Image Pic)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                Pic.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, " Method Name : GetImageBytes(Image Pic)");
            }
            return null;
        }

        /// <summary>
        /// Check if the table has any row or it is null.
        /// This method Returns True if table is not null and has more than zero rows else returns false.
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <returns>Returns True if table is not null and has more than zero rows else returns false.</returns>
        public bool ValidateTable(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the dataset has any table or it is null.
        /// This method Returns True if dataset is not null and has more than zero table count else returns false.
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>Returns True if dataset is not null and has more than zero talbe count else returns false.</returns>
        public bool ValidateDataSet(DataSet ds)
        {
            if (ds != null && ds.Tables.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Encrypt a string using dual encryption method. Return a encrypted  Text
        /// </summary>
        /// <param name="toEncrypt">string to be encrypted</param>
        /// <param name="useHashing">use hashing? send to for extra security</param>
        /// <returns></returns>
        public string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "abdulmateen1989";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// If you had encrypted a string by passing true to UseHashing parameter then you must pass true 
        /// while decrypting the string.
        /// </summary>
        /// <param name="cipherString">encrypted string</param>
        /// <param name="useHashing">Did you use hashing to encrypt this data? pass true if yes</param>
        /// <returns></returns>
        public string Decrypt(string cipherString, bool useHashing)
        {
            string strDecryptString = "";
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                //Get your key from config file to open the lock!
                string key = "abdulmateen1989";

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                strDecryptString = UTF8Encoding.UTF8.GetString(resultArray);
                return strDecryptString;
            }
            catch (FormatException)
            {
                clsUtility.ShowErrorMessage("The encrypted text is not in correct format.", "CoreApp");
            }
            return strDecryptString;
        }

        /// <summary>
        /// Show a window form for making connection string. 
        /// You can also Encrypt or Decrypt connection string.
        /// </summary>

        public void ShowConnectionBuilder()
        {
            ConStringBuilder ObjConBuilder = new ConStringBuilder();
            ObjConBuilder.Show();
        }

        /// <summary>
        /// Convert String to Byte Array.
        /// </summary>
        /// <param name="str">Pass string to convert into bytes.</param>
        /// <returns>Returns byte array.</returns>
        public byte[] GetBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        /// <summary>
        /// Highlight the textbox or RichTextBox when control gets the focus.
        /// </summary>
        /// <param name="control">Pass the control. (TextBox or RichTextBox)</param>
        /// <param name="color">Set the Highlight color</param>
        public void SetTextHighlightColor(object control, Color color)
        {
            if (control.GetType() == typeof(TextBox))
            {
                if (((TextBox)control).ReadOnly)
                {
                    return;
                }
                ((TextBox)control).BackColor = color;
            }
            else if (control.GetType() == typeof(RichTextBox))
            {
                if (((RichTextBox)control).ReadOnly)
                {
                    return;
                }
                ((RichTextBox)control).BackColor = color;
            }
            else
            {
                throw new Exception("Invalid Control Type. Control must be textbox or rich textbox.");
            }
        }

        /// <summary>
        /// Highlight the textbox or RichTextBox when control gets the focus.<br></br>
        /// the default color is Info
        /// </summary>
        /// <param name="control">Pass the control. (TextBox or RichTextBox)</param>

        public void SetTextHighlightColor(object control)
        {
            if (control.GetType() == typeof(TextBox))
            {
                if (((TextBox)control).ReadOnly)
                {
                    return;
                }
                ((TextBox)control).BackColor = Color.FromKnownColor(KnownColor.Info);
            }
            else if (control.GetType() == typeof(RichTextBox))
            {
                if (((RichTextBox)control).ReadOnly)
                {
                    return;
                }
                ((RichTextBox)control).BackColor = Color.FromKnownColor(KnownColor.Info);
            }
            else
            {
                throw new Exception("Invalid Control Type. Control must be textbox or rich textbox.");
            }
        }

        /// <summary>
        /// Scroll to the end of the rich text box.
        /// </summary>
        /// <param name="Rtb">Pass the rich text box object.</param>
        public void ScrollToLast(RichTextBox Rtb)
        {
            Rtb.Select(Rtb.Text.Length - 1, 0);
            Rtb.ScrollToCaret();
        }

        /// <summary>
        /// Get only the time Part in 12hrs Format. (AM/PM) (SQL)
        /// </summary>
        /// <param name="ColName"></param>
        /// <returns>SQL string </returns>
        public string GetTime(string ColName)
        {
            ColName = ColName.Trim(c1);
            return " LTRIM(RIGHT(CONVERT(VARCHAR(20),[" + ColName + "], 100), 7)) AS [" + ColName + "] ";
        }

        /// <summary>
        /// Set the common property to the GridView.
        /// </summary>
        /// <param name="dgv">Pass the Grid view.</param>
        /// <param name="ColMode">Auto size Column mode of the grid view. </param>
        public void SetDataGridProperty(DataGridView dgv, DataGridViewAutoSizeColumnsMode ColMode)
        {
            dgv.DefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Regular);
            dgv.DefaultCellStyle.SelectionBackColor = Color.Navy;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;

            dgv.BackgroundColor = Color.White;

            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = ColMode;
        }

        /// <summary>
        /// Set the common property to the GridView.
        /// </summary>
        /// <param name="dgv">Pass the Grid view.</param>
        /// <param name="ColMode">Auto size Column mode of the grid view. </param>
        /// <param name="BackColor">Back color of the cell </param>
        public void SetDataGridProperty(DataGridView dgv, DataGridViewAutoSizeColumnsMode ColMode, Color BackColor)
        {
            dgv.DefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Regular);
            dgv.DefaultCellStyle.SelectionBackColor = Color.Navy;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;

            dgv.BackgroundColor = BackColor;

            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = ColMode;

        }

        /// <summary>
        /// Set the common property to the GridView.
        /// </summary>
        /// <param name="dgv">Pass the Grid view.</param>

        public void SetDataGridProperty(DataGridView dgv)
        {
            dgv.DefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.Highlight);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = true;
            dgv.AllowUserToResizeRows = false;

            dgv.BackgroundColor = Color.White;

            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        ///  Call this method in  dataGridView1_DataBindingComplete
        ///  Adding Serial Number to the GridView row header.
        /// </summary>
        /// <param name="dgv">Pass Your GridView as a parameter</param>
        public void SetRowNumber(DataGridView dgv)
        {
            //The following code example uses the HeaderCell property to label row headers.
            // Initially Set the Row number as 1.
            int rowNumber = 1;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                // Optional : (If Condition is Optional)
                if (row.IsNewRow)
                {
                    continue;
                }
                // Set the Row Number.
                row.HeaderCell.Value = rowNumber.ToString();

                // Increment the Row Number by 1
                rowNumber = rowNumber + 1;
            }
            dgv.RowHeadersVisible = true;

            // Setting the border style of the Row Header.
            dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            // setting the Font of the Row Header.
            dgv.RowHeadersDefaultCellStyle.Font = new System.Drawing.Font("Times", 11.25f, FontStyle.Regular);

            // Set the width of the Row Header.
            dgv.RowHeadersWidth = 60;

            // Set the Text on the  [upper left Corner cell] of the Grid view
            dgv.TopLeftHeaderCell.Value = "Sr.No.";
            dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgv.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        /// <summary>
        /// Count the total checked items in a grid view.
        /// Your grid view should contain a checkbox column.
        /// </summary>
        /// <param name="dgv">Pass the grid view object.</param>
        /// <param name="strCheckBoxColumn">Pass the name of the CheckBox column</param>
        public int CountCheckedItemsGrid(DataGridView dgv, string strCheckBoxColumn)
        {
            int a = dgv.Rows.Cast<DataGridViewRow>().
                Where(r => Convert.ToBoolean(r.Cells[strCheckBoxColumn].Value) == true).Count();
            return a;
        }

        /// <summary>
        /// Percentage Calculation :  Calculate the Percentage by passing Amount and Total amount.
        /// </summary>
        /// <param name="amount">Pass the amount.</param>
        /// <param name="TotalAmount">Pass the total amount.</param>
        /// <returns></returns>
        public double CalPercent(double amount, double TotalAmount)
        {
            double Percent = (amount * 100) / TotalAmount;
            return Percent;
        }

        /// <summary>
        /// Percentage Calculation :  Calculate the percent amount by passing Percent and Total amount.
        /// </summary>
        /// <param name="Percent">Pass the percentage(%)</param>
        /// <param name="TotalAmount">Pass the total amount.</param>
        /// <returns>Returns the percent amount.</returns>
        public double CalPercentAmount(double Percent, double TotalAmount)
        {
            double PercentAmount = (Percent * TotalAmount) / 100;
            return PercentAmount;
        }

        /// <summary>
        /// Percentage Calculation :  Get the total amount by passing Percent and Amount.
        /// </summary>
        /// <param name="Percent">Pass the percent amount (%)</param>
        /// <param name="Amount">Pass the amount.</param>
        /// <returns>Returns the total amount.</returns>
        public double CalTotalAmount(double Percent, double Amount)
        {
            double TotalAmount = Percent / (Amount * 100);
            return TotalAmount;
        }

        /// <summary>
        /// Show Information message Dialog box with default icon and button.
        /// </summary>
        /// <param name="strMessage">Message text.</param>
        /// <param name="Title">Title of the message.</param>
        public static void ShowInfoMessage(string strMessage, string Title)
        {
            MessageBox.Show(strMessage, Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Show Information message Dialog box with default icon and button.
        /// </summary>
        /// <param name="strMessage">Message text.</param>
        public static void ShowInfoMessage(string strMessage)
        {
            MessageBox.Show(strMessage, strProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Show Error Message Dialog box with Error icon and button.
        /// </summary>
        /// <param name="strMessage">Message text</param>
        /// <param name="Title">Title of the message.</param>
        public static void ShowErrorMessage(string strMessage, string Title)
        {
            MessageBox.Show(strMessage, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Show Error Message Dialog box with Error icon and button.
        /// </summary>
        /// <param name="strMessage">Message text</param>
        public static void ShowErrorMessage(string strMessage)
        {
            MessageBox.Show(strMessage, strProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Show a Question  Dialog  Message with Question mark and Yes No button.
        /// </summary>
        /// <param name="strMessage">Message Text to be passed.</param>
        /// <param name="Title">Title of the message.</param>
        /// <returns>If user press Yes it will return True else False.</returns>
        public static bool ShowQuestionMessage(string strMessage, string Title)
        {
            DialogResult d = MessageBox.Show(strMessage, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Show a Question  Dialog  Message with Question mark and Yes No button.
        /// </summary>
        /// <param name="strMessage">Message Text to be passed.</param>
        /// <returns>If user press Yes it will return True else False.</returns>
        public static bool ShowQuestionMessage(string strMessage)
        {
            DialogResult d = MessageBox.Show(strMessage, strProjectTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check and validates  the controls text property.
        /// (Pass the control name Only. For Example : txtEmployeeName)
        /// </summary>
        /// <param name="c">Pass the control. (textBox, richtext box etc)</param>
        /// <returns>Returns True if Text is empty else returns false.</returns>
        public bool IsControlTextEmpty(Control c)
        {
            if (c.GetType() == typeof(TextBox))
            {
                if (((TextBox)c).Text.Trim().Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (c.GetType() == typeof(RichTextBox))
            {
                if (((RichTextBox)c).Text.Trim().Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (c.GetType() == typeof(ComboBox))
            {
                if (((ComboBox)c).Text.Trim().Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the string is numeric or not.
        /// </summary>
        /// <param name="str">Pass the string.</param>
        /// <returns>Returns true if string or text is numeric else returns false.</returns>
        public bool IsNumeric(string str)
        {
            try
            {
                Convert.ToDouble(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// checking whether entered text is numeric or not
        /// </summary>
        /// <param name="e">Pass the KeyPressEvent.</param>
        /// <returns>Returns true if invalid name or text is numeric else returns false.</returns> 
        public bool IsNumeric(KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar >= 48 && e.KeyChar <= 57 || e.KeyChar == 8)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Check if the string is valid name or not at the time of pressing.
        /// </summary>
        /// <param name="c">Pass the Textbox control.</param>
        /// <param name="e">Pass the KeyPressEvent.</param>
        /// <returns>Returns true if invalid name or text is numeric else returns false.</returns>
        public bool IsDecimal(Control c, KeyPressEventArgs e)
        {
            bool b = false;
            try
            {
                TextBox txt = (TextBox)c;
                if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
                {
                    b = true;
                }
                if (e.KeyChar == 46)
                {
                    if (txt.Text.IndexOf(e.KeyChar) != -1 || txt.SelectionStart == 0)
                        b = true;
                }

                //if (e.KeyChar >= 48 && e.KeyChar <= 57 || e.KeyChar == 8 || e.KeyChar == 46)
                //{
                //    if (txt.SelectionStart == 0 && e.KeyChar != 46 && !txt.Text.Contains("."))
                //        b = false;
                //    else
                //        b = true;

                //    if (txt.SelectionStart >= 1 && e.KeyChar == 46 && !txt.Text.Contains("."))
                //        b = false;

                //    else if (txt.SelectionStart >= 1 && e.KeyChar != 46)
                //        b = false;
                //}
                //else
                //{
                //    b = true;
                //}
                return b;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Check if the string is valid name or not at the time of pressing.
        /// </summary>
        /// <param name="e">Pass the KeyPressEvent.</param>
        /// <returns>Returns true if invalid name or text is numeric else returns false.</returns>
        public bool IsString(KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar >= 65 && e.KeyChar <= 90 || e.KeyChar >= 97 && e.KeyChar <= 122 || e.KeyChar == 8 || e.KeyChar == 32)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Check if the string is valid name or not at the time of pressing.
        /// </summary>
        /// <param name="e">Pass the KeyPressEvent.</param>
        /// <returns>Returns true if invalid name or text is Alpha numeric else returns false.</returns>
        public bool IsAlphaNumeric(KeyPressEventArgs e)
        {
            try
            {
                if (IsString(e) == false || IsNumeric(e) == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Get the Image/Picture by passing the byte array of the image.
        /// </summary>
        /// <param name="b">pass byte array</param>
        /// <returns>Image</returns>
        public Image GetImage(byte[] b)
        {
            MemoryStream ms = new MemoryStream(b);
            Image img = Image.FromStream(ms);
            return img;
        }

        /// <summary>
        /// Run the specified application or program at system start-up.
        /// </summary>
        /// <param name="strVal">Application Name</param>
        /// <param name="strPath">Application executable path.</param>
        public void RunAtStartup(string strVal, string strPath)
        {
            try
            {
                RegistryKey regKey;
                regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\MICROSOFT\\Windows\\CurrentVersion\\Run", true);
                try
                {
                    regKey.DeleteValue(strVal, true);
                }
                catch { }
                regKey.SetValue(strVal, strPath);
                regKey.Close();
            }
            catch (Exception ex)
            {
                clsUtility.ShowErrorMessage(ex.ToString(), "RunAtStartup");
            }
        }

        /// <summary>
        /// Validates the E-mail address.
        /// </summary>
        /// <param name="email">Pass your email address</param>
        /// <returns>Returns true if email address is valid email address</returns>
        public bool ValidateEmail(string email)
        {
            string EId = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" +
               @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" +
               @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";

            //System.Text.RegularExpressions.Regex emailRegex = new System.Text.RegularExpressions.Regex("^(?<user>[^@]+)@(?<host>.+)$");
            //System.Text.RegularExpressions.Match emailMatch = emailRegex.Match(email);

            return System.Text.RegularExpressions.Regex.Match(email, EId).Success;
        }

        private Button ButtonNew, ButtonSave, ButtonEdit, ButtonUpdate, ButtonDelete, ButtonCancel;
        private KryptonButton KrButtonNew, KrButtonSave, KrButtonEdit, KrButtonUpdate, KrButtonDelete, KrButtonCancel;
        private ToolStripButton ToolStripNew, ToolStripSave, ToolStripEdit, ToolStripUpdate, ToolStripDelete, ToolStripCancel;

        /// <summary>
        /// Register the command button for enable/Disable status. This method will register the Button Control
        /// </summary>
        /// <param name="New">New Button</param>
        /// <param name="Save">Save Button</param>
        /// <param name="Edit">Edit Button</param>
        /// <param name="Update"> Update Button</param>
        /// <param name="Delete"> Delete Button</param>
        /// <param name="Cancel"> Cancel Button</param>
        public void RegisterCommandButtons(Button New, Button Save, Button Edit, Button Update, Button Delete, Button Cancel)
        {
            this.ButtonNew = New;
            this.ButtonSave = Save;
            this.ButtonEdit = Edit;
            this.ButtonUpdate = Update;
            this.ButtonDelete = Delete;
            this.ButtonCancel = Cancel;
        }

        /// <summary>
        /// Register the command KryptonButton for enable/Disable status. This method will register the Button Control
        /// </summary>
        /// <param name="New">New Button</param>
        /// <param name="Save">Save Button</param>
        /// <param name="Edit">Edit Button</param>
        /// <param name="Update"> Update Button</param>
        /// <param name="Delete"> Delete Button</param>
        /// <param name="Cancel"> Cancel Button</param>
        public void RegisterCommandButtons(KryptonButton New, KryptonButton Save, KryptonButton Edit, KryptonButton Update, KryptonButton Delete, KryptonButton Cancel)
        {
            this.KrButtonNew = New;
            this.KrButtonSave = Save;
            this.KrButtonEdit = Edit;
            this.KrButtonUpdate = Update;
            this.KrButtonDelete = Delete;
            this.KrButtonCancel = Cancel;
        }

        /// <summary>
        /// Register the command button for enable/Disable status. This method will register the ToolStripButton.
        /// </summary>
        /// <param name="New"></param>
        /// <param name="Save"></param>
        /// <param name="Edit"></param>
        /// <param name="Update"></param>
        /// <param name="Delete"></param>
        /// <param name="Cancel"></param>
        public void RegisterCommandButtons(ToolStripButton New, ToolStripButton Save, ToolStripButton Edit, ToolStripButton Update, ToolStripButton Delete, ToolStripButton Cancel)
        {
            this.ToolStripNew = New;
            this.ToolStripSave = Save;
            this.ToolStripEdit = Edit;
            this.ToolStripUpdate = Update;
            this.ToolStripDelete = Delete;
            this.ToolStripCancel = Cancel;
        }

        /// <summary>
        /// Set the command button enable/Disable status (this method will work for both Button control and ToolStripButton) and pass the IsAdmin or not.
        /// </summary>
        public void SetCommandButtonStatus(clsCommon.ButtonStatus Status, bool IsAdmin)
        {
            switch (Status)
            {
                case clsCommon.ButtonStatus.AfterCancel:

                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = true;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                        else
                        {
                            ButtonNew.Enabled = false;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                    }
                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = false;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = false;
                        }
                    }
                    break;

                case clsCommon.ButtonStatus.AfterDelete:
                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = true;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                    }
                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = true;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = false;
                        }
                    }
                    break;

                case clsCommon.ButtonStatus.AfterEdit:

                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = false;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = true;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = true;
                        }
                        else
                        {
                            ButtonNew.Enabled = false;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = true;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = true;
                        }
                    }
                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = false;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = true;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = true;
                        }
                        else
                        {
                            ToolStripNew.Enabled = false;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = true;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = true;
                        }
                    }
                    break;

                case clsCommon.ButtonStatus.AfterGridClick:

                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = true;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = true;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = true;
                            ButtonCancel.Enabled = true;
                        }
                        else
                        {
                            ButtonNew.Enabled = false;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = true;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = true;
                        }
                    }

                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = true;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = true;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = true;
                            ToolStripCancel.Enabled = true;
                        }
                        else
                        {
                            ToolStripNew.Enabled = false;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = true;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = true;
                        }
                    }
                    break;
                case clsCommon.ButtonStatus.AfterNew:

                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = false;
                            ButtonSave.Enabled = true;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = true;
                        }
                    }

                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = false;
                            ToolStripSave.Enabled = true;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = true;
                        }

                    }
                    break;
                case clsCommon.ButtonStatus.AfterSave:

                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = true;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                    }

                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = true;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = false;
                        }
                    }
                    break;

                case clsCommon.ButtonStatus.AfterUpdate:

                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = true;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                        else
                        {
                            ButtonNew.Enabled = false;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                    }
                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = true;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = false;
                        }
                        else
                        {
                            ToolStripNew.Enabled = false;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = false;
                        }
                    }
                    break;
                case clsCommon.ButtonStatus.Beginning:

                    if (ButtonNew != null)
                    {
                        if (IsAdmin)
                        {
                            ButtonNew.Enabled = true;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                        else
                        {
                            ButtonNew.Enabled = false;
                            ButtonSave.Enabled = false;
                            ButtonEdit.Enabled = false;
                            ButtonUpdate.Enabled = false;
                            ButtonDelete.Enabled = false;
                            ButtonCancel.Enabled = false;
                        }
                    }
                    if (ToolStripNew != null)
                    {
                        if (IsAdmin)
                        {
                            ToolStripNew.Enabled = true;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = false;
                        }
                        else
                        {
                            ToolStripNew.Enabled = false;
                            ToolStripSave.Enabled = false;
                            ToolStripEdit.Enabled = false;
                            ToolStripUpdate.Enabled = false;
                            ToolStripDelete.Enabled = false;
                            ToolStripCancel.Enabled = false;
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// Set the command button enable/Disable status (this method will work for both Button control and ToolStripButton.
        /// </summary>

        public void SetCommandButtonStatus(clsCommon.ButtonStatus Status)
        {
            switch (Status)
            {
                case clsCommon.ButtonStatus.AfterCancel:

                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = true;
                        ButtonSave.Enabled = false;
                        ButtonEdit.Enabled = false;
                        ButtonUpdate.Enabled = false;
                        ButtonDelete.Enabled = false;
                        ButtonCancel.Enabled = false;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = true;
                        KrButtonSave.Enabled = false;
                        KrButtonEdit.Enabled = false;
                        KrButtonUpdate.Enabled = false;
                        KrButtonDelete.Enabled = false;
                        KrButtonCancel.Enabled = false;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = true;
                        ToolStripSave.Enabled = false;
                        ToolStripEdit.Enabled = false;
                        ToolStripUpdate.Enabled = false;
                        ToolStripDelete.Enabled = false;
                        ToolStripCancel.Enabled = false;

                    }
                    break;

                case clsCommon.ButtonStatus.AfterDelete:
                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = true;
                        ButtonSave.Enabled = false;
                        ButtonEdit.Enabled = false;
                        ButtonUpdate.Enabled = false;
                        ButtonDelete.Enabled = false;
                        ButtonCancel.Enabled = false;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = true;
                        KrButtonSave.Enabled = false;
                        KrButtonEdit.Enabled = false;
                        KrButtonUpdate.Enabled = false;
                        KrButtonDelete.Enabled = false;
                        KrButtonCancel.Enabled = false;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = true;
                        ToolStripSave.Enabled = false;
                        ToolStripEdit.Enabled = false;
                        ToolStripUpdate.Enabled = false;
                        ToolStripDelete.Enabled = false;
                        ToolStripCancel.Enabled = false;
                    }
                    break;

                case clsCommon.ButtonStatus.AfterEdit:

                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = false;
                        ButtonSave.Enabled = false;
                        ButtonEdit.Enabled = false;
                        ButtonUpdate.Enabled = true;
                        ButtonDelete.Enabled = false;
                        ButtonCancel.Enabled = true;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = false;
                        KrButtonSave.Enabled = false;
                        KrButtonEdit.Enabled = false;
                        KrButtonUpdate.Enabled = true;
                        KrButtonDelete.Enabled = false;
                        KrButtonCancel.Enabled = true;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = false;
                        ToolStripSave.Enabled = false;
                        ToolStripEdit.Enabled = false;
                        ToolStripUpdate.Enabled = true;
                        ToolStripDelete.Enabled = false;
                        ToolStripCancel.Enabled = true;
                    }
                    break;

                case clsCommon.ButtonStatus.AfterGridClick:

                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = true;
                        ButtonSave.Enabled = false;
                        ButtonEdit.Enabled = true;
                        ButtonUpdate.Enabled = false;
                        ButtonDelete.Enabled = true;
                        ButtonCancel.Enabled = true;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = true;
                        KrButtonSave.Enabled = false;
                        KrButtonEdit.Enabled = true;
                        KrButtonUpdate.Enabled = false;
                        KrButtonDelete.Enabled = true;
                        KrButtonCancel.Enabled = true;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = true;
                        ToolStripSave.Enabled = false;
                        ToolStripEdit.Enabled = true;
                        ToolStripUpdate.Enabled = false;
                        ToolStripDelete.Enabled = true;
                        ToolStripCancel.Enabled = true;
                    }
                    break;

                case clsCommon.ButtonStatus.AfterNew:

                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = false;
                        ButtonSave.Enabled = true;
                        ButtonEdit.Enabled = false;
                        ButtonUpdate.Enabled = false;
                        ButtonDelete.Enabled = false;
                        ButtonCancel.Enabled = true;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = false;
                        KrButtonSave.Enabled = true;
                        KrButtonEdit.Enabled = false;
                        KrButtonUpdate.Enabled = false;
                        KrButtonDelete.Enabled = false;
                        KrButtonCancel.Enabled = true;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = false;
                        ToolStripSave.Enabled = true;
                        ToolStripEdit.Enabled = false;
                        ToolStripUpdate.Enabled = false;
                        ToolStripDelete.Enabled = false;
                        ToolStripCancel.Enabled = true;

                    }
                    break;
                case clsCommon.ButtonStatus.AfterSave:

                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = true;
                        ButtonSave.Enabled = false;
                        ButtonEdit.Enabled = false;
                        ButtonUpdate.Enabled = false;
                        ButtonDelete.Enabled = false;
                        ButtonCancel.Enabled = false;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = true;
                        KrButtonSave.Enabled = false;
                        KrButtonEdit.Enabled = false;
                        KrButtonUpdate.Enabled = false;
                        KrButtonDelete.Enabled = false;
                        KrButtonCancel.Enabled = false;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = true;
                        ToolStripSave.Enabled = false;
                        ToolStripEdit.Enabled = false;
                        ToolStripUpdate.Enabled = false;
                        ToolStripDelete.Enabled = false;
                        ToolStripCancel.Enabled = false;
                    }
                    break;

                case clsCommon.ButtonStatus.AfterUpdate:

                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = true;
                        ButtonSave.Enabled = false;
                        ButtonEdit.Enabled = false;
                        ButtonUpdate.Enabled = false;
                        ButtonDelete.Enabled = false;
                        ButtonCancel.Enabled = false;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = true;
                        KrButtonSave.Enabled = false;
                        KrButtonEdit.Enabled = false;
                        KrButtonUpdate.Enabled = false;
                        KrButtonDelete.Enabled = false;
                        KrButtonCancel.Enabled = false;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = true;
                        ToolStripSave.Enabled = false;
                        ToolStripEdit.Enabled = false;
                        ToolStripUpdate.Enabled = false;
                        ToolStripDelete.Enabled = false;
                        ToolStripCancel.Enabled = false;
                    }
                    break;
                case clsCommon.ButtonStatus.Beginning:

                    if (ButtonNew != null)
                    {
                        ButtonNew.Enabled = true;
                        ButtonSave.Enabled = false;
                        ButtonEdit.Enabled = false;
                        ButtonUpdate.Enabled = false;
                        ButtonDelete.Enabled = false;
                        ButtonCancel.Enabled = false;
                    }
                    else if (KrButtonNew != null)
                    {
                        KrButtonNew.Enabled = true;
                        KrButtonSave.Enabled = false;
                        KrButtonEdit.Enabled = false;
                        KrButtonUpdate.Enabled = false;
                        KrButtonDelete.Enabled = false;
                        KrButtonCancel.Enabled = false;
                    }
                    else if (ToolStripNew != null)
                    {
                        ToolStripNew.Enabled = true;
                        ToolStripSave.Enabled = false;
                        ToolStripEdit.Enabled = false;
                        ToolStripUpdate.Enabled = false;
                        ToolStripDelete.Enabled = false;
                        ToolStripCancel.Enabled = false;
                    }
                    break;
            }
        }

        

        private void SetDataPopup(DataGridView dgv)
        {
            dgvDataPopup = dgv;
        }

        /// <summary>
        /// Set the size of  popup window ( auto Extender ) shown below the textbox.
        /// </summary>
        /// <param name="width">Pass the width. if you don't want to change the width then set it as 0</param>
        /// <param name="height">Pass the height. if you don't want to change the height then set it as 0 </param>
        public void SetDataPopupSize(int width, int height)
        {
            if (ObjDgvPopup != null)
            {
                if (width != 0 && height != 0)
                {
                    ObjDgvPopup.Size = new Size(width, height);
                }
                else if (width == 0)
                {
                    ObjDgvPopup.Size = new Size(ObjDgvPopup.Size.Width, height);
                }
                else if (height == 0)
                {
                    ObjDgvPopup.Size = new Size(width, ObjDgvPopup.Size.Height);
                }
            }
        }
        /// <summary>
        /// Get the popup window ( auto Extender ) shown below the textbox.
        /// </summary>
        /// <returns>returns the data grid (auto extender control)</returns>
        public DataGridView GetDataPopup()
        {
            if (dgvDataPopup != null)
            {
                return dgvDataPopup;
            }

            return null;
        }
        frmDataPopup ObjDgvPopup;

        /// <summary>
        /// this method will close the auto extender popup window.
        /// we can 
        /// </summary>
        public void CloseAutoExtender()
        {
            // if form is not null and handle is there and 
            if (ObjDgvPopup != null && ObjDgvPopup.IsHandleCreated && clsCommon.IsForCompleted)
            {
                ObjDgvPopup.Close();
            }

            else if (ObjDgvPopup != null && ObjDgvPopup.IsHandleCreated && clsCommon.IsPoupGridCellClick == false)
            {
                ObjDgvPopup.Close();
                // is user has not click on the grid then you can close it in case no item is found. 
            }
        }

        /// <summary>
        /// Show the Auto Extender for text boxes.
        /// </summary>
        /// <param name="dt">Pass the data table .</param>
        /// <param name="c">Pass the control on which you want to show the auto extender.</param>
        /// <param name="frm">Pass the form name on which you want to show the auto extender.</param>
        /// <param name="ParentControl">Pass the parent control if any ( for example panel, group box etc) if there is no parent control then pass NULL</param>
        /// <returns></returns>
        public string ShowDataPopup(DataTable dt, Control c, Form frm, Control ParentControl)
        {
            if (!clsCommon.IsPoupGridCellClick)
            {
                if (ObjPopupControl.Count == 0)
                {
                    ShowErrorMessage("Please Set the control data first by calling SetControlData.", "Error");
                }
            }
            else
            {
                return "";
            }

            // if dataPopup is visible then dont show it agan.
            if (ObjDgvPopup != null && ObjDgvPopup.Visible)
            {
                if (ObjDgvPopup != null)
                {
                    if (c.Text.Trim().Length == 0)
                    {
                        ObjDgvPopup.Close();
                    }
                    else
                    {
                        ObjDgvPopup.dgvPopup.DataSource = dt;
                    }
                }
                if (ObjDgvPopup == null)
                {
                    ObjDgvPopup = new frmDataPopup();
                    ObjDgvPopup.dgvPopup.DataSource = dt;
                    ObjDgvPopup.BringToFront();
                    ObjDgvPopup.dgvPopup.ClearSelection();
                }
                else
                {
                    ObjDgvPopup.BringToFront();
                    ObjDgvPopup.dgvPopup.ClearSelection();
                    SetDataPopup(ObjDgvPopup.dgvPopup);
                }
                return c.Name;
            }
            else
            {
                if (ObjDgvPopup == null)
                {
                    ObjDgvPopup = new frmDataPopup();
                    ObjDgvPopup.dgvPopup.DataSource = dt;
                }
                else
                {
                    if (c.Text.Trim().Length == 0)
                    {
                        ObjDgvPopup.Close();
                    }
                    else
                    {
                        ObjDgvPopup.dgvPopup.DataSource = dt;
                    }
                }
            }

            if (clsUtility.dgvPoup != null)
            {
                clsUtility.dgvPoup.ClearSelection();
            }

            ObjDgvPopup.Name = "ObjDgvPoup";

            // get the list of controls.
            Control.ControlCollection colCollection = frm.Controls;

            // Find the specific control
            Control[] arrcol = colCollection.Find(ObjDgvPopup.Name, true);

            // if found then remove it.
            if (arrcol.Length > 0)
            {
                frm.Controls.Remove(arrcol[0]);
            }

            // if textbox is empty then don't show the grid view.
            if (IsControlTextEmpty(c))
            {
                return c.Name;
            }

            if (clsCommon.IsPoupGridCellClick)
            {
                ObjDgvPopup.dgvPopup.DataSource = dt;
                ObjDgvPopup.BringToFront();
                ObjDgvPopup.dgvPopup.ClearSelection();
                SetDataPopup(ObjDgvPopup.dgvPopup);
                return "";
            }

            if (dt == null)
            {
                return string.Empty;
            }

            ObjDgvPopup.TopLevel = false;

            // if control doesn't have any parent control then add it to the form.
            if (ParentControl == null || ParentControl.GetType() == typeof(System.Windows.Forms.Form))
            {
                frm.Controls.Add(ObjDgvPopup);
            }
            else
            {
                // if it has a parent control rather then form then add it to the parent control [ like group box or panel]
                ParentControl.Controls.Add(ObjDgvPopup);
            }

            ObjDgvPopup.Location = new Point(c.Location.X, c.Location.Y + c.Size.Height + 2);
            dgvPoup = ObjDgvPopup.dgvPopup;
            c.KeyDown += c_KeyDown;

            if (!ObjDgvPopup.IsHandleCreated)
            {
                ObjDgvPopup = null;
                ObjDgvPopup = new frmDataPopup();
                ObjDgvPopup.TopLevel = false;
                ObjDgvPopup.Name = "ObjDgvPoup";
                ObjDgvPopup.dgvPopup.DataSource = dt;
                // if control doesn't have any parent control then add it to the form.
                if (ParentControl == null || ParentControl.GetType() == typeof(System.Windows.Forms.Form))
                {
                    frm.Controls.Add(ObjDgvPopup);
                }
                else
                {
                    // if it has a parent control rather then form then add it to the parent control [ like group box or panel]
                    ParentControl.Controls.Add(ObjDgvPopup);
                }
                ObjDgvPopup.Location = new Point(c.Location.X, c.Location.Y + c.Size.Height + 2);
                dgvPoup = ObjDgvPopup.dgvPopup;
                c.KeyDown += c_KeyDown;

                ObjDgvPopup.Show();
                ObjDgvPopup.BringToFront();
                ObjDgvPopup.dgvPopup.ClearSelection();
                SetDataPopup(ObjDgvPopup.dgvPopup);
                return c.Name;
            }
            else
            {
                ObjDgvPopup.Show();
                ObjDgvPopup.BringToFront();
                ObjDgvPopup.dgvPopup.ClearSelection();
                SetDataPopup(ObjDgvPopup.dgvPopup);
                return c.Name;
            }
        }

        void c_KeyDown(object sender, KeyEventArgs e)
        {
            if (clsUtility.dgvPoup != null && e.KeyData == Keys.Down)
            {
                clsUtility.dgvPoup.Focus();
                if (dgvPoup.Rows.Count > 0)
                {
                    clsUtility.dgvPoup.Rows[0].Selected = true;
                }
            }
        }

        /// <summary>
        /// Show a message while closing the form.
        /// </summary>
        /// <param name="e">closing event argument.</param>
        /// <param name="strMessageTitle">message title.</param>
        public static void ShowFormClosingMessage(FormClosingEventArgs e, string strMessageTitle)
        {
            if (!isFormClosing)
            {
                bool b = ShowQuestionMessage("Are you sure, you want to close this window?", strMessageTitle);
                if (b)
                {
                    isFormClosing = true;
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Use this method in DataPoup window. This method binds the columns value to the specific control.
        /// For example : SetControlData(txtFirstName,"EmployeeName"). This will bind the EmployeeName column to txtFirstName textbox.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="ColumnName"></param>
        public void SetControlData(Control Value, string ColumnName)
        {
            if (!clsCommon.IsPoupGridCellClick)
            {
                if (!ObjPopupControl.ContainsKey(ColumnName))
                {
                    ObjPopupControl.Add(ColumnName, Value);
                }
            }
        }

        /// <summary>
        /// This method creates a file and write string line.
        /// </summary>
        /// <param name="path">path of the file.</param>
        /// <param name="strText">text to be written</param>
        /// <param name="Isappend">append the text or not</param>
        public void CreateFile(string path, string strText, bool Isappend)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, Isappend);
                sw.WriteLine(strText);
                sw.Close();
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "CreateFile(string path, string strText, bool Isappend)");
            }
        }

        /// <summary>
        /// This method creates a file and write string line by line.
        /// </summary>
        /// <param name="path">path of the file.</param>
        /// <param name="strText">string array to be written for multiple lines</param>
        /// <param name="Isappend">append the text or not</param>
        public void CreateFile(string path, string[] strText, bool Isappend)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, Isappend);

                foreach (string item in strText)
                {
                    sw.WriteLine(item);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "CreateFile(string path, string strText, bool Isappend)");
            }
        }

        /// <summary>
        /// Read the file from start to end
        /// </summary>
        /// <param name="path">Path of the file to be read.</param>
        /// <returns>returns the text that has been read from the file.</returns>
        public string ReadFile(string path)
        {
            try
            {
                string str = "";
                StreamReader sr = new StreamReader(path);
                str = sr.ReadToEnd();
                sr.Close();
                return str;
            }
            catch (Exception ex)
            {

                clsCommon.ShowError(ex, "ReadFile()");
                return null;
            }
        }
        /// <summary>
        /// Write a Line to the specified file.
        /// </summary>
        /// <param name="strText">Text string to be written to the file.</param>
        /// <param name="path">Path of the file.</param>
        /// <param name="append">if you want to append the content then pass true else pass false.</param>
        /// <returns></returns>
        public bool WriteToFile(string strText, string path, bool append)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, append);
                if (strText == "______________________________________")
                    sw.WriteLine(strText);
                else
                    sw.WriteLine(DateTime.Now + " - " + strText);
                sw.Close();
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            catch (Exception ex)
            {
                if (!clsCommon.isErrorWindowOpen)
                {
                    clsCommon.ShowError(ex.ToString(), "WriteToFile(string strText, string path, bool append)");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Write a text to the specified path with auto append.
        /// </summary>
        /// <param name="strText">Text string to be written to the file.</param>
        /// <param name="filename">Name of file.</param>        
        /// <returns></returns>
        public bool WriteToFile(string strText, string filename)
        {
            try
            {
                filename = filename + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                StreamWriter sw = new StreamWriter(filename, true);
                sw.WriteLine("Log Date : " + DateTime.Now.ToString(), filename, true);
                sw.WriteLine("Log Text : " + strText, filename, true);
                sw.WriteLine("______________________________________", filename, true);
                sw.Close();
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            catch (Exception ex)
            {
                if (!clsCommon.isErrorWindowOpen)
                {
                    clsCommon.ShowError(ex.ToString(), "WriteToFile(string strText, string filename)");
                }
                return false;
            }
            return true;
        }

        public static bool HasFormRights(int FormID, int OperationID)
        {
            bool Result = false;
            string cmdText = "";
            try
            {
                using (SqlConnection con = new SqlConnection(clsConnection_DAL.strConnectionString))
                {
                    string strCondition = "";
                    if (OperationID == 1)
                    {
                        strCondition = "AND IsView=1";
                    }
                    else if (OperationID == 2)
                    {
                        strCondition = "AND IsSave=1";
                    }
                    else if (OperationID == 3)
                    {
                        strCondition = "AND IsUpdate=1";
                    }
                    else if (OperationID == 4)
                    {
                        strCondition = "AND IsDelete=1";
                    }
                    else if (OperationID == 5)
                    {
                        strCondition = "AND IsOther=1";
                    }

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT COUNT(1) FROM " + clsUtility.DBName + ".dbo.tblUserRights WITH(NOLOCK) WHERE FormID=" + FormID + " " + strCondition + " AND UserID=" + clsUtility.LoginID;
                    cmdText = cmd.CommandText;
                    cmd.Connection = con;
                    con.Open();
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        if (Convert.ToInt32(obj) > 0)
                        {
                            Result = true;
                        }
                        else
                        {
                            Result = false;
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, SetError("HasFormRights(int FormID, int OperationID)", cmdText));
            }
            return Result;
        }

        public static bool HasFormRights(int FormID)
        {
            bool Result = false;
            string cmdText = "";
            try
            {
                using (SqlConnection con = new SqlConnection(clsConnection_DAL.strConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT COUNT(1) FROM " + clsUtility.DBName + ".dbo.tblUserRights WITH(NOLOCK) WHERE FormID=" + FormID + " AND UserID=" + clsUtility.LoginID;
                    cmdText = cmd.CommandText;
                    cmd.Connection = con;
                    con.Open();
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        if (Convert.ToInt32(obj) > 0)
                        {
                            Result = true;
                        }
                        else
                        {
                            Result = false;
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, SetError("HasFormRights(int FormID)", cmdText));
            }
            return Result;
        }
        private static string SetError(string strMethod, string cmdText)
        {
            return " " + strMethod + " <BR><BR><FONT FACE='Courier New'> <b>CommandText : </b> " + cmdText + "</Font><BR><BR>";
        }

        /// <summary>
        /// This method is checking whether Entered Form is open or not.
        /// </summary>
        /// <param name="formType">Enter name of Form. i.e typeof(Form Name)</param>
        /// <returns>Return True if Form is opened</returns>
        public bool IsAlreadyOpen(Type formType)
        {
            bool isOpen = false;
            try
            {
                foreach (Form f in Application.OpenForms)
                {
                    if (f.GetType() == formType)
                    {
                        f.BringToFront();
                        f.WindowState = FormWindowState.Normal;
                        isOpen = true;
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex.ToString(), "IsAlreadyOpen(Type formType)");
            }
            return isOpen;
        }

        /// <summary>
        /// This method is checking whether Entered Form is open or not, if yes then close.
        /// </summary>
        /// <param name="formType">Enter name of Form. i.e typeof(Form Name)</param>
        ///// <returns>Return True if Form is Closed</returns>
        public void CloseAlreadyOpen(Type formType)
        {
            try
            {
                foreach (Form f in Application.OpenForms)
                {
                    if (f.GetType() == formType)
                    {
                        f.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex.ToString(), "CloseAlreadyOpen(Type formType)");
            }
        }
    }
}