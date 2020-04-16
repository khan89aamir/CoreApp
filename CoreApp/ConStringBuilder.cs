using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CoreApp
{
    internal partial class ConStringBuilder : Form
    {
        public ConStringBuilder()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtDataSource.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter Data Source.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            else if (txtInitialCatalog.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter Initial Catalog.(Database name)", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            else if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select Integrated Security.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 2)
            {
                if (txtUserID.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Enter User ID.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }

                if (txtPassword.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Enter Password.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            SqlConnectionStringBuilder ConBuilder = new SqlConnectionStringBuilder();
            ConBuilder.DataSource = txtDataSource.Text;

            ConBuilder.InitialCatalog = txtInitialCatalog.Text;
            if (comboBox1.SelectedIndex == 1)
            {
                ConBuilder.IntegratedSecurity = Convert.ToBoolean(comboBox1.SelectedItem);
            }
            else
            {
                ConBuilder.UserID = txtUserID.Text;
                ConBuilder.Password = txtPassword.Text;
            }

            if (checkBox1.Checked)
            {
                ConBuilder.MultipleActiveResultSets = checkBox1.Checked;
            }

            if (chkPooling.Checked)
            {
                ConBuilder.Pooling = chkPooling.Checked;
            }

            if (chkPooling.Checked)
            {
                if (txtMaxPoolSize.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Enter MaxPool Size.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                if (txtMinPoolSize.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Enter MinPool Size.", "Connection String Builder.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                ConBuilder.MaxPoolSize = Convert.ToInt32(txtMaxPoolSize.Text);
                ConBuilder.MinPoolSize = Convert.ToInt32(txtMinPoolSize.Text);
            }

            ConnectingString Obj = new ConnectingString();
            Obj.txtDataSource.Text = ConBuilder.ConnectionString;
            Obj.ShowDialog();
        }

        private void chkPooling_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPooling.Checked)
            {
                panel1.Enabled = true;
            }
            else
            {
                panel1.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 2)
            {
                panel2.Enabled = true;
            }
            else
            {
                panel2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConnectingString Obj = new ConnectingString();
            Obj.ShowDialog();
        }
    }
}