using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SplitContainer_Test
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RESTAPI.CloudApi api = new RESTAPI.CloudApi("https://localhost:7193/", new System.Net.Http.HttpClient());


            Console.WriteLine(api.HiAsync().GetAwaiter().GetResult());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CloudExplorer());



        }



    }
}
