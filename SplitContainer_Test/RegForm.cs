using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplitContainer_Test
{
    public partial class RegForm : Form
    {
        public RegForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username;
            string fio;
            string password;

            fio = textBox1.Text;
            username = textBox2.Text;
            password = textBox3.Text;
            if (false)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Регистрация провалилась, Милорд!");
            }
        }
    }
}
