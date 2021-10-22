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
        string path = @"C:\";
        public Form1()
        {
             InitializeComponent();
            ListDirectory(treeViewEx, path);
   
        }
        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var ListDriverInfor = DriveInfo.GetDrives();
            foreach (DriveInfo drive in ListDriverInfor)
            {
                path = drive.Name.ToString();
               var rootDirectoryInfo = new DirectoryInfo(path);
                //MessageBox.Show(drive.Name);
                treeView.Nodes.Add(CreateDirectoryTreeNode(rootDirectoryInfo));
            }
              

        }
        private static TreeNode CreateDirectoryTreeNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
           try
           {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    directoryNode.Nodes.Add(CreateDirectoryTreeNode(directory));
                }
                foreach (var file in directoryInfo.GetFiles())
                {
                    directoryNode.Nodes.Add(new TreeNode(file.Name));
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (System.IO.IOException)
            {

            }

            return directoryNode;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
        private void test()
        {

        }
    }
}
