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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username;
            string password;

            username = TB_username.Text;
            password = TB_password.Text;

            if (/*User.Login(username, password)*/ false)
            {
                //Globals._Login = true;
                if (checkBox1.Checked)
                {

                }
                DialogResult = DialogResult.OK;

            }
            else
            {
                MessageBox.Show("Регистрация провалилась, Ваше превосходительство!");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result;
            using (var Reg = new RegForm())
                result = Reg.ShowDialog();
                if (result == DialogResult.OK)
                {

                }
        }
    }
}
