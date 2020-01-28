using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace CoreApp
{
    internal partial class ConnectingString : Form
    {
        public ConnectingString()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtDataSource.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter Connecting String First.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (isDecrypt)
            {
                MessageBox.Show("Connecting String is already decrypted.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            string str = Decrypt(txtDataSource.Text, true);
            txtDataSource.Text = str;
            isEncrypt = false;
            isDecrypt = true;
        }
        bool isEncrypt;
        bool isDecrypt;
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtDataSource.Text.Trim().Length==0)
            {
                MessageBox.Show("Enter Connecting String First.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (isEncrypt)
            {
                MessageBox.Show("Connecting String is already encrypted.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
                
            }
            
            string str=  Encrypt(txtDataSource.Text,true);
           txtDataSource.Text = str;
           isEncrypt = true;
           isDecrypt = false;
        }
        /// <summary>
        /// Encrypt a string using dual encryption method. Return a encrypted cipher Text
        /// </summary>
        /// <param name="toEncrypt">string to be encrypted</param>
        /// <param name="useHashing">use hashing? send to for extra secirity</param>
        /// <returns></returns>
        public string Encrypt(string toEncrypt, bool useHashing)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="cipherString">encrypted string</param>
        /// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
        /// <returns></returns>
        public string Decrypt(string cipherString, bool useHashing)
        {
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
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtDataSource.Text);
            MessageBox.Show("Connection string copied to the clipboard");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists("AppConfig"))
            {
                Directory.CreateDirectory("AppConfig");
            }

            StreamWriter sw = new StreamWriter("AppConfig/Server.sc",true);
            sw.WriteLine(txtDataSource.Text);
            sw.Close();
            DirectoryInfo d = new DirectoryInfo("AppConfig");

            clsUtility.ShowInfoMessage("Connection string is saved successfully.\nFile Path: "+d.FullName, "CoreApp");
        }
    }
}
