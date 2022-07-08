using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace CloudProjectClient
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GlobalScope.ApiController = new RESTAPI.CloudApi("https://localhost:7193/", new System.Net.Http.HttpClient());
            GlobalScope.ApiController.ReadResponseAsString = true;

            try
            {
                GlobalScope.ApiController.HiTestAsync().Wait();
            }
            catch
            {
                MessageBox.Show("Нет соединения с сервером");
                return;
            }
           //Console.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileInfo config = new FileInfo(Directory.GetCurrentDirectory()+@"\settings.json");
            if (!config.Exists)
            {
                File.WriteAllText(config.FullName, JObject.FromObject(new ClientSettings()).ToString());
            }

            GlobalScope.Settings = JObject.Parse(File.ReadAllText(config.FullName)).ToObject<ClientSettings>();


            //var answer = GlobalScope.ApiController.LoginAsync(new RESTAPI.LoginRequest { Login = "Test", PassFingerprint=Convert.ToBase64String(GlobalScope.GetHashSha3("test")) }).GetAwaiter().GetResult();
            //Console.WriteLine(answer.Status);

            if (GlobalScope.Settings.DeviceAuth.Active)
            {
                GlobalScope.ApiController.DeviceLoginAsync(new RESTAPI.LoginByDeviceRequest { Device_id = GlobalScope.GetDeviceFingerprint(), Device_token = GlobalScope.Settings.DeviceAuth.DeviceAuthKey })
                    .ContinueWith(t =>
                    {
                        if (t.Result.Status)
                        {
                            GlobalScope.UserToken = Convert.FromBase64String(t.Result.token);
                        }
                        else
                        {
                            GlobalScope.Settings.DeviceAuth = new DeviceAuthSettings();
                            MessageBox.Show("Ошибка входа по токену. Перезапустите приложение");
                            File.WriteAllText(config.FullName, JObject.FromObject(GlobalScope.Settings).ToString());
                            Application.Exit();
                        }
                    });

                GlobalScope.Window = new CloudExplorer();
                Application.Run(GlobalScope.Window);

            }
            else
            {
                DialogResult result;
                using (var loginForm = new LoginForm())
                    result = loginForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    GlobalScope.Window = new CloudExplorer();
                    // login was successful
                    Application.Run(GlobalScope.Window);
                }
            }

            File.WriteAllText(config.FullName, JObject.FromObject(GlobalScope.Settings).ToString());
        }



    }

    public class ClientSettings
    {
        public DeviceAuthSettings DeviceAuth { get; set; } = new DeviceAuthSettings();

    }

    public class DeviceAuthSettings
    {
        public bool Active { get; set; } = false;
        public string DeviceAuthKey { get; set; } = String.Empty;
    }




}
