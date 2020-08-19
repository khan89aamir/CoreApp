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
    /// Data Popup window class.
    /// </summary>
    public partial class frmDataPopup : Form
    {
        /// <summary>
        /// Initialize the data popup window.
        /// </summary>
        public frmDataPopup()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Set the default width of the popup.Default is false. 
        /// </summary>
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvPopup.DefaultCellStyle.Font = new Font("Times", 10, FontStyle.Regular);
            dgvPopup.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Times", 10, FontStyle.Regular);
            dgvPopup.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.Highlight);
            dgvPopup.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvPopup.AllowUserToAddRows = false;
            dgvPopup.AllowUserToDeleteRows = false;
            dgvPopup.AllowUserToResizeColumns = true;
            dgvPopup.AllowUserToResizeRows = false;

            dgvPopup.MultiSelect = false;
            dgvPopup.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPopup.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPopup.RowHeadersVisible = false;

            int Height = dgvPopup.Rows.GetRowsHeight(DataGridViewElementStates.None);
            int Width = dgvPopup.Columns.GetColumnsWidth(DataGridViewElementStates.None);

            if (Height >= 120)
            {
                this.Height = 120;
            }
            else
            {
                this.Height = Height;
            }
            // if only one record is there then set the height as 60
            if (dgvPopup.Rows.Count == 1)
            {
                this.Height = 55;
            }
            else if (dgvPopup.Rows.Count == 2)
            {
                this.Height = 75;
            }
            else if (dgvPopup.Rows.Count == 3)
            {
                this.Height = 100;
            }
            if (!clsUtility.SetDataPoupDefaultWidth)
            {
                this.Width = Width;
            }
        }

        private void dgvPopup_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1 || e.ColumnIndex == -1)
                {
                    return;
                }
                clsCommon.IsPoupGridCellClick = true;
                DataGridViewSelectedRowCollection selectedRows = dgvPopup.SelectedRows;
                if (selectedRows.Count > 0)
                {
                    clsCommon.IsForCompleted = false;
                    for (int i = 0; i < selectedRows[0].Cells.Count; i++)
                    {
                        if (clsUtility.ObjPopupControl.ContainsKey(dgvPopup.Columns[i].Name))
                        {
                            Control c = clsUtility.ObjPopupControl[dgvPopup.Columns[i].Name];
                            if (c != null)
                            {
                                if (c.GetType() == typeof(DateTimePicker))
                                {
                                    ((DateTimePicker)c).Value = Convert.ToDateTime(selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value);
                                }
                                else if (c.GetType() == typeof(TextBox))
                                {
                                    c.Text = selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value.ToString();
                                }
                                else if (c.GetType() == typeof(RichTextBox))
                                {
                                    c.Text = selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value.ToString();
                                }
                                else if (c.GetType() == typeof(Label))
                                {
                                    c.Text = selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value.ToString();
                                }
                            }
                        }
                    }
                    clsCommon.IsForCompleted = true;
                }
                clsUtility.ObjPopupControl.Clear();
                clsCommon.IsPoupGridCellClick = false;
            }
            catch (Exception)
            {
            }
            this.Close();
        }

        private void dgvPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                clsCommon.IsPoupGridCellClick = true;
                DataGridViewSelectedRowCollection selectedRows = dgvPopup.SelectedRows;
                if (selectedRows.Count > 0)
                {
                    clsCommon.IsForCompleted = false;
                    for (int i = 0; i < selectedRows[0].Cells.Count; i++)
                    {
                        if (clsUtility.ObjPopupControl.ContainsKey(dgvPopup.Columns[i].Name))
                        {
                            Control c = clsUtility.ObjPopupControl[dgvPopup.Columns[i].Name];
                            if (c != null)
                            {
                                if (c.GetType() == typeof(DateTimePicker))
                                {
                                    ((DateTimePicker)c).Value = Convert.ToDateTime(selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value);
                                }
                                else if (c.GetType() == typeof(TextBox))
                                {
                                    c.Text = selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value.ToString();
                                }
                                else if (c.GetType() == typeof(RichTextBox))
                                {
                                    c.Text = selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value.ToString();
                                }
                                else if (c.GetType() == typeof(Label))
                                {
                                    c.Text = selectedRows[0].Cells[dgvPopup.Columns[i].Name].Value.ToString();
                                }
                            }
                        }
                    }
                    clsCommon.IsForCompleted = true;
                }
                clsUtility.ObjPopupControl.Clear();
                clsCommon.IsPoupGridCellClick = false;
                this.Close();
            }
        }

        private void dgvPopup_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            dgvPopup.Columns[e.Column.Index].ReadOnly = true;
        }
    }
}