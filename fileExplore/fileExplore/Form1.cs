using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace fileExplore
{
    public partial class Form1 : Form
    {
        //string path = @"C:\";
        //public Form1()
        //{
        //     InitializeComponent();
        //    ListDirectory(treeViewEx, path);

        //}
        //private void ListDirectory(TreeView treeView, string path)
        //{
        //    treeView.Nodes.Clear();
        //    var rootDirectoryInfo = new DirectoryInfo(path);
        //    treeView.Nodes.Add(CreateDirectoryTreeNode(rootDirectoryInfo));
        //}
        //private static TreeNode CreateDirectoryTreeNode(DirectoryInfo directoryInfo)
        //{
        //    var directoryNode = new TreeNode(directoryInfo.Name);
        //   try
        //   {
        //        foreach (var directory in directoryInfo.GetDirectories())
        //        {
        //            directoryNode.Nodes.Add(CreateDirectoryTreeNode(directory));
        //        }
        //        foreach (var file in directoryInfo.GetFiles())
        //        {
        //            directoryNode.Nodes.Add(new TreeNode(file.Name));
        //        }
        //    }
        //    catch (UnauthorizedAccessException)
        //    {

        //    }

        //    return directoryNode;
        //}
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeViewEx.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeViewEx_NodeMouseClick);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;
            var ListDriverInfor = DriveInfo.GetDrives();
            foreach (DriveInfo drive in ListDriverInfor)
            {
                string path = drive.Name.ToString();
                DirectoryInfo info = new DirectoryInfo(path);

                if (info.Exists)
                {

                    rootNode = new TreeNode(info.Name);
                    rootNode.ImageIndex = 2;
                    rootNode.Tag = info;
                    GetDirectories(info.GetDirectories(), rootNode);
             
                    treeViewEx.Nodes.Add(rootNode);
                }
            }
            /*        DirectoryInfo info = new DirectoryInfo(@"E:\filehoctap");
                    if (info.Exists)
                    {
                        rootNode = new TreeNode(info.Name);
                        rootNode.Tag = info;
                        GetDirectories(info.GetDirectories(), rootNode);
                        rootNode.ImageIndex = 2;
                        treeViewEx.Nodes.Add(rootNode);
                    }*/
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                //subSubDirs = subDir.GetDirectories();
                try
                {
                    subSubDirs = subDir.GetDirectories(); 
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (UnauthorizedAccessException)
                {

                }
                catch (IOException)
                {

                }
            }
        }

        private void treeViewEx_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
                        new ListViewItem.ListViewSubItem(item,
                            dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
                        new ListViewItem.ListViewSubItem(item,
                        file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void treeViewEx_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
