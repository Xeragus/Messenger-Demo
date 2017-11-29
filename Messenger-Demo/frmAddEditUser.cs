using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessengerDemo
{
    public partial class frmAddEditUser : Form
    {
        object obj;

        public frmAddEditUser(object obj)
        {
            InitializeComponent();
            this.obj = obj;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void frmAddEditUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (obj == null)
                {
                    if (Globals.DB.Users.FindByUserID(tbPhoneNumber.Text) == null)
                    {
                        AppData.UsersRow row = Globals.DB.Users.NewUsersRow();
                        row.AccountID = Properties.Settings.Default.PhoneNumber;
                        row.UserID = tbPhoneNumber.Text;
                        row.FullName = tbFullName.Text;
                        Globals.DB.Users.AddUsersRow(row);
                        Globals.DB.Users.AcceptChanges();
                        Globals.DB.Users.WriteXml(string.Format("{0}\\users.dat", Application.StartupPath));
                        e.Cancel = false;
                    }
                    else
                    {
                        MessageBox.Show("This phone number already exists.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                }
                else
                {
                    AppData.UsersRow row = Globals.DB.Users.FindByUserID(obj.GetType().GetProperty("PhoneNumber").GetValue(obj, null).ToString());
                    if (row != null)
                    {
                        row.UserID = tbPhoneNumber.Text;
                        row.FullName = tbFullName.Text;
                        Globals.DB.AcceptChanges();
                        Globals.DB.Users.WriteXml(string.Format("{0}\\users.dat", Application.StartupPath));
                        e.Cancel = false;
                    }
                    else
                        e.Cancel = true;
                }
            }
        }

        private void frmAddEditUser_Load(object sender, EventArgs e)
        {
            if (obj != null)
            {
                tbPhoneNumber.Text = obj.GetType().GetProperty("PhoneNumber").GetValue(obj, null).ToString();
                tbFullName.Text = obj.GetType().GetProperty("FullName").GetValue(obj, null).ToString();
            }
        }
    }
}
