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

namespace SplitContainer_Test
{
    public enum FileActions
    {
        Rename,
        Delete,
        Synchronize,
        OpenInExplorer,
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

        public CloudExplorer()
        {
            InitializeComponent();

            // добавляем элементы в меню
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Открыть");
            openMenuItem.Tag = true;

            ToolStripMenuItem openInExplorerMenuItem = new ToolStripMenuItem("Открыть в проводнике");
            openInExplorerMenuItem.Tag = true;

            ToolStripMenuItem synMenuItem = new ToolStripMenuItem("Синхронизировать");
            synMenuItem.Tag = false;

            contextMenu.Items.AddRange(new[] { openMenuItem, openInExplorerMenuItem, synMenuItem });

            // ассоциируем контекстное меню с панелью
            FolderItemsView.ContextMenuStrip = contextMenu;

            contextMenu.Opening += ContextMenu_Opening;
            // устанавливаем обработчики событий для контекстного меню
            openMenuItem.Click += openContextMenuItem_Click;
            openInExplorerMenuItem.Click += openInExplorerContextMenuItem_Click;
            synMenuItem.Click += synContextMenuItem_Click;

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
                    FileInfo f = new FileInfo(@s);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@s);
                    foreach (string s2 in str2)
                    {
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
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (FolderItemsView.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        private void FillFolderView(string path)
        {
            
            string[] allPaths = Directory.GetDirectories(path);
            ListViewItem lw;
            foreach (string localPath in allPaths)
            {
                DirectoryInfo d = new DirectoryInfo(@localPath);
                if (d.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;

                lw = new ListViewItem(new string[] { localPath.Substring(localPath.LastIndexOf('\\') + 1), "Папка", "", d.LastWriteTime.ToString() }, 0);
                lw.Name = localPath;
                lw.Tag = false;
                FolderItemsView.Items.Add(lw);
            }

            allPaths = Directory.GetFiles(path);
            foreach (string localPath in allPaths)
            {
                FileInfo f = new FileInfo(@localPath);
                if (f.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;

                lw = new ListViewItem(new string[] { localPath.Substring(localPath.LastIndexOf('\\') + 1), "Файл", GetFileLenghtString(f.Length), f.LastWriteTime.ToString() }, 1);
                lw.Name = localPath;
                lw.Tag = true;
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
            try
            {
                FillFolderView(currListViewAdress);
            }
            catch { }

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            //обработка двойного нажатия по папке или файлу в ListView
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
                    string[] str2 = Directory.GetDirectories(@tn.Name);
                    foreach (string str in str2)
                    {
                        TreeNode temp = new TreeNode();
                        temp.Name = str;
                        temp.Text = str.Substring(str.LastIndexOf('\\') + 1);
                        e.Node.Nodes[i].Nodes.Add(temp);
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

        private void synContextMenuItem_Click(object sender, EventArgs e)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            Boolean HasLargeFiles = false;

            foreach (ListViewItem selectedItem in FolderItemsView.SelectedItems)
            {
                if ((bool)selectedItem.Tag)
                {
                    FileInfo f = new FileInfo(@selectedItem.Name);
                    if (f.Length / 1024 / 1024 > 10)
                    {
                        HasLargeFiles = true;
                        continue;
                    }

                    fileInfos.Add(f);
                }
                else
                {
                    RecursivelyFilesAdding(fileInfos, selectedItem.Name, HasLargeFiles);
                }

                if (fileInfos.Count > 50)
                {
                    MessageBox.Show("Ограничения демонстрационной версии не позволяют синхронизировать больше 50 файлов");
                    return;
                }
            }

            if (HasLargeFiles)
            {
                MessageBox.Show("Ограничения демонстрационной версии не позволяют файлы размером больше 10 МБ\n" +
                    "Эти файлы будут пропущены");
            }

            if (fileInfos.Count == 0)
            {
                MessageBox.Show("Нет файлов для синхронизации");
                return;
            }

            Console.WriteLine(fileInfos.Count);

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
            FillFolderView(currListViewAdress);
        }
    }
}
