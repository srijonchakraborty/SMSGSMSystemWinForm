/*
|-------------------------------------------------------------------------------|
|	This code was written by Srijon Chakraborty								    |
|	Main source code link on https://github.com/srijonchakro			        |
|	All my source codes are available on http://srijon.softallybd.com           |
|	C# GSM Serial Read Write	                                            |
|	LinkedIn https://bd.linkedin.com/in/srijon-chakraborty-0ab7aba7				|
|-------------------------------------------------------------------------------|
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSSystemGSM
{
    public partial class MainView : Form
    {
        SVCComPortGSMSMS obj = new SVCComPortGSMSMS();
        BOComPortGSMSMS selectedPort = null;
        public MainView()
        {
            InitializeComponent();
        }
        private void btnLoadDevices_Click(object sender, EventArgs e)
        {
            try
            {
                var itms = obj.GetGSMSMSComportList();
                comboBox1.DataSource = itms.Select(c => c.ComPortANDDescription).ToList();
                if (itms != null && itms.Count > 0)
                {
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                obj.ConnectComPortGSMSMS(selectedPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void cmbxPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmb = (ComboBox)sender;
                int selectedIndex = cmb.SelectedIndex;
                var selectedValue = cmb.SelectedValue;
                dynamic selectedPort = cmb.SelectedItem;
                var temp = (string)selectedPort;
                selectedPort = obj.GetGSMSMSComportList()?.Where(c => c.ComPortANDDescription == temp)?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length < 4)
                {
                    MessageBox.Show("Write Number Properly.");
                    return;
                }
                if (richTextBox1.Text.Length < 4)
                {
                    MessageBox.Show("Write Min Text.");
                    return;
                }
                if (obj.IsDeviceConnected == false)
                    MessageBox.Show("Device is not Connected. Please Connect...");
                else if (selectedPort == null)
                    MessageBox.Show("No Device Selected...");
                else
                {
                    bool temp = false;
                    obj.SendComPortGSMSMS(textBox1.Text, richTextBox1.Text, out temp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //Read
                if (obj.IsDeviceConnected == false)
                    MessageBox.Show("Device is not Connected. Please Connect...");
                else
                {
                    if (selectedPort == null)
                        MessageBox.Show("No Device Selected...");
                    else
                    {
                        bool temp = false;
                        richTextBox2.Text = obj.ReadComPortGSMSMS(out temp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
