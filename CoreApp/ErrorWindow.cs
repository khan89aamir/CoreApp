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
    /// <summary>
    /// Error window class.
    /// </summary>
    public partial class ErrorWindow : Form
    {
        internal string errstring;
        /// <summary>
        /// Show the error window
        /// </summary>
        public ErrorWindow()
        {
            try
            {
                InitializeComponent();

                if (System.Threading.Thread.CurrentThread.GetApartmentState() != System.Threading.ApartmentState.STA)
                {
                }
            }
            catch (Exception)
            { 
            }
            
        }

        private void ErrorWindow_Load(object sender, EventArgs e)
        {
            this.BringToFront();
           clsCommon.isErrorWindowOpen = true;
        }

        private void ErrorWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            clsCommon.isErrorWindowOpen = false;
        }
    }
}
