namespace CoreApp
{
    partial class frmDataPopup
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvPopup = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPopup)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPopup
            // 
            this.dgvPopup.AllowUserToAddRows = false;
            this.dgvPopup.AllowUserToDeleteRows = false;
            this.dgvPopup.AllowUserToResizeColumns = false;
            this.dgvPopup.AllowUserToResizeRows = false;
            this.dgvPopup.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPopup.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPopup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPopup.Location = new System.Drawing.Point(0, 0);
            this.dgvPopup.Name = "dgvPopup";
            this.dgvPopup.ReadOnly = true;
            this.dgvPopup.Size = new System.Drawing.Size(289, 152);
            this.dgvPopup.TabIndex = 0;
            this.dgvPopup.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPopup_CellClick);
            this.dgvPopup.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            this.dgvPopup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvPopup_KeyDown);
            // 
            // frmDataPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(289, 152);
            this.Controls.Add(this.dgvPopup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmDataPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmDataPopup";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPopup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.DataGridView dgvPopup;

    }
}