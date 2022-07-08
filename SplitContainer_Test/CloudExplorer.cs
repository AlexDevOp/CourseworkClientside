using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Security.AccessControl;
using RESTAPI;

namespace CloudProjectClient
{
    public enum FileActions
    {
        Rename,
        Delete,
        Synchronize,
        OpenInExplorer,
    }

    public enum CloudContext
    {
        Folder,
        File,
        None
    }


    public partial class CloudExplorer : Form
    {
        //коллекция посещённых адрессов
        ArrayList Adresses = new ArrayList();
        //индекс текущего адресса из коллекции Adresses
        int currIndex =-1;
        //текущий адресс
        string currListViewAdress = "";

        //сортировальщик
        ListViewColumnSorter listViewColumnSorter;

        private ContextMenuStrip WindowsExplorerMenu;
        private ContextMenuStrip CloudExplorerMenu;

        public CloudExplorer()
        {
            InitializeComponent();
            GlobalScope.ApiController.LoadStructureAsync();

            // добавляем элементы в меню
            WindowsExplorerMenu = new ContextMenuStrip();
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Открыть");
            openMenuItem.Tag = true;

            ToolStripMenuItem openInExplorerMenuItem = new ToolStripMenuItem("Открыть в проводнике");
            openInExplorerMenuItem.Tag = true;

            WindowsExplorerMenu.Items.AddRange(new[] { openMenuItem, openInExplorerMenuItem});

            // ассоциируем контекстное меню с панелью
            FolderItemsView.ContextMenuStrip = WindowsExplorerMenu;

            WindowsExplorerMenu.Opening += ContextMenu_Opening;
            // устанавливаем обработчики событий для контекстного меню
            openMenuItem.Click += openContextMenuItem_Click;
            openInExplorerMenuItem.Click += openInExplorerContextMenuItem_Click;


            CloudExplorerMenu = new ContextMenuStrip();
            ToolStripMenuItem OpenCloudFolder = new ToolStripMenuItem("Открыть");
            OpenCloudFolder.Tag = CloudContext.Folder;
            OpenCloudFolder.Click += OpenCloudFolder_Click;

            ToolStripMenuItem DeleteCloudFolder = new ToolStripMenuItem("Удалить папку");
            DeleteCloudFolder.Tag = CloudContext.Folder;
            DeleteCloudFolder.Click += DeleteCloudFolder_Click;

            ToolStripMenuItem RenameCloudFolder = new ToolStripMenuItem("Переименовать папку");
            RenameCloudFolder.Tag = CloudContext.Folder;
            RenameCloudFolder.Click += RenameCloudFolder_Click;

            //ToolStripMenuItem DownloadCloudFolder = new ToolStripMenuItem("Скачать папку");

            ToolStripMenuItem CreateCloudFolder = new ToolStripMenuItem("Создать папку");
            CreateCloudFolder.Tag = CloudContext.None;
            CreateCloudFolder.Click += CreateCloudFolder_Click;

            ToolStripMenuItem UploadCloudFile = new ToolStripMenuItem("Загрузить в облако файл");
            UploadCloudFile.Tag = CloudContext.None;
            UploadCloudFile.Click += UploadCloudFile_Click;

            ToolStripMenuItem OpenCloudFile = new ToolStripMenuItem("Открыть файл");
            OpenCloudFile.Tag = CloudContext.File;
            OpenCloudFile.Click += OpenCloudFile_Click;

            ToolStripMenuItem DeleteCloudFile = new ToolStripMenuItem("Удалить файл");
            DeleteCloudFile.Tag = CloudContext.File;
            DeleteCloudFile.Click += DeleteCloudFile_Click;

            ToolStripMenuItem RenameCloudFile = new ToolStripMenuItem("Переименовать файл");
            RenameCloudFile.Tag = CloudContext.File;
            RenameCloudFile.Click += RenameCloudFile_Click;

            CloudExplorerMenu.Items.AddRange(new[] { OpenCloudFolder, CreateCloudFolder, DeleteCloudFolder, RenameCloudFolder, UploadCloudFile, OpenCloudFile, DeleteCloudFile, RenameCloudFile });

            CloudExplorerMenu.Opening += CloudExplorerMenu_Opening;


            //добавления колонок
            FolderItemsView.ColumnClick += new ColumnClickEventHandler(ClickOnColumn);

            listViewColumnSorter = new ListViewColumnSorter();
            FolderItemsView.ListViewItemSorter = listViewColumnSorter;

            //заполнение TreeView узлами локальных дисков и заполнение дочерних узлов этих дисков
            string[] str = Environment.GetLogicalDrives();
            int n=1;
            foreach(string s in str)
            {
                try
                {
                    TreeNode tn = new TreeNode();
                    tn.Name = s;
                    tn.Text = "Локальный диск " + s;
                    treeView1.Nodes.Add(tn.Name, tn.Text, 2);

                    string t = "";
                    string[] str2 = Directory.GetDirectories(@s);
                    foreach (string s2 in str2)
                    {
                        DirectoryInfo d = new DirectoryInfo(@s2);
                        if (d.Attributes.HasFlag(FileAttributes.Hidden))
                            continue;

                        t = s2.Substring(s2.LastIndexOf('\\')+1);
                        ((TreeNode)treeView1.Nodes[n - 1]).Nodes.Add(s2, t, 0);
                    }
                }
                catch { }
                n++;
            }
            foreach (TreeNode tn in treeView1.Nodes)
            {
                for (int i = 65; i < 91; i++)
                {
                    char sym = Convert.ToChar(i);
                    if (tn.Name == sym + ":\\")
                        tn.SelectedImageIndex = 2;
                }
            }

            treeView1.Nodes.Add(@"Cloud:\", "Облако", 0);
        }

        internal void ReloadTreeView()
        {
            if (IsCloudOpened() && GlobalScope.rootFolder != null)
            {
                FolderItemsView.Items.Clear();
                Adresses.Clear();
                currIndex = 0;

                currListViewAdress = @"Cloud:\";
                toolStripTextBox1.Text = currListViewAdress;
                FillCloudFolderView(GlobalScope.rootFolder);
            }
        }

        private void UploadCloudFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            { 
                if (openFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    GlobalScope.ApiController.UploadFileAsync(currListViewAdress.Replace(@"Cloud:", ""), openFileDialog.SafeFileName, new FileParameter(openFileDialog.OpenFile()) );
                }
            }
        }

        private void CreateCloudFolder_Click(object sender, EventArgs e)
        {
            using (var form = new EnterNameForm())
            {
                DialogResult result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    IsCloudFolderExists(currListViewAdress+ $@"\{form.folderNameTextBox.Text}");
                    GlobalScope.ApiController.CreateFolderAsync(new CreateFolderRequest { NewFolderName= form.folderNameTextBox.Text, NewFolderPath = currListViewAdress.Replace(@"Cloud:", "") });
                }
            }
        }

        private bool IsCloudFolderExists(string cloudPath)
        {
            return FileSystemStructureWorker.GetFolderByPath(cloudPath) != null;
        }

        private void RenameCloudFile_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteCloudFile_Click(object sender, EventArgs e)
        {
            if (FolderItemsView.SelectedItems.Count != 1)
                return;

            FileSystemStructureFile file = (FileSystemStructureFile)FolderItemsView.SelectedItems[0].Tag;

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string path = currListViewAdress.Replace(@"Cloud:\", "");

                GlobalScope.ApiController.DeleteFileAsync(new DeleteFileRequest { FolderPath= path, FileName= file.FileName} );
            }
        }

        private void OpenCloudFile_Click(object sender, EventArgs e)
        {
            if (FolderItemsView.SelectedItems.Count != 1)
                return;

            FileSystemStructureFile file = (FileSystemStructureFile)FolderItemsView.SelectedItems[0].Tag;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = file.FileName; // Default file name
                saveFileDialog.Filter = "All files(*.*)";
                if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    GlobalScope.ApiController.DonwloadFileAsync(new DownloadFileRequest { FolderPath = currListViewAdress.Replace(@"Cloud:", ""), FileToken = file.FileToken }, saveFileDialog.FileName);
                }
            }



        }

        private void RenameCloudFolder_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteCloudFolder_Click(object sender, EventArgs e)
        {
            if (FolderItemsView.SelectedItems.Count != 1)
                return;

            FileSystemStructureFolder folder = (FileSystemStructureFolder)FolderItemsView.SelectedItems[0].Tag;

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string path = currListViewAdress + $@"\{folder.FolderName}";
                path = path.Replace(@"Cloud:\", "");


                GlobalScope.ApiController.DeleteFolderAsync(new DeleteFolderRequest {FolderPath= path });
            }
        }

        private void OpenCloudFolder_Click(object sender, EventArgs e)
        {
            if (FolderItemsView.SelectedItems.Count == 1)
            {
                OpenCloudFolder((FileSystemStructureFolder)FolderItemsView.SelectedItems[0].Tag);
            }
        }

        private void CloudExplorerMenu_Opening(object sender, CancelEventArgs e)
        {
            if (GlobalScope.rootFolder == null)
            {
                e.Cancel = true;
                return;
            }

            if (FolderItemsView.SelectedItems.Count == 0)
            {
                foreach (ToolStripMenuItem context in CloudExplorerMenu.Items)
                {
                    switch ((CloudContext)context.Tag)
                    {
                        case CloudContext.Folder:
                            context.Visible = false;
                            break;
                        case CloudContext.File:
                            context.Visible = false;
                            break;
                        case CloudContext.None:
                            context.Visible = true;
                            break;
                    }
                }
            }
            else if (FolderItemsView.SelectedItems.Count == 1)
            {
                var item = FolderItemsView.SelectedItems[0].Tag;
                if (item.GetType() == typeof (FileSystemStructureFile))
                {
                    foreach (ToolStripMenuItem context in CloudExplorerMenu.Items)
                    {
                        switch((CloudContext)context.Tag)
                        {
                           case CloudContext.Folder:
                                context.Visible = false;
                                break;
                           case CloudContext.File:
                                context.Visible = true;
                                break;
                           case CloudContext.None:
                                context.Visible = false;
                                break;
                        }
                    }
                }
                else
                {
                    foreach (ToolStripMenuItem context in CloudExplorerMenu.Items)
                    {
                        switch ((CloudContext)context.Tag)
                        {
                            case CloudContext.Folder:
                                context.Visible = true;
                                break;
                            case CloudContext.File:
                                context.Visible = false;
                                break;
                            case CloudContext.None:
                                context.Visible = false;
                                break;
                        }
                    }
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void FillCloudTreeView()
        {
            treeView1.Nodes[treeView1.Nodes.Count - 1].Collapse();
            treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes.Clear();

            foreach (var folder in GlobalScope.rootFolder.Folders)
            {
                ((TreeNode)treeView1.Nodes[treeView1.Nodes.Count-1]).Nodes.Add(@"Cloud:\"+folder.FolderName, folder.FolderName, 0);
            }
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (FolderItemsView.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        private void FillFolderView(string path)
        {
            FolderItemsView.ContextMenuStrip = WindowsExplorerMenu;
            string[] allPaths = Directory.GetDirectories(path);
            ListViewItem lw;
            foreach (string localPath in allPaths)
            {
                DirectoryInfo d = new DirectoryInfo(@localPath);
                if (d.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;
                var name = localPath.Substring(localPath.LastIndexOf('\\') + 1);
                lw = new ListViewItem(new string[] { name, "Папка", "", d.LastWriteTime.ToString() }, 0);
                lw.Name = localPath;
                lw.Tag = new WindowsFolderTag { Name = name, LastEditTime = d.LastWriteTime };
                FolderItemsView.Items.Add(lw);
            }

            allPaths = Directory.GetFiles(path);
            foreach (string localPath in allPaths)
            {
                FileInfo f = new FileInfo(@localPath);
                if (f.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;
                var name = localPath.Substring(localPath.LastIndexOf('\\') + 1);
                var lenght = f.Length;
                lw = new ListViewItem(new string[] { name, "Файл", GetFileLenghtString(lenght), f.LastWriteTime.ToString() }, 1);
                lw.Name = localPath;
                lw.Tag = new WindowsFileTag { Name = name , Length = lenght, LastEditTime = f.LastWriteTime };
                FolderItemsView.Items.Add(lw);
            }
        }


        public void FillCloudFolderView(FileSystemStructureFolder path)
        {
            FolderItemsView.ContextMenuStrip = CloudExplorerMenu;
            if (path == null)
                return;

            FileSystemStructureFolder folder = path;
            ListViewItem lw;
            foreach(FileSystemStructureFolder f in folder.Folders)
            {
                lw = new ListViewItem(new string[] { f.FolderName.Substring(f.FolderName.LastIndexOf('\\') + 1), "Папка", "", f.FolderEditTime.ToString() }, 0);
                lw.Name = f.FolderName;
                lw.Tag = f;
                FolderItemsView.Items.Add(lw);
            }


            foreach (FileSystemStructureFile f in folder.Files)
            {
                lw = new ListViewItem(new string[] { f.FileName.Substring(f.FileName.LastIndexOf('\\') + 1), "Файл", GetFileLenghtString(f.FileLenght), f.FileEditTime.ToString() }, 1);
                lw.Name = f.FileName;
                lw.Tag = f;
                FolderItemsView.Items.Add(lw);
            }
        }


        private string GetFileLenghtString(long fileLenght)
        {
            if (fileLenght < 1024)
            {
                return $"{fileLenght} байт";
            }

            fileLenght /= 1024;

            if (fileLenght < 1024)
            {
                return $"{fileLenght} КБ";
            }

            fileLenght /= 1024;

            if (fileLenght < 1024)
            {
                return $"{fileLenght} МБ";
            }

            fileLenght /= 1024;

            return $"{fileLenght} ГБ";
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (Adresses.Count != 0)
            {
                string strtmp = ((string)Adresses[Adresses.Count - 1]);
                Adresses.Clear();
                Adresses.Add(strtmp);
                currIndex = 0;
            }            
            Adresses.Add(e.Node.Name);
            currIndex++;
            //проверка возможности перехода назад/вперёд
            if (currIndex + 1 == Adresses.Count)
                toolStripButton2.Enabled = false;
            else
                toolStripButton2.Enabled = true;
            if (currIndex - 1 == -1)
                toolStripButton1.Enabled = false;
            else
                toolStripButton1.Enabled = true;



            FolderItemsView.Items.Clear();
            currListViewAdress = e.Node.Name;
            toolStripTextBox1.Text = currListViewAdress;
            //заполнение ListView

            if (currListViewAdress.Contains(@"Cloud:\") && GlobalScope.rootFolder != null)
                FillCloudFolderView(FileSystemStructureWorker.GetFolderByPath(currListViewAdress.Replace(@"Cloud:\","")));
            else
                FillFolderView(currListViewAdress);
        }

        private bool IsCloudOpened()
        {
            return currListViewAdress.Contains(@"Cloud:\");
        }

        private void OpenCloudFolder(FileSystemStructureFolder folder)
        {
            FolderItemsView.Items.Clear();
            currListViewAdress += @"\" + folder.FolderName;
            currListViewAdress = currListViewAdress.Replace(@"\\", @"\");
            toolStripTextBox1.Text = currListViewAdress;
            Adresses.Add(currListViewAdress);
            currIndex++;
            FillCloudFolderView(FileSystemStructureWorker.GetFolderByPath(currListViewAdress.Replace(@"Cloud:\", "")));
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IsCloudOpened())
            {
                if (FolderItemsView.SelectedItems[0].Tag.GetType() == typeof(FileSystemStructureFolder))
                {
                    OpenCloudFolder((FileSystemStructureFolder)FolderItemsView.SelectedItems[0].Tag);
                }
                else if (FolderItemsView.SelectedItems[0].Tag.GetType() == typeof(FileSystemStructureFile))
                {
                    FileSystemStructureFile file = (FileSystemStructureFile)FolderItemsView.SelectedItems[0].Tag;

                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.FileName = file.FileName; // Default file name
                        saveFileDialog.Filter = "All files(*.*)|*.*";
                        if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
                        {
                            GlobalScope.ApiController.DonwloadFileAsync(new DownloadFileRequest { FolderPath = currListViewAdress.Replace(@"Cloud:", ""), FileToken = file.FileToken }, saveFileDialog.FileName);
                        }
                    }
                }
            }
            else
            {
                //обработка двойного нажатия по папке или файлу в ListView
                if (FolderItemsView.SelectedItems[0].Tag.GetType() == typeof(WindowsFileTag))
                {
                    try
                    {
                        //обработка нажатия на файл(его запуска)
                        System.Diagnostics.Process MyProc = new System.Diagnostics.Process();
                        MyProc.StartInfo.FileName = FolderItemsView.SelectedItems[0].Name;
                        MyProc.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}");
                    }
                }

                else
                {
                    OpenFolder(FolderItemsView.SelectedItems[0].Name);
                }
            }
            
        }

        private void ClickOnColumn(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == listViewColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (listViewColumnSorter.Order == SortOrder.Ascending)
                {
                    listViewColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    listViewColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                listViewColumnSorter.SortColumn = e.Column;
                listViewColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.FolderItemsView.Sort();
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            int i = 0;
            //заполнение дочерних узлов дочерними узлами развёртываемого узала
            try
            {
                foreach (TreeNode tn in e.Node.Nodes)
                {
                    if (@tn.Name.Contains(@"Cloud:\"))
                    {
                        foreach (var folder in FileSystemStructureWorker.GetFolderByPath(@tn.Name.Replace(@"Cloud:\", "")).Folders)
                        {
                            TreeNode temp = new TreeNode();
                            temp.Text = folder.FolderName;
                            temp.Name = e.Node.Nodes[i].Name + @"\" + folder.FolderName;
                            e.Node.Nodes[i].Nodes.Add(temp);
                        }
                    }
                    else
                    {
                        string[] str2 = Directory.GetDirectories(@tn.Name);
                        foreach (string str in str2)
                        {
                            TreeNode temp = new TreeNode();
                            temp.Name = str;
                            temp.Text = str.Substring(str.LastIndexOf('\\') + 1);
                            e.Node.Nodes[i].Nodes.Add(temp);
                        }
                    }



                    i++;
                }
            }
            catch { }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //обработка "Назад"
            if (currIndex - 1 != -1)
            {
                currIndex--;
                currListViewAdress = ((string)Adresses[currIndex]);
                if (currIndex + 1 == Adresses.Count)
                    toolStripButton2.Enabled = false;
                else
                    toolStripButton2.Enabled = true;
                if (currIndex - 1 == -1)
                    toolStripButton1.Enabled = false;
                else
                    toolStripButton1.Enabled = true;
                toolStripTextBox1.Text = currListViewAdress;
                FolderItemsView.Items.Clear();

                try
                {
                    if (currListViewAdress.Contains(@"Cloud:\"))
                        FillCloudFolderView(FileSystemStructureWorker.GetFolderByPath(currListViewAdress.Replace(@"Cloud:\", "")));
                    else
                        FillFolderView(currListViewAdress);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //обработка "Вперёд"
            if (currIndex + 1 != Adresses.Count)
            {
                currIndex++;
                currListViewAdress = ((string)Adresses[currIndex]);
                if (currIndex + 1 == Adresses.Count)
                    toolStripButton2.Enabled = false;
                else
                    toolStripButton2.Enabled = true;
                if (currIndex - 1 == -1)
                    toolStripButton1.Enabled = false;
                else
                    toolStripButton1.Enabled = true;
                toolStripTextBox1.Text = currListViewAdress;
                FolderItemsView.Items.Clear();          

                try
                {
                    if (currListViewAdress.Contains(@"Cloud:\"))
                        FillCloudFolderView(FileSystemStructureWorker.GetFolderByPath(currListViewAdress.Replace(@"Cloud:\", "")));
                    else
                        FillFolderView(currListViewAdress);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }
        }

        private void FolderItemsView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && FolderItemsView.SelectedItems.Count == 1)
            {
                OpenFolder(FolderItemsView.SelectedItems[0].Name);
            }
        }

        private void AddToAddresses(string folderPath)
        {
            if (currIndex + 1 < Adresses.Count)
            {
                for (int i = Adresses.Count-1; i > currIndex; i--)
                {
                    Adresses.RemoveAt(i);
                }
            }

            Adresses.Add(folderPath);
            currIndex++;
            if (currIndex + 1 == Adresses.Count)
                toolStripButton2.Enabled = false;
            else
                toolStripButton2.Enabled = true;
            if (currIndex - 1 == -1)
                toolStripButton1.Enabled = false;
            else
                toolStripButton1.Enabled = true;
        }

        private void OpenFolder(string folderPath)
        {
            string exFolderPath = currListViewAdress;
            try
            {
                currListViewAdress = folderPath;
                toolStripTextBox1.Text = currListViewAdress;
                AddToAddresses(folderPath);

                FolderItemsView.Items.Clear();
                FillFolderView(currListViewAdress);
            }
            catch (Exception ex)
            {
                Adresses.RemoveAt(currIndex);
                currIndex--;
                OpenFolder(exFolderPath);
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //проверка на то что был нажат enter, если был нажат entet и введённый адресс синтаксически верен, то будет произведён переход
            if (e.KeyValue == 13)
            {
                if (new DirectoryInfo(toolStripTextBox1.Text).Exists)
                    OpenFolder(toolStripTextBox1.Text);
                else
                    MessageBox.Show($"Произошла ошибка: Папки не существует");
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (FolderItemsView.SelectedItems.Count == 0)
                return;

            foreach (ToolStripMenuItem context in FolderItemsView.ContextMenuStrip.Items)
            {
                if (FolderItemsView.SelectedItems.Count > 1 && (bool)context.Tag)
                {
                    context.Enabled = false;
                }
                else
                {
                    context.Enabled = true;
                }
            }
        }

        private void RecursivelyFilesAdding(List<FileInfo> fileInfos, string folderPath, Boolean hasLargeFiles)
        {
            string[] allfolders = Directory.GetDirectories(folderPath);
            foreach (string folder in allfolders)
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(@folderPath);
                    if (d.Attributes.HasFlag(FileAttributes.Hidden))
                        continue;

                    RecursivelyFilesAdding(fileInfos, folder, hasLargeFiles);

                    if (fileInfos.Count > 50)
                    {
                        return;
                    }
                }
                catch
                {
                }
            }

            string[] allfiles = Directory.GetFiles(folderPath);
            foreach (string file in allfiles)
            {
                try
                {
                    FileInfo f = new FileInfo(@file);
                    if (f.Attributes.HasFlag(FileAttributes.Hidden))
                        continue;

                    if (f.Length / 1024 / 1024 > 10)
                    {
                        hasLargeFiles = true;
                        continue;
                    }

                    fileInfos.Add(f);
                    if (fileInfos.Count > 50)
                    {
                        return;
                    }
                }
                catch
                {
                }
            }
        }

        private void openInExplorerContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((bool)FolderItemsView.SelectedItems[0].Tag)
            {
                System.Diagnostics.Process MyProc = new System.Diagnostics.Process();
                MyProc.StartInfo.FileName = @FolderItemsView.SelectedItems[0].Name;
                MyProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                MyProc.StartInfo.FileName = "explorer";
                MyProc.StartInfo.Arguments = @"/n, /select, " + FolderItemsView.SelectedItems[0].Name;
                MyProc.Start();

            }
            else
            {
                System.Diagnostics.Process MyProc = new System.Diagnostics.Process();
                MyProc.StartInfo.FileName = @FolderItemsView.SelectedItems[0].Name;
                MyProc.Start();
            }
        }

        private void openContextMenuItem_Click(object sender, EventArgs e)
        {
            //обработка двойного нажатия по папке или файла в ListView
            if ((bool)FolderItemsView.SelectedItems[0].Tag)
            {
                try
                {
                    //обработка нажатия на файл(его запуска)
                    System.Diagnostics.Process MyProc = new System.Diagnostics.Process();
                    MyProc.StartInfo.FileName = FolderItemsView.SelectedItems[0].Name;
                    MyProc.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }

            else
            {
                OpenFolder(FolderItemsView.SelectedItems[0].Name);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            FolderItemsView.Items.Clear();
            Adresses.Clear();
            currIndex = 0;
            treeView1.Nodes[treeView1.Nodes.Count - 1].Collapse();

            if (currListViewAdress.Contains(@"Cloud:\"))
            {
                treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes.Clear();

                GlobalScope.ApiController.LoadStructureAsync();
            }
            else
                FillFolderView(currListViewAdress);

        }

        private void выйтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalScope.Settings.DeviceAuth = new DeviceAuthSettings();
            this.Close();
        }
    }

    public class WindowsFolderTag
    {
        public string Name { get; set; }

        public DateTime LastEditTime { get; set; }
    }

    public class WindowsFileTag
    {
        public string Name { get; set; }

        public DateTime LastEditTime { get; set; }

        public long Length { get; set; }
    }
}
