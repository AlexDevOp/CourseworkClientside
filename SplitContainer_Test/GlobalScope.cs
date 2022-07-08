using RESTAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudProjectClient
{
    internal class GlobalScope
    {
        public static FileSystemStructureFolder rootFolder = new FileSystemStructureFolder();
        public static DateTime StructureTimestamp;
        public static CloudExplorer Window;

        public static CloudApi ApiController;

        public static byte[] UserToken;

        public static string UserName;

        public static ClientSettings Settings;

        public static string Getbase64UserToken()
        {
            return Convert.ToBase64String(UserToken);
        }

        public static byte[] GetHashSha3(string input)
        {
            var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);

            // Choose correct encoding based on your usecase
            byte[] inputArray = Encoding.Unicode.GetBytes(input);

            hashAlgorithm.BlockUpdate(inputArray, 0, inputArray.Length);

            byte[] result = new byte[64]; // 512 / 8 = 64
            hashAlgorithm.DoFinal(result, 0);

            return result;
        }
        public static byte[] GetHashSha3(byte[] input)
        {
            var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);

            hashAlgorithm.BlockUpdate(input, 0, input.Length);

            byte[] result = new byte[64]; // 512 / 8 = 64
            hashAlgorithm.DoFinal(result, 0);

            return result;
        }

        public static string GetDeviceFingerprint()
        {
            return Convert.ToBase64String(GetHashSha3(System.Security.Principal.WindowsIdentity.GetCurrent().User.Value));
        }
        delegate void FillCloudTreeViewDelegate();
        delegate void ReloadTreeViewDelegate();
        
        public static void OnStructureLoaded()
        {
            FillCloudTreeViewDelegate d = Window.FillCloudTreeView;
            Window.Invoke(d);

            ReloadTreeViewDelegate rd = Window.ReloadTreeView;
            Window.Invoke(rd);
        }

        internal static void ReloadView()
        {
            ReloadTreeViewDelegate d = Window.ReloadTreeView;
            Window.Invoke(d);
        }
    }
}
