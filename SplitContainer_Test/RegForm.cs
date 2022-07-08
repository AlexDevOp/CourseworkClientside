using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudProjectClient
{
    public partial class RegForm : Form
    {
        public RegForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox2.Text;
            string fio = textBox1.Text;
            string password = textBox3.Text;


            var answer = GlobalScope.ApiController.CreateUserAsync(new RESTAPI.CreateAccountRequest { UserName = fio, Login = username, PassFingerprint = Convert.ToBase64String(GlobalScope.GetHashSha3(password)) }).GetAwaiter().GetResult();

            if (!answer.Status)
            {
                MessageBox.Show("Ошибка создания нового пользователя");
                return;
            }

            GlobalScope.UserToken = Convert.FromBase64String(answer.token);
            GlobalScope.UserName = answer.userName;


            DialogResult = DialogResult.OK;
        }
    }
}
