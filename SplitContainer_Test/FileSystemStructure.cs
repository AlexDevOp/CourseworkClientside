using RESTAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudProjectClient
{
    public class FileSystemStructureWorker
    {

        public static FileSystemStructureFolder GetFolderByPath(string folderPath = "")
        {
            FileSystemStructureFolder currentFolder = GlobalScope.rootFolder;

            if (folderPath.Length > 0 && folderPath[0] == '\\')
            {
                folderPath = folderPath.Remove(0, 1);
            }

            if (folderPath.Length == 0)
            {
                return currentFolder;
            }

            var foldersPaths = folderPath.Split('\\');

            if (foldersPaths.Length == 0)
                return currentFolder;

            foreach (var folderpath in foldersPaths)
            {
                currentFolder = currentFolder.Folders.Find(_folder => _folder.FolderName == folderpath);

                if (currentFolder == null)
                    return null;
            }

            return currentFolder;
        }

        public static void FolderIterator(FileSystemStructureFolder folder)
        {
            foreach (var file in folder.Files)
            {

                Console.WriteLine(file.FileName);
            }

            foreach (var childFolder in folder.Folders)
            {
            
                Console.WriteLine(childFolder.FolderName);
            }
        }
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
