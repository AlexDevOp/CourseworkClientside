using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitContainer_Test
{
    public class FileSystemStructureWorker
    {
        public static FileSystemStructureFolder rootFolder;

        public static FileSystemStructureFolder CreateTestRootFolder()
        {
            rootFolder = new FileSystemStructureFolder();
            rootFolder.Folders.Add(new FileSystemStructureFolder { FolderName = "TestFolder1"});
            rootFolder.Folders.Add(new FileSystemStructureFolder { FolderName = "TestFolder2"});
            rootFolder.Files.Add(new FileSystemStructureFile { FileName = "TestFile1"});
            rootFolder.Files.Add(new FileSystemStructureFile { FileName = "TestFile2"});
            return rootFolder;

        }

        public static FileSystemStructureFolder GetFolderByPath(string folderPath)
        {
            FileSystemStructureFolder currentFolder = rootFolder;

            var foldersPaths = folderPath.Split('\\');

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
}
