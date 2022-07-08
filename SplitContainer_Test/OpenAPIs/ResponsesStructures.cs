using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTAPI.ResponsesStructures
{
    public class CreateAccountRequestSuccessResponse
    {
        public bool Status { get; set; } = false;

        public string userName { get; set; }

        public string token { get; set; }

    }

    public class LoginResponce
    {
        public bool Status { get; set; } = false;
        public string token { get; set; }
    }

    public class TrustDeviceSuccessResponce 
    {
        public bool Status { get; set; } = false;
        public string deviceToken { get; set; }
    }

    /*
    public class FileSystemStructureFolder
    {
        public string FolderName { get; set; } = String.Empty;

        public DateTime FolderEditTime { get; set; } = DateTime.Now;

        public List<FileSystemStructureFolder> Folders { get; set; } = new List<FileSystemStructureFolder>();

        public List<FileSystemStructureFile> Files { get; set; } = new List<FileSystemStructureFile>();
    }

    public class FileSystemStructureFile
    {
        public string FileName { get; set; } = String.Empty;
        public DateTime FileEditTime { get; set; } = DateTime.Now;
        public string FileHash { get; set; } = String.Empty;
        public long FileLenght { get; set; } = 0;
        public string FileToken { get; set; } = String.Empty;
    }
    */
}
