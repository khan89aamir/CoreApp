using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace CoreApp
{
    /// <summary>
    /// This class contains Multi-Threading and Thread safe functions.
    /// </summary>
    public class clsThreadTask
    {
        #region Delegates List.

        private delegate void dSetControlText(Control c, string str);
        private delegate int dSetProgressBarValue(ProgressBar c, int curValue, int MaxValue);
        private delegate void dSetOwnerForm(Form Parent, Form Child);
        private delegate void dCloseLoading();
        private delegate void dSetLoadingProgressPercent(int CurValue, int MaxValue);
        private delegate string dGetControlText(Control c);
        private delegate void dSetControlVisible(Control c, bool Value);
        private delegate void dSetControlEnable(Control c, bool Value);
        #endregion

        #region Private variables and objects.
        Form Frm;
        LoadingDialog ObjLoading = new LoadingDialog();
        private LoadingContinue ObjImageLoading = new LoadingContinue();
        bool LoadingDialogOpen = false;
        bool ImageLoadingDialogOpen = false;
        int ProgressPercent;
        Thread ThrLoading;
        Thread thrImageLoading;
        private string DialogTitle, LableText;



        /// <summary>
        /// Set the seize of the picture box.
        /// </summary>
        public PictureBoxSizeMode picSizeMode;
        private Color _ImageLoadingBackColor;
        private Image _ImageLoadingBackImage;
        private Size _ImageLoadingSize;
        private Font _ImageLoadingMessageFont;
        private Color _ImageLoadingLableForeColor;
        Color _LoadingBackColor;
        Image _LoadingBackImage;
        Size _LoadingSize;
        Font _LoadingMessageFont;
        Color _LoadingLableForeColor;
        private string strControlText;
        #endregion

        /// <summary>
        /// Set the owner form
        /// </summary>
        /// <param name="Parent">Pass the Parent form</param>
        /// <param name="Child">Pass the child form</param>
        private void SetOwner(Form Parent, Form Child)
        {
            if (Parent.InvokeRequired)
            {
                dSetOwnerForm d = new dSetOwnerForm(SetOwner);
                Parent.Invoke(d, new object[] { Parent, Child });
            }
            else
            {
                Child.Owner = Parent;
                Child.ShowDialog();
            }
        }

        #region Image Loading Dialog Box Properties.

        /// <summary>
        /// Get or Set the back color of ImageLoading Dialog Box.
        /// If you set the background image then back color will not be set.
        /// </summary>
        public Color ImageLoadingBackColor
        {
            get { return _ImageLoadingBackColor; }
            set { _ImageLoadingBackColor = value; }
        }

        /// <summary>
        ///  Set the background image of the ImageLoading window.
        /// </summary>
        public Image ImageLoadingBackImage
        {

            set { _ImageLoadingBackImage = value; }
        }

        /// <summary>
        ///Get or Set the size of the ImageLoading Window.
        /// </summary>
        public Size ImageLoadingSize
        {
            get { return _ImageLoadingSize; }
            set { _ImageLoadingSize = value; }
        }

        /// <summary>
        /// Set the Font of the Label on the ImageLoading Dialog window.
        /// </summary>
        public Font ImageLoadingMessageFont
        {
            get { return _ImageLoadingMessageFont; }
            set { _ImageLoadingMessageFont = value; }
        }

        /// <summary>
        /// Get or Set the fore Color of the message label on ImageLoading Dialog Window.
        /// </summary>
        public Color ImageLoadingLableForeColor
        {
            get { return _ImageLoadingLableForeColor; }
            set { _ImageLoadingLableForeColor = value; }
        }
        #endregion


        #region Loading Dialog Box Proerties.

        /// <summary>
        /// Get or Set the back color of Loading Dialog Box.
        /// If you set the background image then back color will not be set.
        /// </summary>
        public Color LoadingBackColor
        {
            get { return _LoadingBackColor; }
            set { _LoadingBackColor = value; }
        }

        /// <summary>
        ///  Set the background image of the Loading window.
        /// Pass the Path of the Background Image file if your want to set the Background Image.
        /// </summary>
        public Image LoadingBackImage
        {

            set { _LoadingBackImage = value; }
        }

        /// <summary>
        ///Get or Set the size of the Loading Window.
        /// </summary>
        public Size LoadingSize
        {
            get { return _LoadingSize; }
            set { _LoadingSize = value; }
        }

        /// <summary>
        /// Get or Set the Font of the Label on the ImageLoading Dialog window.
        /// </summary>
        public Font LoadingMessageFont
        {
            get { return _LoadingMessageFont; }
            set { _LoadingMessageFont = value; }
        }

        /// <summary>
        /// Get or set the fore color of the Loadinglable
        /// </summary>
        public Color LoadingLableForeColor
        {
            get { return _LoadingLableForeColor; }
            set { _LoadingLableForeColor = value; }
        }

        #endregion

        /// <summary>
        /// Set the progress bar value for Cross-thread operation and get the progress percentage.
        /// </summary>
        /// <param name="Progressbar">Pass the progress bar object.</param>
        /// <param name="curValue">Current changing value.</param>
        /// <param name="MaxValue">Maximum Value</param>
        /// <returns>Returns the progress value in percentage (0-100 %)</returns>
        public int SetProgressValue(ProgressBar Progressbar, int curValue, int MaxValue)
        {
            // If thread is not running then skip it.
            if ((LoadingDialogOpen || ImageLoadingDialogOpen) && !clsCommon.IsThreadRunning)
            {
                return 0;
            }

            if (curValue > MaxValue)
            {
                return 0;
            }

            if (Progressbar.InvokeRequired)
            {
                dSetProgressBarValue d = new dSetProgressBarValue(SetProgressValue);
                Progressbar.Invoke(d, new object[] { Progressbar, curValue, MaxValue });
            }
            else
            {
                Progressbar.Maximum = MaxValue;
                Progressbar.Value = curValue;
                ProgressPercent = (curValue * 100) / MaxValue;
            }
            return ProgressPercent;
        }

        /// <summary>
        /// Show Loading Window in Multi-Threaded Application.
        /// </summary>
        /// <param name="Title">Title of the Window.</param>
        /// <param name="LableText">Message Text on the window.</param>
        /// <param name="OwnerFrm">Parent Form where you want to show Loading window.</param>

        public void ShowLoadingDialog(string Title, string LableText, Form OwnerFrm)
        {
            if (ObjLoading.IsHandleCreated == false)
            {
                ObjLoading = null;
                ObjLoading = new LoadingDialog();
            }

            this.Frm = OwnerFrm; ;
            this.DialogTitle = Title;
            this.LableText = LableText;
            ObjLoading.Text = DialogTitle;

            if (_LoadingBackColor != null)
            {
                ObjLoading.BackColor = _LoadingBackColor;
            }
            if (_LoadingBackImage != null)
            {
                ObjLoading.BackgroundImage = _LoadingBackImage;
            }

            if (_LoadingMessageFont != null)
            {
                ObjLoading.lblMessage.Font = LoadingMessageFont;
            }
            ObjLoading.lblMessage.ForeColor = _LoadingLableForeColor;

            if (_LoadingSize.Height != 0 && _LoadingSize.Width != 0)
            {
                ObjLoading.Size = _LoadingSize;
            }

            ObjLoading.lblMessage.Text = LableText;

            ThrLoading = new Thread(new ThreadStart(ShowLoading));
            ThrLoading.Start();
        }

        /// <summary>
        /// Show the loading dialog with progress bar.
        /// </summary>
        private void ShowLoading()
        {
            clsCommon.IsThreadRunning = true;
            LoadingDialogOpen = true;
            SetOwner(Frm, ObjLoading);
        }

        /// <summary>
        /// Set the Progress bar Value on Loading Dialog in Multi-Threaded Application.
        /// </summary>
        /// <param name="CurValue">Current Value.</param>
        /// <param name="MaxValue">Maximum Value</param>
        public void SetLoadingProgressPercent(int CurValue, int MaxValue)
        {
            // If thread is not running then skip it.
            if ((LoadingDialogOpen || ImageLoadingDialogOpen) && !clsCommon.IsThreadRunning)
            {
                return;
            }

            if (CurValue > MaxValue)
            {
                return;
            }
            if (ObjLoading.InvokeRequired)
            {
                dSetLoadingProgressPercent d = new dSetLoadingProgressPercent(SetLoadingProgressPercent);
                ObjLoading.Invoke(d, new object[] { CurValue, MaxValue });
            }
            else
            {
                ObjLoading.progressBar1.Value = CurValue;
                ObjLoading.progressBar1.Maximum = MaxValue;
                ObjLoading.lblProgress.Text = ((CurValue * 100) / MaxValue).ToString() + "%";
            }
        }

        /// <summary>
        /// Close the Loading Window in Multi-Thread Application.
        /// </summary>
        public void CloseLoadingDialog()
        {
            try
            {
                if (ObjLoading.InvokeRequired)
                {
                    dCloseLoading d = new dCloseLoading(CloseLoadingDialog);
                    ObjLoading.Invoke(d, null);
                }
                else
                {
                    ThrLoading.Abort();
                    ObjLoading.Close();
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "CloseLoading()");
            }
        }

        /// <summary>
        /// Show Loading window the Loading animated image.
        /// This window will not show any progress of the operation.
        /// </summary>
        /// <param name="Title">Title of the window.</param>
        /// <param name="LableText">Message text on the window.</param>
        /// <param name="OwnerFrm">Parent form on which you want to show the window.</param>
        ///<param name="LoadingImage">Image that you want to show on the loading window.</param>

        public void ShowImageLoading(string Title, string LableText, Form OwnerFrm, Image LoadingImage)
        {
            if (ObjImageLoading.IsHandleCreated == false)
            {
                ObjImageLoading = null;
                ObjImageLoading = new LoadingContinue();
            }

            this.Frm = OwnerFrm; ;
            this.DialogTitle = Title;
            this.LableText = LableText;

            if (_ImageLoadingBackColor != null)
            {
                ObjImageLoading.BackColor = _ImageLoadingBackColor;
            }
            ObjImageLoading.picLoading.SizeMode = this.picSizeMode;
            if (_ImageLoadingBackImage != null)
            {
                ObjImageLoading.BackgroundImage = _ImageLoadingBackImage;
            }

            if (_ImageLoadingMessageFont != null)
            {
                ObjImageLoading.lblMessage.Font = _ImageLoadingMessageFont;
            }

            ObjImageLoading.lblMessage.ForeColor = _ImageLoadingLableForeColor;

            if (_ImageLoadingSize.Height != 0 && _ImageLoadingSize.Width != 0)
            {
                ObjImageLoading.Size = _ImageLoadingSize;
            }

            if (LoadingImage != null)
            {
                ObjImageLoading.picLoading.Image = LoadingImage;
            }
            ObjImageLoading.Text = DialogTitle;
            ObjImageLoading.lblMessage.Text = LableText;

            thrImageLoading = new Thread(new ThreadStart(ShowLoading2));
            thrImageLoading.Start();
        }

        /// <summary>
        /// Get the control text in thread safe manner.
        /// </summary>
        /// <param name="c">Pass the control.</param>
        /// <returns>Returns the control text.</returns>
        public string GetControlText(Control c)
        {
            // If thread is not running then skip it.
            if ((LoadingDialogOpen || ImageLoadingDialogOpen) && !clsCommon.IsThreadRunning)
            {
                return string.Empty;
            }

            if (c.InvokeRequired)
            {
                dGetControlText d = new dGetControlText(GetControlText);
                c.Invoke(d, new object[] { c });
            }
            else
            {
                strControlText = c.Text;
            }
            return strControlText;
        }

        /// <summary>
        /// Show the Loading Dialog with Loading image.
        /// </summary>
        private void ShowLoading2()
        {
            clsCommon.IsThreadRunning = true;
            ImageLoadingDialogOpen = true;
            SetOwner(Frm, ObjImageLoading);
        }

        /// <summary>
        /// Close the Image Loading window
        /// </summary>
        public void CloseImageLoadingDialog()
        {
            try
            {
                if (ObjImageLoading.InvokeRequired)
                {
                    dCloseLoading d = new dCloseLoading(CloseImageLoadingDialog);
                    ObjImageLoading.Invoke(d, null);
                }
                else
                {
                    ObjImageLoading.Close();
                    thrImageLoading.Abort();
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "CloseImageLoadingDialog()");
            }
        }

        /// <summary>
        /// Set the control text in  thread safe manner operation.
        /// </summary>
        /// <param name="c1">Control</param>
        /// <param name="str">Value</param>
        public void SetControlText(Control c1, string str)
        {
            // If thread is not running then skip it.
            if ((LoadingDialogOpen || ImageLoadingDialogOpen) && !clsCommon.IsThreadRunning)
            {
                return;
            }

            if (c1.InvokeRequired)
            {
                dSetControlText d = new dSetControlText(SetControlText);
                c1.Invoke(d, new object[] { c1, str });

            }
            else
            {
                c1.Text = str;
            }
        }

        /// <summary>
        /// Set the control visible true or false in thread-safe manner.
        /// </summary>
        /// <param name="c">Pass the control</param>
        /// <param name="Value">Set the visibility of the control true or false.</param>
        public void SetControlVisible(Control c, bool Value)
        {
            if (c.InvokeRequired)
            {
                dSetControlVisible d = new dSetControlVisible(SetControlVisible);
                c.Invoke(d, new object[] { c, Value });
            }
            else
            {
                c.Visible = Value;
            }
        }

        /// <summary>
        /// Set control enable or disable in thread-safe manner. 
        /// </summary>
        /// <param name="c">Pass the control</param>
        /// <param name="Value">set controls enable property to true or false.</param>
        public void SetControlEnable(Control c, bool Value)
        {
            if (c.InvokeRequired)
            {
                dSetControlEnable d = new dSetControlEnable(SetControlEnable);
                c.Invoke(d, new object[] { c, Value });
            }
            else
            {
                c.Enabled = Value;
            }
        }
    }
}
