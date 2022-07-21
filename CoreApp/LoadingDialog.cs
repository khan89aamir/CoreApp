using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ComponentFactory.Krypton.Toolkit;

namespace CoreApp
{
    internal partial class LoadingDialog : KryptonForm
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