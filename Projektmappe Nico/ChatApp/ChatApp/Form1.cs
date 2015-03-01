using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CSerialPortCOM a = new CSerialPortCOM(true);
            bool b = true;
            ChatOrganization o = new ChatOrganization("Nico", null, "Hallo", a, ref b);
            MessageBox.Show(b.ToString());
        }
    }
}
