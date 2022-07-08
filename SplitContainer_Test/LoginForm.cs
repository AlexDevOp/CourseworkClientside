using RESTAPI;
using RESTAPI.ResponsesStructures;
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
            Console.WriteLine(username);
            Console.WriteLine(Convert.ToBase64String(GlobalScope.GetHashSha3(password)));
            var answer = GlobalScope.ApiController.LoginAsync(new RESTAPI.LoginRequest { Login = username, PassFingerprint=Convert.ToBase64String(GlobalScope.GetHashSha3(password))}).GetAwaiter().GetResult();

            if (!answer.Status)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            GlobalScope.UserToken = Convert.FromBase64String(answer.token);

            if (checkBox1.Checked)
            {
                GlobalScope.ApiController.AddTrustedDeviceAsync(new TrustDeviceRequest { DeviceFingerprint = GlobalScope.GetDeviceFingerprint() })
                    .ContinueWith(task =>
                    {
                        if (task.Result.Status)
                        {
                            GlobalScope.Settings.DeviceAuth.Active = true;
                            GlobalScope.Settings.DeviceAuth.DeviceAuthKey = task.Result.deviceToken;
                        }
                    });
            }


            DialogResult = DialogResult.OK;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result;
            using (var Reg = new RegForm())
                result = Reg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    DialogResult = DialogResult.OK;
                }
        }
    }
}
