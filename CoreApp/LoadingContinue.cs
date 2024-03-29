﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace CoreApp
{
    /// <summary>
    /// Loading form class with loading image.
    /// </summary>
    public partial class LoadingContinue : KryptonForm
    {
        /// <summary>
        /// Initialize the loading form with loading image.
        /// </summary>
        public LoadingContinue()
        {
            InitializeComponent();
        }

        private void LoadingContinue_FormClosing(object sender, FormClosingEventArgs e)
        {
            clsCommon.IsThreadRunning = false;
        }
    }
}