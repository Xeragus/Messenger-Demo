using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WhatsAppApi;

namespace MessengerDemo
{
    public partial class formWhatsApp : Form
    {
        WhatsApp wa;
       

        public formWhatsApp()
        {
            InitializeComponent();
        }

        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signOutToolStripMenuItem.Visible = false;
            wa.Disconnect();
            panel2.Enabled = false;
            panel1.Enabled = true;
            panel1.BringToFront();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAbout frm = new frmAbout())
            {
                frm.ShowDialog();
            }
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            wa = new WhatsApp(Properties.Settings.Default.PhoneNumber, Properties.Settings.Default.Password, Properties.Settings.Default.FullName, true);
            wa.OnLoginSuccess += Wa_OnLoginSuccess;
            wa.OnLoginFailed += Wa_OnLoginFailed;
            wa.OnConnectFailed += Wa_OnConnectFailed;
            wa.Connect();
            wa.Login();
        }

        private void Wa_OnLoginSuccess(string phoneNumber, byte[] data)
        {
            panel1.Enabled = false;
            panel2.BringToFront();
            panel2.Enabled = true;
            signOutToolStripMenuItem.Visible = true;
            Globals.DB.Accounts.Clear();
            Globals.DB.Users.Clear();
            Globals.DB.AcceptChanges();
            string accountFile = string.Format("{0}\\accounts.dat", Application.StartupPath);
            if (File.Exists(accountFile))
                Globals.DB.Accounts.ReadXml(accountFile);
            string userFile = string.Format("{0}\\users.dat", Application.StartupPath);
            if (File.Exists(userFile))
                Globals.DB.Users.ReadXml(userFile);
            LoadData();
        }

        private void LoadData()
        {
            var query = from user in Globals.DB.Users
                        where user.AccountID == Properties.Settings.Default.PhoneNumber
                        select new
                        {
                            PhoneNumber = user.UserID,
                            FullName = user.FullName,
                            Display = string.Format("{0} (+{1})", user.FullName, user.UserID)
                        };
            lbUsers.DataSource = query.ToList();
        }

        private void Wa_OnLoginFailed(string data)
        {
            MessageBox.Show(string.Format("Login failed: {0}", data), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Wa_OnConnectFailed(Exception ex)
        {
            MessageBox.Show(string.Format("Connect failed: {0}", ex.StackTrace), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void lllbNewAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (formRegister frm = new formRegister())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    tbPhoneNumber.Text = Properties.Settings.Default.PhoneNumber;
                    tbPassword.Text = Properties.Settings.Default.Password;
                }
            }
        }

        private void formWhatsApp_Load(object sender, EventArgs e)
        {
            signOutToolStripMenuItem.Visible = false;
            panel1.BringToFront();
            panel2.Enabled = false;
            lbUsers.DisplayMember = "Display";
            lbUsers.ValueMember = "PhoneNumber";
            if (Properties.Settings.Default.Remember)
            {
                tbPhoneNumber.Text = Properties.Settings.Default.PhoneNumber;
                tbPassword.Text = Properties.Settings.Default.Password;
                ckbRemember.Checked = Properties.Settings.Default.Remember;
            }
        }

        private void ckbRemember_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Remember = ckbRemember.Checked;
            Properties.Settings.Default.PhoneNumber = tbPhoneNumber.Text;
            Properties.Settings.Default.Password = tbPassword.Text;
            Properties.Settings.Default.Save();
            
        }

        private void btnAddEdit_Click(object sender, EventArgs e)
        {
            using (frmAddEditUser frm = new frmAddEditUser(lbUsers.SelectedItem))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    LoadData();
                lbUsers.SelectedIndex = -1;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbUsers.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to remove this phone number?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var obj = lbUsers.SelectedItem;
                    if (obj != null)
                    {
                        AppData.UsersRow row = Globals.DB.Users.FindByUserID(obj.GetType().GetProperty("PhoneNumber").GetValue(obj, null).ToString());
                        Globals.DB.Users.RemoveUsersRow(row);
                        Globals.DB.Users.AcceptChanges();
                        Globals.DB.Users.WriteXml(string.Format("{0}\\users.dat", Application.StartupPath));
                        LoadData();
                        lbUsers.SelectedIndex = -1;
                    }
                }
            }
        }
    }
}
