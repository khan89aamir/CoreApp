using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreApp
{
    public partial class Form_Alert : KryptonForm
    {
        private Form_Alert.enmAction action;
        private int x, y;

        /// <summary>
        /// 
        /// </summary>
        public Form_Alert()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public enum enmAction
        {
            wait,
            start,
            close
        }

        /// <summary>
        /// 
        /// </summary>
        public enum enmType
        {
            Success,
            Warning,
            Error,
            Info
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (this.action)
            {
                case enmAction.wait:
                    timer1.Interval = 5000;
                    action = enmAction.close;
                    break;
                case Form_Alert.enmAction.start:
                    this.timer1.Interval = 1;
                    this.Opacity += 0.1;
                    if (this.x < this.Location.X)
                    {
                        this.Left--;
                    }
                    else
                    {
                        if (this.Opacity == 1.0)
                        {
                            action = Form_Alert.enmAction.wait;
                        }
                    }
                    break;
                case enmAction.close:
                    timer1.Interval = 1;
                    this.Opacity -= 0.1;

                    this.Left -= 3;
                    if (base.Opacity == 0.0)
                    {
                        base.Close();
                    }
                    break;
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            action = enmAction.close;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <param name="title"></param>
        public void showAlert(string msg, enmType type, string title = "")
        {
            try
            {
                this.Opacity = 0.0;
                this.StartPosition = FormStartPosition.Manual;
                string fname;

                for (int i = 1; i < 10; i++)
                {
                    fname = "alert" + i.ToString();
                    Form_Alert frm = (Form_Alert)Application.OpenForms[fname];

                    if (frm == null)
                    {
                        this.Name = fname;
                        this.x = Screen.PrimaryScreen.WorkingArea.Width - this.Width + 15;
                        this.y = Screen.PrimaryScreen.WorkingArea.Height - this.Height * i - 5 * i;
                        this.Location = new Point(this.x, this.y);
                        break;
                    }

                }
                this.x = Screen.PrimaryScreen.WorkingArea.Width - base.Width - 5;

                switch (type)
                {
                    case enmType.Success:
                        this.picMessageType.Image = Properties.Resources.success;
                        this.BackColor = Color.SeaGreen;
                        break;
                    case enmType.Error:
                        this.picMessageType.Image = Properties.Resources.error;
                        this.BackColor = Color.DarkRed;
                        break;
                    case enmType.Info:
                        this.picMessageType.Image = Properties.Resources.info;
                        this.BackColor = Color.RoyalBlue;
                        break;
                    case enmType.Warning:
                        this.picMessageType.Image = Properties.Resources.warning;
                        this.BackColor = Color.DarkOrange;
                        break;
                }

                this.lblTitleMessage.Text = title;
                this.lblMsg.Text = msg;
                this.TopMost = true;
                this.Show();
                this.action = enmAction.start;
                this.timer1.Interval = 1;
                this.timer1.Start();
            }
            catch(Exception ex)
            {
                clsCommon.ShowError(ex.ToString(), "showAlert()");
            }
        }
    }
}