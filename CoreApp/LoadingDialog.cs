using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace CoreApp
{
    internal partial class LoadingDialog : Form
    {
        public LoadingDialog()
        {
            InitializeComponent();
        }

        private void LoadingDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            clsCommon.IsThreadRunning = false;
        }
    }
}
