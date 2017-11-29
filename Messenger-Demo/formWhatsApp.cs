using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

        }
    }
}
