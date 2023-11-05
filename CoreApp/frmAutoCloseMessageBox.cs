using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CoreApp
{
    public partial class frmAutoCloseMessageBox : KryptonForm
    {
        /// <summary>
        /// Auto Close MessageBox
        /// </summary>
        public frmAutoCloseMessageBox()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Interval == clsUtility.AutoCloseMessageTimer)
            {
                timer1.Stop();

                Form activeForm = Form.ActiveForm;
                if (activeForm != null)
                {
                    activeForm.Close();
                }
                this.Close();
            }

        }

        private void frmAutoCloseMessageBox_Load(object sender, EventArgs e)
        {
            timer1.Interval = clsUtility.AutoCloseMessageTimer;
            timer1.Enabled = true;
            timer1.Start();

            if (this.Text.Contains("Error"))
                KryptonMessageBox.Show(lblMessage.Text, clsUtility.strProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                KryptonMessageBox.Show(lblMessage.Text, clsUtility.strProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            this.Close();
        }
    }
}