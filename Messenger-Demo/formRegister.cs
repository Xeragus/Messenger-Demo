using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MessengerDemo
{
    public partial class formRegister : Form
    {
        public formRegister()
        {
            InitializeComponent();
        }

        string password;

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbPhoneNumber.Text))
            {
                MessageBox.Show("Please enter your phone number.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbPhoneNumber.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tbFullName.Text))
            {
                MessageBox.Show("Please enter your full name.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbFullName.Focus();
                return;
            }
            if (WhatsAppApi.Register.WhatsRegisterV2.RequestCode(tbPhoneNumber.Text, out password, "sms"))
            {
                if (!string.IsNullOrEmpty(password))
                    Save();
                else
                {
                    grbRequestCode.Enabled = false;
                    grbConfirmCode.Enabled = true;
                }
            }
            else
                MessageBox.Show("Could not generate password.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Save()
        {
            this.grbRequestCode.Enabled = false;
            this.grbConfirmCode.Enabled = false;
            Properties.Settings.Default.PhoneNumber = tbPhoneNumber.Text;
            Properties.Settings.Default.Password = password;
            Properties.Settings.Default.FullName = tbFullName.Text;
            Properties.Settings.Default.Save();
            if (Globals.DB.Accounts.FindByAccountID(tbPhoneNumber.Text) == null)
            {
                AppData.AccountsRow row = Globals.DB.Accounts.NewAccountsRow();
                row.AccountID = tbPhoneNumber.Text;
                row.FullName = tbFullName.Text;
                row.Password = password;
                Globals.DB.Accounts.AddAccountsRow(row);
                Globals.DB.Accounts.AcceptChanges();
                Globals.DB.Accounts.WriteXml(string.Format("{0}\\accounts.dat", Application.StartupPath));
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbSMSCode.Text))
            {
                MessageBox.Show("Please enter your sms code.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbSMSCode.Focus();
                return;
            }
            password = WhatsAppApi.Register.WhatsRegisterV2.RegisterCode(tbPhoneNumber.Text, tbSMSCode.Text);
            Save();
        }
    }
}
