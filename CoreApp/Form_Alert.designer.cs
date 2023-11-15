namespace CoreApp
{
    partial class Form_Alert
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblMsg = new System.Windows.Forms.Label();
            this.picMessageType = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.picClose = new System.Windows.Forms.PictureBox();
            this.lblTitleMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picMessageType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.ForeColor = System.Drawing.Color.White;
            this.lblMsg.Location = new System.Drawing.Point(8, 42);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(102, 17);
            this.lblMsg.TabIndex = 0;
            this.lblMsg.Text = "Message Text";
            // 
            // picMessageType
            // 
            this.picMessageType.Image = global::CoreApp.Properties.Resources.info;
            this.picMessageType.Location = new System.Drawing.Point(2, 0);
            this.picMessageType.Name = "picMessageType";
            this.picMessageType.Size = new System.Drawing.Size(41, 39);
            this.picMessageType.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMessageType.TabIndex = 2;
            this.picMessageType.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // picClose
            // 
            this.picClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picClose.Image = global::CoreApp.Properties.Resources.Close;
            this.picClose.Location = new System.Drawing.Point(354, 0);
            this.picClose.Name = "picClose";
            this.picClose.Size = new System.Drawing.Size(26, 30);
            this.picClose.TabIndex = 3;
            this.picClose.TabStop = false;
            this.picClose.Click += new System.EventHandler(this.picClose_Click);
            // 
            // lblTitleMessage
            // 
            this.lblTitleMessage.AutoSize = true;
            this.lblTitleMessage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleMessage.ForeColor = System.Drawing.Color.White;
            this.lblTitleMessage.Location = new System.Drawing.Point(49, 13);
            this.lblTitleMessage.Name = "lblTitleMessage";
            this.lblTitleMessage.Size = new System.Drawing.Size(140, 19);
            this.lblTitleMessage.TabIndex = 4;
            this.lblTitleMessage.Text = "Title Message Text";
            // 
            // Form_Alert
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(382, 74);
            this.Controls.Add(this.lblTitleMessage);
            this.Controls.Add(this.picClose);
            this.Controls.Add(this.picMessageType);
            this.Controls.Add(this.lblMsg);
            this.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Alert";
            this.Text = "Form_Alert";
            ((System.ComponentModel.ISupportInitialize)(this.picMessageType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.PictureBox picMessageType;
        internal System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox picClose;
        private System.Windows.Forms.Label lblTitleMessage;
    }
}